using Microsoft.IdentityModel.Tokens;
using RS.DataAccessLibrary.Configuration;
using RS.DataAccessLibrary.DTOs.Requests;
using RS.DataAccessLibrary.Interfaces;
using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.Helpers
{
    public static class AuthHelpers
    {
        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="config">The JWTConfig including the secret</param>
        /// <param name="user">The user to generate a token for</param>
        public static string GenerateToken(JWTConfig config, UserModel user)
        {
            // Initialize token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Get key
            var key = Encoding.ASCII.GetBytes(config.Secret);

            // Setup token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                    new Claim(JwtRegisteredClaimNames.Sub, user.EmailAddress),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create and return the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a salt to be added to the users password upon encryption
        /// </summary>
        public static string GenerateSalt() 
        {
            // Create a byte array for the salt
            var saltBytes = new byte[16];

            // Create salt
            using (RandomNumberGenerator RNG = RandomNumberGenerator.Create())
            {
                RNG.GetBytes(saltBytes);
            }

            // Convert the bytes into a Base64 string
            var saltBase64 = Convert.ToBase64String(saltBytes);

            // Return the string
            return saltBase64;
        }

        /// <summary>
        /// Encrypt a given password by adding a salt and hashing it
        /// </summary>
        /// <param name="password">The password entered by the user</param>
        /// <returns>The salt of the user</returns>
        public static string EncryptPassword(this string password, string salt)
        {
            // Create a hasher
            var hasher = SHA256.Create();

            // Encrypt password with an added on salt
            var encryptedPass = Convert.ToBase64String(hasher.ComputeHash(Encoding.Default.GetBytes(password + salt)));

            // Return the encrypted password
            return encryptedPass;
        }

        public static async Task<UserModel> AttemptLogin(IRepository data, UserLoginRequest user)
        {
            // Get the users salt
            var salt = await data.GetUserSalt(user.EmailAddress);

            var pass = user.Password.EncryptPassword(salt);

            user.Password = user.Password.EncryptPassword(salt);

            return await data.AttemptLogin(user.EmailAddress, user.Password);
        }
    }
}
