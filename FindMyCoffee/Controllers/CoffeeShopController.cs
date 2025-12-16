using AutoMapper;
using FindMyCoffee.Data;
using FindMyCoffee.Data.API;
using FindMyCoffee.Data.HttpResponses;
using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Dots;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;
using FindMyCoffee.Services.Calculations;
using FindMyCoffee.Services.Distance;
using FindMyCoffee.Services.Validation;
using FindMyCoffee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;


/*
 * Central controller for all coffee shop logic.
 *
 * Handles:
 *  - Creating, updating, and deleting coffee shops
 *  - Searching shops by distance, rating, name, and type
 *  - Using external geocoding services to resolve addresses
 *
 * Includes optional integrations with the Google Places API (currently disabled).
 */

[Route("api/[controller]")]
[ApiController]
public class CoffeeShopController : ControllerBase
{
    //private readonly MockShopRepo _repository = new MockShopRepo();// Acts as a fake coffeeShop finder from a fake database
    private readonly ICoffeeShopRepository _repository;
    private readonly IMapper _mapper;               // Mapping from one 
    private readonly string GeoapifyApiKey;         // API that generates longitude and latitude based on giving address
    private readonly FindMyCoffeeContext _context;  // Communicates with the database FindMyCoffeeContext inheriting from DbContext

    //For Google API
    private readonly IConfiguration _config;
    private readonly PullData _pullData;
    private readonly string googleApiKey;

    public CoffeeShopController(ICoffeeShopRepository repository, IConfiguration config, PullData pullData, IMapper mapper, FindMyCoffeeContext context)
    {
        _context = context;
        _config = config;
        _repository = repository;
        _pullData = pullData;
        _mapper = mapper;
        googleApiKey = _config["Settings:GooglePlacesApiKey"];
        GeoapifyApiKey = _config["Settings:GeoapifyApiKey"];
    }
    
    
    [HttpGet ("GetWebShopInfo")]
    public ActionResult<IEnumerable<CoffeeShopReadDto>> GetWebShopInfo()
    {
        var coffeeShops = _repository.GetWebShopInfo();

        return Ok(_mapper.Map<IEnumerable<CoffeeShopReadDto>>(coffeeShops));
    }


    /* Get a specific coffeeshop by its ID */
    [HttpGet("{id}", Name = "GetShopById")]
    public ActionResult<CoffeeShopReadDto> GetShopById(int id)
    {
        var coffeeShop = _repository.GetShopById(id);
        if (coffeeShop == null) return BadRequest($"CoffeeShop with ID \"{id}\" was not found!");//We can also use NotFound();

        else return Ok(_mapper.Map<CoffeeShopReadDto>(coffeeShop));
    }

    /* Get a specific coffeeshop by its name */
    [HttpGet("{name}", Name = "GetShopByName")]
    public ActionResult<CoffeeShopReadDto> GetShopByName(string name)
    {
        CoffeeShopEntity coffeeShop = _repository.GetShopByName(name);
        if (coffeeShop == null) return NotFound($"Coffee shop named \"{name}\" doesn't exist.");
        else return Ok($"Coffee shop named \"{name}\" was found!");
    }

    /* Get all the shops from the database */
    [HttpGet("GetShops")]
    public ActionResult<IEnumerable<CoffeeShopReadDto>> GetShops()
    {
        IEnumerable<CoffeeShopEntity> shops = _repository.GetWebShopInfo();

        List<CoffeeShopReadDto> viewedShops = new List<CoffeeShopReadDto>();
        foreach (CoffeeShopEntity shop in shops)
        {
            viewedShops.Add(_mapper.Map<CoffeeShopReadDto>(shop));
        }

        return Ok(viewedShops);
    }

