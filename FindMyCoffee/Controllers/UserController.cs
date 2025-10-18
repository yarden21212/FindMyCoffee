using AutoMapper;
using FindMyCoffee.Data;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;
using Microsoft.AspNetCore.Mvc;
using FindMyCoffee.Services.Security;
using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FindMyCoffee.ViewModels;


namespace FindMyCoffee.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UserController : Controller
    {
        private readonly FindMyCoffeeContext _context;
        private readonly IMapper _mapper;

        public UserController(FindMyCoffeeContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserReadDto>> FindUserByID(int id)
        {
            var user = await _context.Users.FindAsync(id); // async version

            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserReadDto>(user));
        }

        public sealed record LoginRequest(string UserName, string Password);
        [HttpPost("loginUser")]
        public async Task<ActionResult<UserReadDto>> Login([FromBody] LoginRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == req.UserName);

            if (user == null) return Unauthorized("Username or password is wrong.");
            else
            {
                if (!PasswordHasher.CheckPasswordHash(req.Password, user.PasswordHash))
                    return Unauthorized("Username or password is wrong.");  // 401

                return Ok(_mapper.Map<UserReadDto>(user));
            }
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<UserReadDto>> Register(RegisterViewModel request)
        {

            if (await _context.Users.AnyAsync(user => user.UserName == request.UserName))
                return Conflict("User name already exists!");
            if (await _context.Users.AnyAsync(user => user.Email == request.Email))
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

            _context.Add(entity);
            await _context.SaveChangesAsync();

            var read = _mapper.Map<UserReadDto>(entity);
            return CreatedAtAction(nameof(FindUserByID), new { id = read.Id }, read);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPost("{id}/role")]
        //public async Task<ActionResult<RegisterRequestData>> SetRole(int userName, [FromBody] SetRoleDto dto)
        //{
        //    //Needs to be implemented one day
        //}
    }   
}
