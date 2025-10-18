using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.ViewModels
{
    public class ResendConfirmationEmailViewModel
    {
        [Required(ErrorMessage = "Email Id is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
    }
}
