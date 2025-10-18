using AutoMapper;
using FindMyCoffee.Data;
using FindMyCoffee.Data.API;
using FindMyCoffee.Data.HttpResponses;
using FindMyCoffee.Dots;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;
using FindMyCoffee.Services.Calculations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MoreLinq;

[Route("api/[controller]/[action]")]
[ApiController]

public class CoffeeShopController : ControllerBase
{
    //private readonly MockShopRepo _repository = new MockShopRepo();//Acts as a fake coffeeShop finder from a fake database
    private readonly IConfiguration _config;
    private readonly IFindMyCoffeeRepo _repository;
    private readonly PullData _pullData;
    private readonly IMapper _mapper;
    private readonly string key;
    private readonly FindMyCoffeeContext _context;

    public CoffeeShopController(IFindMyCoffeeRepo repository, IConfiguration config, PullData pullData, IMapper mapper, FindMyCoffeeContext context)
    {
        _context = context;
        _config = config;
        _repository = repository;
        _pullData = pullData;
        _mapper = mapper;
        key = _config["Settings:GooglePlacesApiKey"];
    }

    [HttpGet("GoogleApiShops")]
    public async Task<string> Get()
    {
        string apiKey = _config["GoogleApiKey"];
        string latitude = "32.08";
        string longitude = "34.78";
        string radius = "1500";
        string type = "cafe";

        //If I run this code 3 times (in 3 seconds delay for each run) I can get 60 results instead of 20
        string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&radius={radius}&type={type}&key={apiKey}";

        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();

        return json;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoffeeShopReadDto>> GetWebShopInfo()
    {
        var coffeeShops = _repository.GetWebShopInfo();

        return Ok(_mapper.Map<IEnumerable<CoffeeShopReadDto>>(coffeeShops));
    }

    [HttpGet("{id}", Name = "GetShopById")]
    public ActionResult<CoffeeShopReadDto> GetShopById(int id)
    {
        var coffeeShop = _repository.GetShopById(id);
        if (coffeeShop == null) return BadRequest($"CoffeeShop with ID \"{id}\" was not found!");//We can also use NotFound();

        else return Ok(_mapper.Map<CoffeeShopReadDto>(coffeeShop));
    }

    [HttpGet("{name}", Name = "GetShopByName")]
    public ActionResult<CoffeeShopReadDto> GetShopByName(string name)
    {
        CoffeeShopEntity coffeeShop = _repository.GetShopByName(name);
        if (coffeeShop == null) return NotFound($"Coffee shop named \"{name}\" doesn't exist.");
        else return Ok($"Coffee shop named \"{name}\" was found!");
    }

    [HttpGet]
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

    [HttpPost("GoogleData")]
    public async Task<IActionResult> PostGoogleData()
    {
        var results = await _pullData.PullGoogleInfo();
        if (results == null) { return BadRequest(); }

        await _pullData.StoreInDB(results);
        return Ok();
    }

    [HttpPost]
    public ActionResult<CoffeeShopReadDto> CreateShop(CoffeeShopCreateDto shop)
    {
        var coffeeModel = _mapper.Map<CoffeeShopEntity>(shop);
        _repository.CreateCoffeeShop(coffeeModel);
        _repository.SaveChanges();

        var coffeeReadDto = _mapper.Map<CoffeeShopReadDto>(coffeeModel);

        return CreatedAtRoute(nameof(GetShopByName), new { Name = coffeeReadDto.Name }, coffeeReadDto);
        //return Ok(coffeeReadDto);
    }

    //PUT api/CoffeeShop/{id}
    [HttpPut("{id}")]
    public ActionResult UpdateCoffeeShop(int id, CoffeeShopUpdateDto shopUpdateDto)
    {
        var shopModelFromRepo = _repository.GetShopById(id);
        if (shopModelFromRepo == null) { return NotFound(); }

        _mapper.Map(shopUpdateDto, shopModelFromRepo);

        _repository.UpdateCoffeeShop(shopModelFromRepo);//Don't do anything, But maybe other implementations will require that.

        _repository.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public ActionResult PartialCoffeeShopUpdate(int id, JsonPatchDocument<CoffeeShopUpdateDto> patchDoc)
    {
        var shopModelFromRepo = _repository.GetShopById(id);
        if (shopModelFromRepo == null) { return NotFound(); }
        else
        {
            var shopToPatch = _mapper.Map<CoffeeShopUpdateDto>(shopModelFromRepo);
            patchDoc.ApplyTo(shopToPatch, ModelState);
            if (!TryValidateModel(patchDoc))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(shopToPatch, shopModelFromRepo);

            _repository.UpdateCoffeeShop(shopModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCoffeeShop(int id)
    {
        var shopModelFromRepo = _repository.GetShopById(id);
        if (shopModelFromRepo == null) { return NotFound(); }
        else
        {
            _repository.DeleteCoffeeShop(shopModelFromRepo);

            _repository.SaveChanges();
            return Ok($"Coffee shop \"{shopModelFromRepo.Name}\" got deleted!");
        }
    }



    [HttpPost("closest")]
    public async Task<IActionResult> FindCoffeeShops()
    {

        var body = new
        {
            origins = new[]
            {
            new {
                waypoint = new {
                    location = new {
                        latLng = new { latitude = 32.0832358, longitude = 34.78929520000001 }
                    }
                }
            }
        },
            destinations = new[]
       {
            new {
                waypoint = new {
                    placeId = "ChIJswI6zYZLHRURvn2cn5uxmKk"  // <-- example; use your own
                }
            }
            // add more destinations as needed
        },
            travelMode = "DRIVE"
        };

        var json = JsonSerializer.Serialize(body);
        using HttpClient client = new HttpClient();
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://routes.googleapis.com/distanceMatrix/v2:computeRouteMatrix?key={key}"
        );

        request.Headers.Add("X-Goog-FieldMask",
        "originIndex,destinationIndex,distanceMeters,duration,status");

        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var result = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return Content(result, "application/json"); // or return Ok(JsonDocument.Parse(result));
        }
        else
        {
            return StatusCode((int)response.StatusCode, result);

        }



    }


    [HttpGet("FindByAddress")]
    public async Task<IActionResult> FindByAddress(string streetName, string apartmentNumber, string cityName, string state)
    {

        string apiKey = key;

        using (HttpClient client = new HttpClient())
        {

            client.BaseAddress = new Uri("https://maps.googleapis.com/");

            var address = $"{streetName} {apartmentNumber}, {cityName}, {state}";
            var encoded = Uri.EscapeDataString(address);
            string endPoint = $"maps/api/geocode/json?" +
                $"address={encoded}" +
                $"&key={apiKey}";

            //string endPoint = $"maps/api/geocode/json?" +
            //    $"address=Jerusalem Forest st 10,+Modiin,+Israel" +
            //    $"&key={apiKey}";
            var response = await client.GetAsync(endPoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);

                var data = JsonSerializer.Deserialize<GeocodingResponse>(json);

                var first = data?.Results?.FirstOrDefault();
                var lat = first.Geometry?.Location?.Lat;
                var lng = first.Geometry?.Location.Lng;

                if (data.Status != "OK")
                {
                    return BadRequest(new { message = "Geocoding failed", status = data?.Status });
                }
                if (lat == null || lng == null)
                {
                    return NotFound("Given location was not found");
                }
                else
                    return Ok(new { lat, lng });

            }


            return BadRequest();
        }
    }

    /*
     *  Purpose: I will get lat and lng of the user. With HTTP request, I will pull the city where the lat and lng belong to.
     *  Then I'll find the closest coffee shop in the specific city. (I will use it in order to reduce the amount of searches).
     */
    [HttpGet("FindByGeometry")]
    public async Task<IActionResult> FindByGeometry(double lat, double lng)
    {

        string apiKey = key;

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://maps.googleapis.com");

            var response = await client.GetStringAsync($"/maps/api/geocode/json?latlng={lat}, {lng}&key={apiKey}");

            try
            {
                return Ok(response);
            }
            catch
            {
                return BadRequest("Could not fetch data");
            }
        }

    }

    [HttpPost("DistanceCalculationGoogle")]
    public async Task<IActionResult> DistanceBetweenCoffeeshopsGoogle(bool drive, double lat1, double lng1, double lat2, double lng2)
    {

        using (HttpClient client = new HttpClient())
        {

            client.BaseAddress = new Uri("https://maps.googleapis.com");

            string action = drive ? "driving" : "walking";

            try
            {
                var url = $"/maps/api/distancematrix/json" +
                $"?origins={lat1},{lng1}" +
                $"&destinations={lat2},{lng2}" +
                $"&mode={action}" +
                $"&units=metric" +
                $"&key={key}";

                var http = await client.GetAsync(url);

                var body = await http.Content.ReadAsStringAsync();

                if (!http.IsSuccessStatusCode)
                    return StatusCode((int)http.StatusCode, new { error = "HTTP error from Google", status = http.StatusCode, body });

                var dto = JsonSerializer.Deserialize<DistanceMatrixResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Ok(dto);
            }
            catch
            {
                return BadRequest("Could not fetch data");

            }
        }

    }

    /*
     *  In another method we will iterate over all the coffee shops and apply this method on each of them.
     *  This will return the distance of the specific coffeeshop from the user's spot given from the frontend.
     */
    [HttpPost("FindShortestDistanceDB")]
    public async Task<IActionResult> FindShortestDistanceDB(double userLat1, double userLng1)
    {
        var coffeeshops = _context.CoffeeShops.AsNoTracking().ToList();

        try
        {
            var closest = coffeeshops.OrderBy(shop => Formulas.Haversine(userLat1, userLng1, shop.Latitude, shop.Longitude)).First();
            var distance = (int)Formulas.Haversine(userLat1, userLng1, closest.Latitude, closest.Longitude) + " km";
            return Ok(new { closest.Name, closest.Vicinity, distance });
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
    [HttpPost]
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
                    shop.Name,
                    shop.Title,
                    shop.Vicinity,
                    shop.PriceLevel,
                    shop.Rating,
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
    [HttpPost]
    public async Task<IActionResult> FindByRating([FromBody] RatingRequest data, int Amount = 10)
    {
        var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

        Console.WriteLine($"Received from client: Lat={data.UserLat}, Lng={data.UserLng}, Amount={Amount}");

        int rangeMin = 0;
        int rangeMax = 0;
        Console.WriteLine(data.DistanceRanage);

        switch (data.DistanceRanage)
        {
            case "closest":
                {
                    rangeMin = 0;
                    rangeMax = 5;
                    break;
                }
            case "close":
                {
                    rangeMin = 5;
                    rangeMax = 10;
                    break;
                }
            case "mid":
                {
                    rangeMin = 10;
                    rangeMax = 30;
                    break;
                }
            case "far":
                {
                    rangeMin = 30;
                    rangeMax = 60;
                    break;
                }
            default:
                {
                    rangeMin = 0;
                    rangeMax = 1000000;
                    break;
                }
        }

        Console.WriteLine("RangeMin:" + rangeMin);
        Console.WriteLine("RangeMax:" + rangeMax);

        try
        {
            var listOfShopsInGivenRate = coffeeshops
                .Where(shop => shop.Rating >= data.MinRating && shop.Rating <= data.MaxRating
                    && Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude) >= rangeMin && Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude) <= rangeMax)
                .OrderByDescending(shop => shop.Rating)
                .Take(Amount)
                .Select(shop => new
                {
                    shop.Name,
                    shop.Title,
                    shop.Vicinity,
                    shop.PriceLevel,
                    shop.Rating,
                    distanceKm = Math.Round(Formulas.Haversine(data.UserLat, data.UserLng, shop.Latitude, shop.Longitude))
                })
                 .Pipe(shop => Console.WriteLine($"{shop.Name}: {shop.distanceKm} km"))
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
            var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

            try
            {
                var listOfShopsWithGivenName = coffeeshops
                    .Where(shop => shop.Name.ToUpper() == request.Name.ToUpper())
                    .OrderByDescending(shop => Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude))
                    .Select( shop => new
                    {
                        shop.Name,
                        shop.Title,
                        shop.Vicinity,
                        shop.PriceLevel,
                        shop.Rating,
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
            var coffeeshops = await _context.CoffeeShops.AsNoTracking().ToListAsync();

            int rangeMin = 0;
            int rangeMax = 0;
            Console.WriteLine(request.DistanceRanage);

            switch (request.DistanceRanage)
            {
                case "closest":
                    {
                        rangeMin = 0;
                        rangeMax = 5;
                        break;
                    }
                case "close":
                    {
                        rangeMin = 5;
                        rangeMax = 10;
                        break;
                    }
                case "mid":
                    {
                        rangeMin = 10;
                        rangeMax = 30;
                        break;
                    }
                case "far":
                    {
                        rangeMin = 30;
                        rangeMax = 60;
                        break;
                    }
                default:
                    {
                        rangeMin = 0;
                        rangeMax = 1000000;
                        break;
                    }
            }

            Console.WriteLine("RangeMin:" + rangeMin);
            Console.WriteLine("RangeMax:" + rangeMax);

            try
            {
                var coffeeshopListWithGivenType = coffeeshops
                .Where(shop => shop.Type.ToUpper() == request.Type.ToUpper())
                .OrderByDescending(shop => Formulas.Haversine(request.UserLat, request.UserLng, shop.Latitude, shop.Longitude))
                .Select(shop => new
                {
                    shop.Name,
                    shop.Title,
                    shop.Vicinity,
                    shop.PriceLevel,
                    shop.Rating,
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
}
        