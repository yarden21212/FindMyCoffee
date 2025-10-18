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

        [Timestamp]
        public byte[] RowVersion { get; set; } //I don't understand this fully yet.. I'll leave it this way for now
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

    }
}
