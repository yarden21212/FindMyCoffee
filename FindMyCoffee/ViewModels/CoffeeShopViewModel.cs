using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.ViewModels
{
    public class CoffeeShop
    {
        public int Id { get; set; }
        public int BusinessProfileId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Latitude { get; set; }    // see numeric note below
        public decimal Longitude { get; set; }
        public string? GooglePlaceId { get; set; }
    }

    public class CoffeeShopViewModel
    {
        [Required]
        public string? BusinessName { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int PriceLevel { get; set; }
        [Required]
        public string? Street { get; set; }
        //[Required]
        //public string? Apartment { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Title { get; set; }
        public string? Vicinity { get; set; }

    }
}