    //PUT api/CoffeeShop/{id}
    [HttpPut("{id}")]
    public ActionResult UpdateCoffeeShop(int id, CoffeeShopUpdateDto shopUpdateDto)
    {
        var shopModelFromRepo = _repository.GetShopById(id);
        if (shopModelFromRepo == null) { return NotFound(); }

        _mapper.Map(shopUpdateDto, shopModelFromRepo);

        _repository.UpdateCoffeeShop(shopModelFromRepo);//Don't do anything, But maybe other implementations will require that.

        _repository.AsyncSaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCoffeeShop(int id)
    {
        var shopModelFromRepo = _repository.GetShopById(id);
        if (shopModelFromRepo == null) { return NotFound(); }
        else
        {
            _repository.DeleteCoffeeShop(shopModelFromRepo);

            _repository.AsyncSaveChanges();
            return Ok($"Coffee shop \"{shopModelFromRepo.BusinessName}\" got deleted!");
        }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    [Authorize(Roles = "Business")] // Only allow access to this endpoint if the user is authenticated -> meaning the user have a valid authentication cookie (or JWT token).
    [HttpPost("CreateShop")]
    public async Task<ActionResult<CoffeeShopReadDto>> CreateShop(CoffeeShopViewModel coffeeshop)
    {
        //The user sends JSON from the frontend -> Checks if everything is valid
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var coffeeModel = _mapper.Map<CoffeeShopEntity>(coffeeshop);

        /* Tries to get the user ID from NameIdentifier.
        Returns the id value like "5", "17" of the user */
        var ownerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(ownerIdClaim)) return Unauthorized();


        coffeeModel.OwnerId = ownerIdClaim;

        coffeeModel.Email = "--";


        var locationResult = await FindByAddress(coffeeshop.Street,coffeeshop.City, coffeeshop.Country);

        var okResult = locationResult as OkObjectResult;
        if (okResult is not null && okResult.Value is not null)
        {
            // FindByAddress returns: { lat, lng }
            var json = JsonSerializer.Serialize(okResult.Value);
            var loc = JsonSerializer.Deserialize<Location>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loc is null) return BadRequest("Failed to parse geocoding result.");

            coffeeModel.Latitude = loc.lat;
            coffeeModel.Longitude = loc.lng;


        }
        else if (locationResult is BadRequestObjectResult bad)
        {
            // keep the *real* reason from FindByAddress
            return BadRequest(bad.Value);
        }
        else if (locationResult is ObjectResult obj)
        {
            // other status codes (404, 500, etc.)
            return StatusCode(obj.StatusCode ?? 500, obj.Value);
        }
        else
        {
            return StatusCode(500, "Unexpected geocoding error.");
        }

        _repository.CreateCoffeeShop(coffeeModel);
        await _repository.AsyncSaveChanges();


        int userId;
        //If ownerIdClaim is valid -> then isNumber = true and the value userId gets filled with the value of ownerIdClaim (the user id)
        bool isNumber = int.TryParse(ownerIdClaim, out userId);
        if (isNumber == false)
            return StatusCode(500, "Invalid user id in token.");

        //Creates a link for UserCoffeeShops table with a new row includes the UserId and CoffeeShopId
        var link = new UserCoffeeShopsEntity
        {
            UserId = userId,
            CoffeeShopId = coffeeModel.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserCoffeeShops.Add(link);
        await _context.SaveChangesAsync();


        var coffeeReadDto = _mapper.Map<CoffeeShopReadDto>(coffeeModel);
        return Created("/api/CoffeeShop", coffeeReadDto);
    }


    //GeoapifyResponse → Features → Geometry → Coordinates
    public class GeoapifyResponse { public List<GeoapifyFeature> Features { get; set; } } 
    public class GeoapifyFeature { public GeoapifyGeometry Geometry { get; set; } } 
    public class GeoapifyGeometry { public List<double> Coordinates { get; set; } } // [lon, lat]

    /* 
     * Find longitude and latitude of a given address.
     * This is the way a Business user can create a new business with a given location.
     * Most people don't know longitude and latitude
     */
    [HttpGet]
    public async Task<IActionResult> FindByAddress(string streetName, string cityName, string country, string? state = null)
    {
        // Clean inputs (remove extra spaces so we don't send messy data)
        streetName = streetName?.Trim() ?? "";
        cityName = cityName?.Trim() ?? "";
        country = country?.Trim() ?? "";
        state = state?.Trim();

        // Basic validation to block empty / obviously bad input
        var validationError = AddressValidation.ValidateAddressInput(streetName, cityName, country, state);
        if (validationError != null)
            return BadRequest(new { message = validationError });

        // Build the address query string (state is optional)
        var query = $"{streetName}, {cityName}";
        if (!string.IsNullOrWhiteSpace(state))
            query += $", {state}";

        query += $", {country}";


        // Call Geoapify API
        using var client = new HttpClient { BaseAddress = new Uri("https://api.geoapify.com/") };
        var url = $"v1/geocode/search?text={Uri.EscapeDataString(query)}&limit=1&lang=en&apiKey={GeoapifyApiKey}";

        var resp = await client.GetAsync(url);
        var body = await resp.Content.ReadAsStringAsync();

        // If Geoapify failed, return the error
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode, new { message = "Geocoding call failed", upstream = body });

        // Parse the response JSON
        var data = JsonSerializer.Deserialize<GeoapifyResponse>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Try to extract latitude and longitude from the first result
        var coords = data?.Features?.FirstOrDefault()?.Geometry?.Coordinates;

        if (coords != null && coords.Count >= 2)
            // GeoJSON order is [longitude, latitude]
            return Ok(new { lat = coords[1], lng = coords[0] }); // GeoJSON = [lon, lat]

        // No valid location found
        return BadRequest(new { message = "No results" });
    }


