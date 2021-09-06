using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RS.DataAccessLibrary.DTOs;
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
        /// <param name="config">The config implementation</param>
        /// <param name="user">The user to generate a token for</param>
        public static async Task<TokenResult> GenerateTokens(IConfiguration config, UserModel user, IRepository data)
        {
            // Initialize token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Get key
            var key = Encoding.ASCII.GetBytes(config["JWTConfig:Secret"]);

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
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = config["JWTConfig:Issuer"],
                Audience = config["JWTConfig:Audience"]
            };

            // Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Create a refresh token
            var refreshToken = GenerateRefreshToken(token, user);

            // Save the refresh token to the database
            await data.SaveRefreshTokenAsync(refreshToken);

            // Return the tokens
            return new TokenResult { Token = tokenHandler.WriteToken(token), RefreshToken = refreshToken.Token };
        }

        /// <summary>
        /// Generates a refresh token for a given user
        /// </summary>
        /// <param name="token">The original created token</param>
        /// <param name="user">The user connected to the token</param>
        public static RefreshTokenModel GenerateRefreshToken(SecurityToken token, UserModel user)
            => new()
               {
                   JwtId = token.Id,
                   IsUsed = false,
                   IsRevoked = false,
                   UserId = user.UserId,
                   AddedDate = DateTime.UtcNow,
                   ExpiryDate = DateTime.UtcNow.AddMonths(6),
                   Token = GenerateSalt(35) + Guid.NewGuid()
               };

        /// <summary>
        /// Verifies and generates a new token for the user
        /// </summary>
        /// <param name="tokenRequest">The object containing the tokens</param>
        /// <param name="tokenValidationParams">The parameters specified for the tokens</param>
        /// <param name="data">The repository instance</param>
        /// <param name="config">The JWT config instance</param>
        public static async Task<TokenResult> VerifyAndGenerateToken(TokenRequest tokenRequest, TokenValidationParameters tokenValidationParams, IRepository data, IConfiguration config)
        {
            // Create instance of the token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                #region Validations

                tokenValidationParams.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JWTConfig:Secret"]));

                // 01. Validate JWT token format
                var tokenInVerification = tokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParams, out var validatedToken);

                // 02. Validate the type and encryption of token
                if (validatedToken is JwtSecurityToken securityToken)
                {
                    var result = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) return null;
                }

                // 03. Validate expiry time of token
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // Get the expiration date
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                // Check if the token has expired or not
                // If it hasn't expired, then don't refresh it
                if (expiryDate > DateTime.UtcNow)
                {
                    return null;
                }

                // 04. Check that the refresh token exists
                var storedToken = await data.GetRefreshTokenAsync(tokenRequest.RefreshToken);
                if (storedToken == null)
                {
                    return null;
                }

                // 05. Check if the token is used
                if (storedToken.IsUsed)
                {
                    return null;
                }

                // 06. Ceck if the token has been revoked
                if (storedToken.IsRevoked)
                {
                    return null;
                }

                // 07. Make sure the JTI matches
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return null;
                }

                #endregion

                // Update current token
                storedToken.IsUsed = true;
                await data.SaveRefreshTokenAsync(storedToken);

                // Get the user
                var user = await data.GetSingleUserAsync(storedToken.UserId);

                // Generate a new token
                return await GenerateTokens(config, user, data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the expiry date out of the seconds in a tokens claims
        /// </summary>
        /// <param name="unixTimeStamp">The number of seconds registered in the token</param>
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Get the first day of Utc time
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            // Add the expiry time
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            // Return the result
            return dateTimeVal;
        }

        /// <summary>
        /// Generates a salt to be added to the users password upon encryption
        /// </summary>
        public static string GenerateSalt(int length) 
        {
            // Create a byte array for the salt
            var saltBytes = new byte[length];

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
            var salt = await data.GetUserSaltAsync(user.EmailAddress);

            var pass = user.Password.EncryptPassword(salt);

            user.Password = user.Password.EncryptPassword(salt);

            return await data.AttemptLoginAsync(user.EmailAddress, user.Password);
        }
    }
}
