using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FindMyCoffee.Services.Calculations
{
    public class Formulas
    {
        private readonly IConfiguration _config;
        private string _key;

        /* Constructor that injects configuration settings and loads the Google Places API key 
            for distance and location-related calculations. */
        public Formulas(IConfiguration configuration)
        {
            _config = configuration;
            _key = configuration["GooglePlacesApiKey"];
        }

        /* Calculates the distance (in kilometers) between two geographic points using the Haversine formula. */
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

        /*
         * Converts an angle from degrees to radians.
         * Degrees are a way of measuring direction on a circle (e.g., 90°, 45°, 180°).
         */
        private static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }

        /*
         * Rating logic 
         * This first version does not store individual user ratings, it only supports adding new ratings and updating the average.
         */

        // Calculates the new rating's average, after a user rated a coffeeshop
        public static double CalculateNewAverage(double oldAvg, int oldRatingCount, int newUserRate)
        {
            decimal val =  decimal.Round(((decimal)(oldAvg * oldRatingCount + newUserRate) / (oldRatingCount + 1)), 2, MidpointRounding.AwayFromZero);
            return (double)val;
        }

        // Increase the given coffeeshop rating count by 1 (a new rating was given)
        public static int NewCount(int oldCount)
        {
            return oldCount + 1;
        }


    }

}
