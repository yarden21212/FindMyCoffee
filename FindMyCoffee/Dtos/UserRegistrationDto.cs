using FindMyCoffee.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.Dtos
{
    public class UserRegistrationDto
    {
        
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateOnly? DOB { get; set; }           
        public string FirstName { get; set; }               
        public string LastName { get; set; }                                                
        public Gender Gender { get; set; }                                                   
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
