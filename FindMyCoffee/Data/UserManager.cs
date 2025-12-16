using AutoMapper;
using FindMyCoffee.Data.Interfaces;
using FindMyCoffee.Domain;
using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Dots;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;
using FindMyCoffee.Services.Security;
using Microsoft.EntityFrameworkCore;
using static FindMyCoffee.Controllers.BusinessController;

namespace FindMyCoffee.Data;

public class UserManager : IUserManager
{
    private readonly FindMyCoffeeContext _context;
    private readonly IMapper _mapper;

    public UserManager(FindMyCoffeeContext context)
    {
        _context = context;
    }

    /* Checks whether a user with the given email exists. */
    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if(user == null) 
        {
            return false;
        }

        return true;
    }

    /* Checks whether a username is already taken. */
    public async Task<bool> UserNameExistsAsync(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            return false;
        }

        return true;
    }

    /* Retrieves a user by ID or throws if the user is not found. */
    public async Task<UserEntity> FindUserById(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new UnauthorizedAccessException("User Doesn't exist");

        return user;
    }

    /* Finds a user by username(case-insensitive). Used mainly during registration and login. */
    public async Task<UserEntity?> FindByName(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == userName.ToUpper());
    }

    /* Upgrades a regular user to a Business role if possible. */
    public async Task<bool> UpgradeRole(UserEntity user)
    {
        Console.WriteLine("Inside UserUniquenessChecker -> UpgradeRole");
        if (user == null)
        {
            Console.WriteLine("UpgradeRole -> user == null");
            return false;
        }

        var currentRole = user.Role;
        if (currentRole == Role.User)
        {
            Console.WriteLine("UserUniquenessChecker -> UpgradeRole -> user != null");

            user.Role = Role.Business;
            // Persist the change to the database asynchronously
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            Console.WriteLine("Confirmation: User has been upgraded to 'Business'");
            return true;
        }
        else
        {
            Console.WriteLine("Error: User is already a business or admin!");
            return false;
        }
    }

    /* Updates the user's business email and phone if the values are valid. */
    public async Task<bool> UpdatePhoneAndEmail(UserEntity user, string BusinessContactEmail, string BusinessPhone)
    {

        user.BusinessPhone = BusinessPhone;
        user.BusinessContactEmail = BusinessContactEmail;
        bool infoOk = UserInfoChecker.IsEmailValid(BusinessContactEmail) && UserInfoChecker.IsPhoneNumberValid(BusinessPhone);
        if (!infoOk) { return false; }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    /* Returns the user's role as a string. */
    public async Task<string> GetRole(UserEntity user)
    {
        // To satisfy CS1998, use Task.FromResult to return a Task<string>
        return await Task.FromResult(user.Role.ToString());
    }
}
