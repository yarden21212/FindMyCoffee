using FindMyCoffee.Domain;

namespace FindMyCoffee.Data;

public class UserUniquenessChecker : IUserUniquenessChecker
{
    private readonly FindMyCoffeeContext _context;

    public UserUniquenessChecker(FindMyCoffeeContext context)
    {
        _context = context;
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        if (_context.Users.Any(user => user.Email == email))
        {
            throw new ArgumentException("The given email is already in use, use a different one please");
        }

        return Task.FromResult(true);
    }

    public Task<bool> UserNameExistsAsync(string userName)
    {
        if (_context.Users.Any(user => user.UserName == userName))
        {
            throw new ArgumentException("The given Username is already taken, try something else please");
        }

        return Task.FromResult(true);
    }
}
