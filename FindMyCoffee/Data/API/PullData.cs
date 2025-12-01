using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FindMyCoffee.Data.API
{
    public class PullData
    {
        private readonly FindMyCoffeeContext context;
        private readonly IConfiguration configuration;
        public PullData(FindMyCoffeeContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<PlacesResults?> PullGoogleInfo()
        {
            string endpoint = configuration["Settings:GoogleLegacyEndpoint"];
            double lat = 32.08530;
            double lng = 34.78177;
            int radius = 1500;
            string type = "cafe";
            string apiKey = configuration["GoogleApiKey"];

            string url = endpoint + $"/json?location={lat},{lng}&radius={radius}&type={type}&key={apiKey}";

            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);

            var results = JsonSerializer.Deserialize<PlacesResults>(response);
            return results; //Needs to be fixed. What happens when result = null?
        }

        public async Task StoreInDB(PlacesResults apiResults)
        {
            if (apiResults == null || apiResults.Result == null || apiResults.Result.Count == 0) 
                return;
            
            List<CoffeeShopEntity> newShops = new List<CoffeeShopEntity>();

            foreach (var item in apiResults.Result)
            {
                if (item == null)
                    continue;

                var placeId = item.PlaceId?.Trim();
                if (string.IsNullOrEmpty(placeId)) continue;

                var exists = await context.CoffeeShops.AnyAsync(c => c.PlaceId == placeId);
                if (exists) continue;

                //results[0].photos[0].html_attributions  
                if (item.Photos == null)
                {
                    item.Photos = new List<PhotoInfo> { };
                    //item.Photos[0].HtmlAttributions = "N/A";
                    //cafe.PhotoAttribute = new List<PhotoInfo>();

                }
                if (item.Photos[0].HtmlAttributions == null)
                {
                    item.Photos[0].HtmlAttributions = new List<string>();
                }
                if (string.IsNullOrEmpty(item.Photos[0].HtmlAttributions[0]))
                {
                    item.Photos[0].HtmlAttributions = new List<string> { };
                }

                if (string.IsNullOrEmpty(item.PhotoReference))
                {
                    item.PhotoReference = "N/A";
                }

                if(item.OpeningHours == null)
                {
                    item.OpeningHours = new Activity();
                }

                CoffeeShopEntity cafe = new CoffeeShopEntity
                {
                    BusinessName = item.Name,
                    PriceLevel = (Domain.Enums.PriceLevel)item.PriceLevel,
                    Longitude = item.Geometry.Location.lng,
                    Latitude = item.Geometry.Location.lat,
                    Rating = item.Rating,
                    Email = item.Name + "@Gmail.com",
                    PlaceId = item.PlaceId,
                    Vicinity = item.Vicinity,
                    IsOpen = item.OpeningHours.IsOpen,
                    Type = item.Types[1],
                    TotalUserRating = item.TotalRating,
                    Title = "N/A",
                    PhotoAttribute = item.Photos[0].HtmlAttributions[0],
                    PhotoReference = item.PhotoReference,
                    OwnerId = "N/A"
                };
                newShops.Add(cafe);
            }

            if (newShops.Count() == 0) return;

            await context.CoffeeShops.AddRangeAsync(newShops);
            await context.SaveChangesAsync();
        }
    }
}
