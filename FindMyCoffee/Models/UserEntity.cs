using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindMyCoffee.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class UserEntity
    {

        //// Used by EF Core for optimistic concurrency control.
        //// Automatically updated each time this row changes in the database.
        //// Helps detect conflicting edits (two users updating the same record at once).
        //// Not required right now, but useful if update/edit features are added later.
        //[Timestamp]
        //public byte[] RowVersion { get; set; } //I don't understand this fully yet.. I'll leave it this way for now
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAtUtc { get; set; } = DateTime.UtcNow;

        [Key] public int Id { get; set; }

        [Required, MaxLength(30)]
        [RegularExpression ("^[a-zA-Z0-9]+$")]
        public string UserName { get; set; } = default!;

        [Required, MaxLength(254), EmailAddress]
        public string Email { get; set; } = default!;

        public DateOnly? DOB { get; set; } = default!;

        public Role Role { get; set; } = Role.User;

        [MaxLength(30)]
        [RegularExpression(@"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$",
            ErrorMessage = "Letters only; spaces, hyphens, and apostrophes allowed.")]
        public string FirstName { get; set; } = default!;

        [MaxLength(30)]
        [RegularExpression(@"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$",
            ErrorMessage = "Letters only; spaces, hyphens, and apostrophes allowed.")]
        public string LastName { get; set; } = default!; 

        public Gender Gender { get; set; } = default!;

        [Required, MaxLength(100)]
        public string PasswordHash { get; set; } = default!;

        //Link for many-to-many
        public ICollection<UserCoffeeShopsEntity> UserCoffeeShops { get; set; } = []; // Navigation property for many-to-many

        //For business upgrade

        [Phone]
        public string? BusinessPhone { get; set; }

        [EmailAddress]
        public string? BusinessContactEmail { get; set; }
        public bool AcceptBusinessTerms { get; set; } = false;
        public bool IsBusiness { get; set; } = false;

    }
}
