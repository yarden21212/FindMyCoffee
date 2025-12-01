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