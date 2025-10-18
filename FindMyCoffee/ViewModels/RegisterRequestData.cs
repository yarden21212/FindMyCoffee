using FindMyCoffee.Domain.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace FindMyCoffee.ViewModels
{
    public record RegisterViewModel(

    [Required, StringLength(30, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z0-9]+$")]
    string UserName,

    [Required, EmailAddress, StringLength(254)]
    string Email,

    [Required, DataType(DataType.Date)]
    DateOnly? DOB,

    [Required, StringLength(100, MinimumLength = 8)]
    string Password,
    [Required, StringLength(100, MinimumLength = 8)]
    string ConfirmPassword,

    [Required, StringLength(30, MinimumLength = 1)]
    string FirstName,

    [Required, StringLength(30, MinimumLength = 1)]
    string LastName,

    Gender Gender
     );
}














//using FindMyCoffee.Domain.Enums;
//using Newtonsoft.Json;
//using System.ComponentModel.DataAnnotations;
//using System.Security.Cryptography.X509Certificates;
//namespace FindMyCoffee.ViewModels
//{
//    public class RegisterViewModel
//    {

//        [Required]
//        [Display(Name = "First Name")]
//        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
//        public string FirstName { get; set; } = null!;

//        [Display(Name = "Last Name")]
//        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
//        public string? LastName { get; set; }

//        [Required(ErrorMessage = "Email Id is Required")]
//        [EmailAddress(ErrorMessage = "Invalid Email Address")]
//        public string Email { get; set; } = null!;

//        [DataType(DataType.Date)]
//        [EmailAddress(ErrorMessage = "Invalid Email Address")]
//        public DateTime? DOB { get; set; }

//        [Required, StringLength(30, MinimumLength = 1)]
//        [RegularExpression("^[a-zA-Z0-9]+$")]
//        public string UserName { get; set; }

//        [Required]
//        [DataType(DataType.Password)]
//        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 6 characters.")]
//        public string Password { get; set; } = null!;

//        [Required]
//        [DataType(DataType.Password)]
//        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
//        [Display(Name = "Confirm Password")]
//        public string ConfirmPassword { get; set; } = null!;

//        public Gender Gender;

//        //[Required, EmailAddress, StringLength(254)]
//        //string Email,

//        //[Required, DataType(DataType.Date)]
//        //DateOnly? DOB,

//        //[Required, StringLength(30, MinimumLength = 1)]
//        //string FirstName,

//        //[Required, StringLength(30, MinimumLength = 1)]
//        //string LastName,

//        //[Required(ErrorMessage = "PhoneNumber is Required")]
//        //[Phone(ErrorMessage = "Please enter a valid Phone number")]
//        //[Display(Name = "Phone Number")]
//        //public string PhoneNumber { get; set; } = default!;
//    }
//}