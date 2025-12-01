using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class CoffeeShopEntity
{
    [Key]
    public int Id { get; set; }

    /* ------------------------ Relationship ------------------------ */
    [Required]
    public string OwnerId { get; set; }  // Foreigner key → User.Id -> We need it because first you create a user and then a shop.

    //public ApplicationUser Owner { get; set; }  // Navigation (optional)

    /* ------------------------ Shop info ------------------------*/
    [Required]
    public string BusinessName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public PriceLevel PriceLevel { get; set; }

    public string State { get; set; }
    [Required]
    public string Country { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string Street { get; set; }

    public string? ApartmentNumber { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public double Rating { get; set; } = 0;

    public int TotalUserRating { get; set; } = 0;

    public bool IsOpen { get; set; } = false;
    public string? PhotoUrl { get; set; }

    public string? Title { get; set; }
    public string? Vicinity { get; set; }

    public ICollection<UserEntity> Users { get; } = [];//The list of users the coffeeshop is in charged of "many to many"

    /* ------------------- Google Data -------------------*/
    //[Index(nameof(PlaceId), IsUnique = true)]
    public string? PlaceId{ get; set; } = string.Empty;
    public string PhotoAttribute { get; set; } = string.Empty;
    public string PhotoReference {  get; set; } = string.Empty;
}
