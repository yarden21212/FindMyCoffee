using AutoMapper;
using FindMyCoffee.Data;
using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Domain;
using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;
using FindMyCoffee.Services.Security;
using FindMyCoffee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * This controller manages user-related operations.
 *
 * Responsibilities:
 *  - Registering new users -> Integrating with the database, adds a new UserEntity to the database.
 *  - Retrieving existing users by ID
 *
 * This controller integrates with the database through the FindMyCoffeeContext
 * and uses AutoMapper to convert between entities and DTOs.
 */
namespace FindMyCoffee.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly FindMyCoffeeContext _legacyContext;
        private readonly IUserManager _context;
        private readonly IMapper _mapper;

        public UserController(FindMyCoffeeContext legacyContext, IUserManager context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _legacyContext = legacyContext;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserReadDto>> FindUserByID(int id)
        {
            //var user = await _context.Users.FindAsync(id); // async version
            try
            {
                var user = await _context.FindUserById(id);

                return Ok(_mapper.Map<UserReadDto>(user));

            }
            catch
            {
                return NotFound();
            }

        }

       

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<UserReadDto>> Register(RegisterViewModel request)
        {

            if (await _legacyContext.Users.AnyAsync(user => user.UserName == request.UserName))
                return Conflict("User name already exists!");
            if (await _legacyContext.Users.AnyAsync(user => user.Email == request.Email))
                return Conflict("Email already exists!");

            UserRequestLogic logic;
            try
            {
                logic = new UserRequestLogic(request.UserName, request.Email, request.DOB, request.FirstName, request.LastName, request.Gender, request.Password, request.ConfirmPassword);
            }catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }

            UserRegistrationDto dto = _mapper.Map<UserRegistrationDto>(logic);

            UserEntity entity = _mapper.Map<UserEntity>(dto);
            entity.PasswordHash = PasswordHasher.HashPassword(dto.Password);
            entity.Role = Role.User;

            _legacyContext.Add(entity);
            await _legacyContext.SaveChangesAsync();

            var read = _mapper.Map<UserReadDto>(entity);
            return CreatedAtAction(nameof(FindUserByID), new { id = read.Id }, read);
        }
        
    }   
}
