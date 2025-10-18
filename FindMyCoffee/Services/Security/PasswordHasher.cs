using System.ComponentModel.DataAnnotations;

namespace FindMyCoffee.Services.Security
{
    public class PasswordHasher
    {
        //Accepts a plaintext password from the user (when register) and converts it into hash-type password.
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

        internal static bool CheckPasswordHash((string password, string PasswordHash) value)
        {
            throw new NotImplementedException();
        }
    }

}
