namespace FindMyCoffee.Domain;

public interface IUserUniquenessChecker
{
    Task<bool> UserNameExistsAsync(string userName);
    Task<bool> EmailExistsAsync(string email);
}
