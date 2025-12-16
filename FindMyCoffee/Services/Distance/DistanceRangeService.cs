using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FindMyCoffee.Services.Distance
{
    /* Less professional implementation*/
    //public class DistanceRangeService : IDistanceRangeService
    //{
    //    public (int rangeMin, int rangeMax) GetRange(string distanceRange)
    //    {
    //        int min, max;
    //        switch (distanceRange)
    //        {
    //            case "closest":
    //                {
    //                    min = 0; max = 5; 
    //                    break;
    //                }
    //            case "close":
    //                {
    //                    min = 5; max = 10;
    //                    break;
    //                }
    //            case "mid":
    //                {
    //                    min = 10; max = 30;
    //                    break;
    //                }
    //            case "far":
    //                {
    //                    min = 30; max = 60;
    //                    break;
    //                }
    //            default:
    //                {
    //                    min = 0; max = 1000000;
    //                    break;
    //                }
    //        }

    //        return (min, max);
    //    }
    //}


    /* Returns the distance range (min–max) for filtering coffee shops by user location. */
    public class DistanceRangeService : IDistanceRangeService
    {
        public static  (int rangeMin, int rangeMax) GetRange(string distanceRange)
        {
            return distanceRange switch
            {
                "closest" => (0, 5),
                "close" => (5, 10),
                "mid" => (10, 30),
                "far" => (30, 60),
                _ => (0, 1_000_000)
            };
        }
    }
}
