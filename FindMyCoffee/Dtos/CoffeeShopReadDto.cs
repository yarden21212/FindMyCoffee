using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindMyCoffee.Dots
{
    public class CoffeeShopReadDto
    {
        public string? BusinessName { get; set; }

        public string? Type { get; set; }

        public string? Title { get; set; }

        public double Rating { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsOpen { get; set; }

        public PriceLevel PriceLevel { get; set; }

        public int TotalUserRating { get; set; }

        public string? Vicinity { get; set; }

    }
}