using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Models;
using FindMyCoffee.Services.Security;
using FindMyCoffee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/*
 * This controller handles upgrading a regular user into a business user (role: User → Business).
 *
 * All business upgrade actions require authentication, ensuring that only logged-in users
 * can request to become a business account.
 */

namespace FindMyCoffee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : Controller
    {
        private readonly IUserUniquenessChecker _context;

        public BusinessController(IUserUniquenessChecker context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("RegisterAsBusiness")]
        public async Task<ActionResult> RegisterAsBusiness(BusinessViewModel businessData)
        {
            Console.WriteLine("Inside BusinessController -> RegisterAsBusiness");

            //HttpContext represents the current HTTP request and response (of the current cookie -> it's an object provided by ASP.NET Core).
            //.User is the currently logged-in user's claims, that are stored in the cookies or in the JWT token
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("The value pulled from the db is: " + id);

            if (id == null)
                return NotFound("No one is logged in? Try to login in first!");

            //TODO: To check if the phone number and email are correct -> Let the user react react on them (email sent to the user's mail, and phone-number to get a notification -> the user approve it)
            var phoneNumber = businessData.Phone;
            var contactEmail = businessData.ContactEmail;

            Console.WriteLine("RegisterAsBusiness -> Checks phoneNumber &&  contactEmail");
            //TODO: var AcceptBusinessTerms = businessData.AcceptBusinessTerms;
            if (phoneNumber != null && contactEmail != null)
            {
                /* Check if the given email and phone-number are valid*/
                bool phoneNumberValid = UserInfoChecker.IsPhoneNumberValid(phoneNumber);
                bool emailValid = UserInfoChecker.IsEmailValid(contactEmail);
                if (!phoneNumberValid)
                    return BadRequest("Phone number given is invalid!");
                if (!emailValid)
                    return BadRequest("Email given is invalid!");

                Console.WriteLine("------------------------------------------------");
                int idToInt = Int32.Parse(id);
                Console.WriteLine("The id string converted to int is: " + idToInt);
                var user = await _context.FindUserById(idToInt);

                //This null may happen only if there is problem with the integration between the cookie and the database
                if (user == null)
                    return NotFound("Something went wrong with the cookie and database!");
                Console.WriteLine("------------------------------------------------");

                Console.WriteLine("BusinessController -> RegisterAsBusiness -> Calling UserToBusiness method");
                await UserToBusiness(user, phoneNumber, contactEmail);
            }

            return Ok();
        }


        private async Task<bool> UserToBusiness(UserEntity user, string phoneNumber, string contactEmail)
        {
            Console.WriteLine("Entered: BusinessController -> UserToBusiness method");

            var confirmation = await _context.UpgradeRole(user);
            if (!confirmation)
            {
                Console.WriteLine("Error: User has not been upgraded!");
                return false;
            }

            await _context.UpdatePhoneAndEmail(user, contactEmail, phoneNumber);

            return true;
        }

        public record User(string Username);
    }
}