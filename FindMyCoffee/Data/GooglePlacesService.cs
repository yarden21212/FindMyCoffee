using System.Text.Json;

namespace FindMyCoffee.Data
{
    public class GooglePlacesService
    {
        private readonly IConfiguration _config;

        public GooglePlacesService(IConfiguration config)
        {
            _config = config;
        }
    }
}
