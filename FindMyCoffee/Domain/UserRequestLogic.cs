using FindMyCoffee.Domain.Enums;
using Microsoft.Extensions.Primitives;
using System;
using FindMyCoffee.Services.Security;

namespace FindMyCoffee.Domain
{

    /*
     * Domain-level validation object responsible for enforcing business rules during user registration.
     * Ensures that invalid user data cannot enter the system.
    */
    public class UserRequestLogic
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateOnly? Dob { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string Password { get; set; }

        public UserRequestLogic(string userName, string email, DateOnly? DOB, string firstName, string lastName, Gender gender, string password, string ConfirmPassword)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName) + ": First name must contain at least 1 character");
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException(nameof(lastName) + ": Last name must contain at least 1 character");
            if(!UserInfoChecker.IsEmailValid(email))
                throw new ArgumentException(nameof(email) + ": Email form is incorrect, maybe '@' is missing? try again");
            //if (!email.Contains('@'))
            //    throw new ArgumentException(nameof(email) + ": Email must contain '@', try again");
            if (!Enum.IsDefined(typeof(Gender), gender))
                throw new ArgumentException(nameof(gender) + ": Gender must be 'Male', 'Female' or 'Other'");
            if(password != ConfirmPassword)
                 throw new ArgumentException("Passwords do not match");

            UserName = userName;
            Email = email;
            Dob = DOB;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            Password = password;
        }
    }
}