    [HttpPost("FindShortestDistanceDB")]
    public async Task<IActionResult> FindShortestDistanceDB(double userLat1, double userLng1)
    {
        var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

        try
        {
            var closest = coffeeshops.OrderBy(shop => Formulas.Haversine(userLat1, userLng1, shop.Latitude, shop.Longitude)).First();
            var distance = (int)Formulas.Haversine(userLat1, userLng1, closest.Latitude, closest.Longitude) + " km";
            return Ok(new { closest.BusinessName, closest.Vicinity, distance });
        }
        catch
        {
            return BadRequest("Something is wrong with the database, no stores were found!");
        }
    }


    /*
     *  Find coffeeshops by distance from the user's current location.
     */

    //[FromBody] basically tells the API to look into the request body for the data for the parameter "ClosestRequest" which looks for 3 specific attributes:
    //Amount, UserLat, UserLng.
    public class ClosestRequest
    {
        public int Amount { get; set; }
        public double UserLat { get; set; }
        public double UserLng { get; set; }
    }

    [HttpPost("FindClosestCoffeeshops")]
    public async Task<IActionResult> FindClosestCoffeeshops([FromBody] ClosestRequest data)
    {
        Console.WriteLine($"Received from client: Lat={data.UserLat}, Lng={data.UserLng}, Amount={data.Amount}");

        /*
         * List the shops as "only read" object. By default, queries that return entity types are tracking.
         * Turns off the query's tracking, means any changes to entity instances are not being tracked and saved by SaveChanges.
         */
        var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

        try
        {
            var closest = coffeeshops
                .OrderBy(shop => Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude))
                .Take(data.Amount)
                .Select(shop => new
                {
                    shop.BusinessName,
                    shop.State,
                    shop.City,
                    shop.Street,
                    shop.Title,
                    shop.Vicinity,
                    shop.PriceLevel,
                    shop.Rating,
                    shop.TotalUserRating,
                    shop.Id,
                    distanceKm = Math.Round(Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude))
                })
                .ToList();

