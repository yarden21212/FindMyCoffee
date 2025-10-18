using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FindMyCoffee.Services.Calculations
{
    public class Formulas
    {
        private readonly IConfiguration _config;
        private string key;
        public Formulas(IConfiguration configuration) 
        {
            _config = configuration;
            key = configuration["GooglePlacesApiKey"];
        }
        public static double Haversine(double lat1, double lng1, double lat2, double lng2)
        {

            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lng2 - lng1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        private static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }


    }

}
