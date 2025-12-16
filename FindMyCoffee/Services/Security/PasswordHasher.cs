using System.ComponentModel.DataAnnotations;

/*
 * Converts plain-text password to hashed + salted password and store it inside the database.
 * Gives better security, passwords are easy to hack while in plain-text form.
 */
namespace FindMyCoffee.Services.Security
{
    public class PasswordHasher
    {
        //Accepts a plain-text password from the user (when register) and converts it into hash-type password.
        public static string HashPassword(string rawPassword)
        {
            if(rawPassword.Length < 8)
            {
                throw new ArgumentOutOfRangeException("Password is too short! Password has to contain at least 8 digits");
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            return hashPassword;
        }

        //Compares the given password of the user when tries to login with the "passwordHash" from the database (probably will be pulled by the user's name given from the database)
        public static bool CheckPasswordHash(string rawPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(rawPassword, passwordHash);
        }
    }

}
