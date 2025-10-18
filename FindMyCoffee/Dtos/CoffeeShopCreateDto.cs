using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindMyCoffee.Dtos
{
    public class CoffeeShopCreateDto
    {
        [Required]
        [MaxLength(70)]
        public string? Name { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public long? PriceLevel { get; set; }

        [Required]
        public string? Vicinity { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? OwnerID { get; set; }
    }
}