            return Ok(closest);
        }
        catch
        {
            return BadRequest("Something is wrong with the database, no stores were found!");
        }
    }


    /*
     * Find shops by rating. 
     * Finds all the shops between the given minRating and maxRating.
     * In the future, also given distance\radius will be added
     */
    public class RatingRequest
    {
        public double MinRating { get; set; }
        public double MaxRating { get; set; }
        public double UserLat { get; set; }
        public double UserLng { get; set; }
        public string DistanceRanage { get; set; }
    }
    [HttpPost("FindByRating")]
    public async Task<IActionResult> FindByRating([FromBody] RatingRequest data, int Amount = 10)
    {
        // Fetch all coffee shops from the database without tracking
        var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

        Console.WriteLine($"Received from client: Lat={data.UserLat}, Lng={data.UserLng}, Amount={Amount}");

        // Resolve the numeric distance bounds (min/max in kilometers)
        int rangeMin, rangeMax;
        Console.WriteLine(data.DistanceRanage);

        (rangeMin, rangeMax) = DistanceRangeService.GetRange(data.DistanceRanage);
    
        Console.WriteLine("RangeMin:" + rangeMin);
        Console.WriteLine("RangeMax:" + rangeMax);

        // Filter by type and distance, then sort the results and select the needed data
        try
        {
            var listOfShopsInGivenRate = coffeeshops
                .Where(shop => shop.Rating >= data.MinRating && shop.Rating <= data.MaxRating
                    && Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude) >= rangeMin && Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude) <= rangeMax)
                .OrderByDescending(shop => shop.Rating)
                .Take(Amount)
                .Select(shop => new
                {
                    shop.BusinessName,
                    shop.State,
                    shop.City,
                    shop.Street,
                    shop.Title,
                    shop.Vicinity,
                    shop.PriceLevel,
                    shop.Rating,
                    shop.TotalUserRating,
                    shop.Id,
                    distanceKm = Math.Round(Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude))
                })
                 .Pipe(shop => Console.WriteLine($"{shop.BusinessName}: {shop.distanceKm} km"))
                .ToList();

            return Ok(listOfShopsInGivenRate);
        }
        catch
        {
            return BadRequest("Something is wrong with the database, no stores were found!");
        }
    }

    public class NameRequest {
        public string Name{ set; get; }
        public double UserLat { set; get; }
        public double UserLng { set; get; }
    }

    [HttpPost("GetShopsByName")]
    public async Task<IActionResult> GetShopsByName(NameRequest request)
    {

        try
        {
            // Fetch all coffee shops from the database without tracking
            var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

            try
            {
                var listOfShopsWithGivenName = coffeeshops
                    .Where(shop => shop.BusinessName.ToUpper() == request.Name.ToUpper())
                    .OrderByDescending(shop => Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude))
                    .Select( shop => new
                    {
                        shop.BusinessName,
                        shop.State,
                        shop.City,
                        shop.Street,
                        shop.Title,
                        shop.Vicinity,
                        shop.PriceLevel,
                        shop.Rating,
                        shop.TotalUserRating,
                        shop.Id,
                        distanceKm = Math.Round(Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude))
                    })
                    .ToList();
                    
                return Ok(listOfShopsWithGivenName);
            }
            catch
            {
                return BadRequest("No shops were found!");
            }
        }
        catch
        {
            return BadRequest("Error: something went wrong with the database!");
        }
    }


    public class TypeRequest
    {
        public string Type { set; get; }
        public double UserLat { set; get; }
        public double UserLng { set; get; }
        public string DistanceRanage { get; set; }

    }

    [HttpPost("GetShopsByType")]
    public async Task<IActionResult> GetShopsByType(TypeRequest request)
    {
        try
        {
            // Fetch all coffee shops from the database without tracking
            var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

            int rangeMin, rangeMax;
            Console.WriteLine(request.DistanceRanage);

            // Resolve the numeric distance bounds (min/max in kilometers)
            (rangeMin, rangeMax) = DistanceRangeService.GetRange(request.DistanceRanage);

            Console.WriteLine("RangeMin:" + rangeMin);
            Console.WriteLine("RangeMax:" + rangeMax);

            // Filter by type and distance, then sort the results and select the needed data
            try
            {
                var coffeeshopListWithGivenType = coffeeshops
                .Where(shop => shop.Type.ToUpper() == request.Type.ToUpper()
                && ( Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude) >= rangeMin 
                && (Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude) <= rangeMax)))
                .OrderByDescending(shop => Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude))
                .Select(shop => new
                {
                    shop.BusinessName,
                    shop.State,
                    shop.City,
                    shop.Street,
                    shop.Title,
                    shop.Vicinity,
                    shop.PriceLevel,
                    shop.Rating,
                    shop.TotalUserRating,
                    shop.Id,
                    distanceKm = Math.Round(Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude))
                })
                .ToList();

                return Ok(coffeeshopListWithGivenType);
            }
            catch
            {
                Console.WriteLine($"Error: no shops with type {request.Type} exist in the given distance!");
                return BadRequest($"Error: no shops with type {request.Type} exist in the given distance!");
            }
        }
        catch
        {
            Console.WriteLine("Error: Did not manage to access the database!");
            return BadRequest("Error: Did not manage to access the database!");
        }

    }


    /* 
     * Rating mechanism.
     * The current implementation is a temporary one because it doesn't prevent spam or repeated votes by the same user.
     * Will be fixed on the next version.
     */
    public class RateRequest
    {
        public int Id { set; get; }
        public int UserRate { set; get; }
    }
    [HttpPost("Rate")]
    public async Task<IActionResult> RateCoffeeshop([FromBody]RateRequest request)
    {
        //Prevents bad input.
        if (request.UserRate < 1 || request.UserRate > 5)
            return BadRequest("Rating must be between 1 and 5.");

        var coffeeshop = await _context.CoffeeShops.FirstOrDefaultAsync(s => s.Id == request.Id);
        if (coffeeshop == null)
            return NotFound();

        coffeeshop.Rating = Formulas.CalculateNewAverage(coffeeshop.Rating, coffeeshop.TotalUserRating, request.UserRate);
        coffeeshop.TotalUserRating = Formulas.NewCount(coffeeshop.TotalUserRating);

        await _context.SaveChangesAsync();

        return Ok(new { coffeeshop.Id, coffeeshop.Rating, coffeeshop.TotalUserRating });
    }


    //[HttpPatch("{id}")]
    //public ActionResult PartialCoffeeShopUpdate(int id, JsonPatchDocument<CoffeeShopUpdateDto> patchDoc)
    //{
    //    var shopModelFromRepo = _repository.GetShopById(id);
    //    if (shopModelFromRepo == null) { return NotFound(); }
    //    else
    //    {
    //        var shopToPatch = _mapper.Map<CoffeeShopUpdateDto>(shopModelFromRepo);
    //        patchDoc.ApplyTo(shopToPatch, ModelState);
    //        if (!TryValidateModel(patchDoc))
    //        {
    //            return ValidationProblem(ModelState);
    //        }

    //        _mapper.Map(shopToPatch, shopModelFromRepo);

    //        _repository.UpdateCoffeeShop(shopModelFromRepo);

    //        _repository.AsyncSaveChanges();

    //        return NoContent();
    //    }
    //}







    /* -------------------------------------- Google services (Unavailable right now) --------------------------------------*/
    //[HttpGet("GoogleApiShops")]
    //public async Task<string> Get()
    //{
    //    string apiKey = _config["GoogleApiKey"];
    //    string latitude = "32.08";
    //    string longitude = "34.78";
    //    string radius = "1500";
    //    string type = "cafe";

    //    //If I run this code 3 times (in 3 seconds delay for each run) I can get 60 results instead of 20
    //    string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&radius={radius}&type={type}&key={apiKey}";

    //    using var client = new HttpClient();
    //    var response = await client.GetAsync(url);
    //    string json = await response.Content.ReadAsStringAsync();

    //    return json;
    //}

    //[HttpPost("GoogleData")]
    //public async Task<IActionResult> PostGoogleData()
    //{
    //    var results = await _pullData.PullGoogleInfo();
    //    if (results == null) { return BadRequest(); }

    //    await _pullData.StoreInDB(results);
    //    return Ok();
    //}
    /*
 *  Purpose: I will get lat and lng of the user. With HTTP request, I will pull the city where the lat and lng belong to.
 *  Then I'll find the closest coffee shop in the specific city. (I will use it in order to reduce the amount of searches).
 */
    //[HttpGet("FindByGeometry")]
    //public async Task<IActionResult> FindByGeometry(double lat, double lng)
    //{

    //    using (HttpClient client = new HttpClient())
    //    {
    //        client.BaseAddress = new Uri("https://maps.googleapis.com");

    //        var response = await client.GetStringAsync($"/maps/api/geocode/json?latlng={lat}, {lng}&key={apiKey}");

    //        try
    //        {
    //            return Ok(response);
    //        }
    //        catch
    //        {
    //            return BadRequest("Could not fetch data");
    //        }
    //    }

    //}

    //[HttpPost("DistanceCalculationGoogle")]
    //public async Task<IActionResult> DistanceBetweenCoffeeshopsGoogle(bool drive, double lat1, double lng1, double lat2, double lng2)
    //{

    //    using (HttpClient client = new HttpClient())
    //    {

    //        client.BaseAddress = new Uri("https://maps.googleapis.com");

    //        string action = drive ? "driving" : "walking";

    //        try
    //        {
    //            var url = $"/maps/api/distancematrix/json" +
    //            $"?origins={lat1},{lng1}" +
    //            $"&destinations={lat2},{lng2}" +
    //            $"&mode={action}" +
    //            $"&units=metric" +
    //            $"&key={apiKey}";

    //            var http = await client.GetAsync(url);

    //            var body = await http.Content.ReadAsStringAsync();

    //            if (!http.IsSuccessStatusCode)
    //                return StatusCode((int)http.StatusCode, new { error = "HTTP error from Google", status = http.StatusCode, body });

    //            var dto = JsonSerializer.Deserialize<DistanceMatrixResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    //            return Ok(dto);
    //        }
    //        catch
    //        {
    //            return BadRequest("Could not fetch data");

    //        }
    //    }

    //}

    /*
     *  In another method we will iterate over all the coffee shops and apply this method on each of them.
     *  This will return the distance of the specific coffeeshop from the user's spot given from the frontend.
     */

    //[HttpPost("closest")]
    //public async Task<IActionResult> FindCoffeeShops()
    //{

    //    var body = new
    //    {
    //        origins = new[]
    //        {
    //        new {
    //            waypoint = new {
    //                location = new {
    //                    latLng = new { latitude = 32.0832358, longitude = 34.78929520000001 }
    //                }
    //            }
    //        }
    //    },
    //        destinations = new[]
    //   {
    //        new {
    //            waypoint = new {
    //                placeId = "ChIJswI6zYZLHRURvn2cn5uxmKk"  // <-- example; use your own
    //            }
    //        }
    //        // add more destinations as needed
    //    },
    //        travelMode = "DRIVE"
    //    };

    //    var json = JsonSerializer.Serialize(body);
    //    using HttpClient client = new HttpClient();
    //    using var request = new HttpRequestMessage(
    //        HttpMethod.Post,
    //        $"https://routes.googleapis.com/distanceMatrix/v2:computeRouteMatrix?key={apiKey}"
    //    );

    //    request.Headers.Add("X-Goog-FieldMask",
    //    "originIndex,destinationIndex,distanceMeters,duration,status");

    //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

    //    var response = await client.SendAsync(request);

    //    var content = new StringContent(json, Encoding.UTF8, "application/json");

    //    var result = await response.Content.ReadAsStringAsync();

    //    if (response.IsSuccessStatusCode)
    //    {
    //        return Content(result, "application/json"); // or return Ok(JsonDocument.Parse(result));
    //    }
    //    else
    //    {
    //        return StatusCode((int)response.StatusCode, result);

    //    }



    //}
}
