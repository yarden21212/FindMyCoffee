using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FindMyCoffee.Services.Distance
{
    public interface IDistanceRangeService
    {
        public abstract static (int rangeMin, int rangeMax) GetRange(string distanceRange);
    }
}
