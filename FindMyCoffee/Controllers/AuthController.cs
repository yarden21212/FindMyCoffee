using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Services.Security;
using FindMyCoffee.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FindMyCoffee.Controllers;


/*
 * This controller handles cookie-based authentication.
 *
 * When a user logs in successfully, the server creates an authentication cookie containing basic user information.
 * 
 * The cookie includes:
 *  - The user's database ID (ClaimTypes.NameIdentifier)
 *  - The user's username        (ClaimTypes.Name)
 *  - The user's role            (ClaimTypes.Role) -> This will permit "Business users" to create new coffee shops.
 */
namespace FindMyCoffee.Controllers;

/* --------------------------------------------------------- Manual cookie authentication ---------------------------------------------------------*/
[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserUniquenessChecker _context;

    public AuthController(IUserUniquenessChecker context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
       
        //Check if a user with the given username exists in the database
        var validUser = await _context.FindByName(request.Username);

        //If doesn't exist -> A wrong Username was given, then alert about it
        if (validUser == null)
        {
            Console.WriteLine("validUser:" + validUser);
            return Unauthorized("Error: Invalid username or password.");
        }

        //The Username exists -> Check if the given password and the given Username's password in the database match.
        var correctPassword = PasswordHasher.CheckPasswordHash(request.Password, validUser.PasswordHash);

        if (!correctPassword)
        {
            return Unauthorized("Error: Invalid username or password.");
        }
        //The passwords match!
        else
        {

            // Claims are used to store data about the logged-in user.
            Console.WriteLine("I'm initializing claims variable right now:");
            var claims = new List<Claim>
{
                new(ClaimTypes.NameIdentifier, validUser.Id.ToString()),
                new(ClaimTypes.Name, validUser.UserName),
                new(ClaimTypes.Role, validUser.Role.ToString()),
            };
            Console.WriteLine("claims variable:" + claims);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine("identify variable:" + identity);
            var principal = new ClaimsPrincipal(identity);
            Console.WriteLine("principle variable:" + principal);

            var props = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),  
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow,
            };
            Console.WriteLine("props variable:" + props);

            Console.WriteLine("Arrived to SignInAsync");
            // Takes the entire ClaimsPrincipal (your "ID card") -> encrypts and serializes it into a single string -> Then sent to the user's browser inside an HTTP Cookie.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal, props);
            Console.WriteLine("SignInAsync has done!");
            return Ok(new { message = "signed-in" });
        }
       
    }

    [Authorize]
    [HttpGet("getUsername")]
    // This endpoint just returns the username of the logged-in user.
    // It is protected by the [Authorize] attribute, which means that only logged-in users can access it.
    public ActionResult<LoginResponse> Get()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);


        if (username == null)
        {
            return Unauthorized();
        }

        Console.WriteLine("Username value is:" + username);
        Console.WriteLine("Username type is::" + username.GetType());


        return new LoginResponse(username);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        // Clear the existing external cookie
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok();
    }
}


public record LoginResponse(string Username);
public record LoginRequest(string Username, string Password);