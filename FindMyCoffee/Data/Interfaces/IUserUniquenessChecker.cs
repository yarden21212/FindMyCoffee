using FindMyCoffee.Dtos;
using FindMyCoffee.Models;

namespace FindMyCoffee.Data.Interfaces;

public interface IUserUniquenessChecker
{
    Task<bool> UserNameExistsAsync(string userName);
    Task<bool> EmailExistsAsync(string email);
    Task<UserEntity> FindUserById(int id);
    Task<UserEntity?> FindByName(string userName);
    Task<bool> UpgradeRole(UserEntity user);
    Task<bool> UpdatePhoneAndEmail(UserEntity user, string BusinessContactEmail, string BusinessPhone);
}
