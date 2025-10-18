using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindMyCoffee.Models
{
    public class CoffeeShopEntity
    {
        string[] types = {"Concept", "Bakery", "Coffee Truck", "Drive-thru", "Italian" , "French", "Pub" };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(15)]
        public int Id { get; set; }

        [Required]
        [MaxLength(70)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Type { get; set; }

        [MaxLength(250)]
        public string? Title { get; set; }

        public double Rating { get; set; }

        public double Latitude { get; set; }//how far north/south

        public double Longitude { get; set; }// how far east/west

        public bool IsOpen { get; set; }

        public string? PhotoReference { get; set; }

        public string? PhotoAttribute { get; set; }

        public string? PhotoUrl { get; set; }

        public string? PlaceId { get; set; }

        [Required]
        public long PriceLevel { get; set; }

        public int TotalUserRating { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Vicinity { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? OwnerID {  get; set; }




        public string ToString()
        {
            string attributes = "";
            attributes = $"ID: {Id}\nName: {Name}\nType: {Type}\nTitle: {Title}\nRating: {Rating}";

            return attributes;
        }

    }

}