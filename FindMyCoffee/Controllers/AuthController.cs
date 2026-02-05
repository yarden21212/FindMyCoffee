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
 *  
 * This video explains the subject well: https://www.youtube.com/watch?v=SV9Jd2cauxw
 */
namespace FindMyCoffee.Controllers;

/* --------------------------------------------------------- Manual cookie authentication ---------------------------------------------------------*/
[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserManager _context;

    public AuthController(IUserManager context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
       
        // Validation: checks if a user with the given username exists in the database
        var validUser = await _context.FindByName(request.Username);

        //If doesn't exist -> A wrong Username was given, then alert about it
        if (validUser == null)
        {
            Console.WriteLine("validUser:" + validUser);
            return Unauthorized("Error: Invalid username or password.");
        }

        //Validation: the username exists -> checks if the given password and the given username's password in the database match.
        var correctPassword = PasswordHasher.CheckPasswordHash(request.Password, validUser.PasswordHash);

        if (!correctPassword)
        {
            return Unauthorized("Error: Invalid username or password.");
        }

        else
        {
            //The passwords match!

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
            Console.WriteLine("Identity variable:" + identity);
            var principal = new ClaimsPrincipal(identity);
            Console.WriteLine("principle variable:" + principal);

            var props = new AuthenticationProperties()
            {
                IsPersistent = true,                             // The cookie remains\survives if the browser is closed.
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),   // The cookie remains for 24 hours if wasn't switched 
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow,
            };
            Console.WriteLine("props variable:" + props);

            Console.WriteLine("Arrived to SignInAsync");
            // Takes all the info, encrypts it, and sends it to the browser as a cookie.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal, props);
            Console.WriteLine("SignInAsync has done!");
            return Ok(new { message = "signed-in" });
        }
       
    }

    // "Authorized" -> Only logged-in users can use it (only request with valid cookie), otherwise .NET will reject it (before the code will even run)
    //In other words: If a logged-in user exists (which means a cookie exists) then this HTTP will return the cookie's user name, otherwise, will reject it immediately.
    [Authorize]
    [HttpGet("getUsername")]
    public ActionResult<LoginResponse> GetUsername()
    {
        var username = User.FindFirstValue(ClaimTypes.Name); // User is part of "Authentication Middleware", should read about it more. MiddleWare -> seems cookie -> Decrypts it -> Find the user-name


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