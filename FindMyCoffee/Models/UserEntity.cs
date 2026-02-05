using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindMyCoffee.Models
{

    /*
     * Database entity that represents an application user in the FindMyCoffee system.
     *
     * Responsibilities:
     *   Stores user account data (UserName, Email, PasswordHash) and profile information (DOB, Gender, FirstName, LastName).
     *   Tracks timestamps (CreatedAtUtc, ModifiedAtUtc) -> Especially for future use.
     *   Stores authorization role (Role) used for permission checks (e.g., regular user vs. business user vs. future admin user).
     *
     * Validation / constraints:
     *   UserName is unique and restricted to alphanumeric and numeric characters
     *   Email is unique and validated by EmailAddress attribute.
     *   FirstName/LastName are limited to letters with optional spaces/hyphens/apostrophes.
     *
     * Relationships:
     *   UserCoffeeShops: many-to-many link to coffee shops via UserCoffeeShopsEntity.
     *
     * Business upgrade fields:
     *   Supports upgrading a user to a business account by storing business contact info and flags
     *   (BusinessPhone, BusinessContactEmail, AcceptBusinessTerms, IsBusiness).
     */


    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class UserEntity
    {
        

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
        public bool AcceptBusinessTerms { get; set; } = false; // For future use
        public bool IsBusiness { get; set; } = false;

    }
}
