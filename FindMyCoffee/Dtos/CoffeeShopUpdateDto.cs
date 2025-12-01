using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.Dtos
{
    public class CoffeeShopUpdateDto
    {
        [Required]
        [MaxLength(70)]
        public string? BusinessName { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public PriceLevel? PriceLevel { get; set; }

        [Required]
        public string? Vicinity { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? OwnerID { get; set; }

    }
}
