using FindMyCoffee.Dtos;
using FindMyCoffee.Models;

namespace FindMyCoffee.Data.Interfaces;

/*
 * IUserUniquenessChecker
 * Used for checking if a username or email is already taken and for handling role upgrades and contact updates.
 * It’s not a repository because it doesn’t provide full user CRUD, only specific checks and update operations.
 */
public interface IUserManager
{
    Task<bool> UserNameExistsAsync(string userName);
    Task<bool> EmailExistsAsync(string email);
    Task<UserEntity> FindUserById(int id);
    Task<UserEntity?> FindByName(string userName);
    Task<bool> UpgradeRole(UserEntity user);
    Task<bool> UpdatePhoneAndEmail(UserEntity user, string BusinessContactEmail, string BusinessPhone);
}
