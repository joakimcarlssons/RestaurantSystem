using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RS.DataAccessLibrary.DTOs;
using RS.DataAccessLibrary.DTOs.Requests;
using RS.DataAccessLibrary.DTOs.Responses;
using RS.DataAccessLibrary.Helpers.Models;
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
        #region Tokens

        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="config">The config implementation</param>
        /// <param name="user">The user to generate a token for</param>
        public static async Task<AuthResult> GenerateTokens(IConfiguration config, UserModel user, IRepository data)
        {
            try
            {
                // Initialize token handler
                var tokenHandler = new JwtSecurityTokenHandler();

                // Get key
                var key = Encoding.ASCII.GetBytes(config["JWTConfig:Secret"]);

                // Setup token
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim("Id", user.UserId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                        new Claim(JwtRegisteredClaimNames.Sub, user.EmailAddress),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = config["JWTConfig:Issuer"],
                    Audience = config["JWTConfig:Audience"]
                };

                // Add user roles
                await AddUserRoles(data, user, tokenDescriptor);

                // Create the token
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Create a refresh token
                var refreshToken = GenerateRefreshToken(token, user);

                // Save the refresh token to the database
                await data.SaveRefreshTokenAsync(refreshToken);

                // Return the tokens
                return new AuthResult { Token = tokenHandler.WriteToken(token), RefreshToken = refreshToken.Token };
            }
            catch (Exception ex)
            {
                return new AuthResult { Error = new ErrorResponse(500, ex.Message) };
            }
        }

        /// <summary>
        /// Add user roles as claims to the token
        /// </summary>
        /// <param name="data">The repository instance</param>
        /// <param name="user">The user to get the roles of</param>
        /// <param name="tokenDescriptor">The <see cref="SecurityTokenDescriptor"/> to add the roles to</param>
        public static async Task AddUserRoles(IRepository data, UserModel user, SecurityTokenDescriptor tokenDescriptor)
        {
            // Get the user roles
            var userRoles = await data.GetUserRolesAsync(user.UserId);

            // Assign the roles
            foreach (var role in userRoles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim("Roles", role.RoleName));
            }
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
        public static async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest, TokenValidationParameters tokenValidationParams, IRepository data, IConfiguration config)
        {
            // Create instance of the token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var refreshTokenValidationParams = new RefreshTokenValidationParameters(tokenValidationParams);

            try
            {
                #region Validations

                // 01. Validate JWT token format
                var tokenInVerification = tokenHandler.ValidateToken(tokenRequest.Token, refreshTokenValidationParams, out var validatedToken);

                // 02. Validate the type and encryption of token
                if (validatedToken is JwtSecurityToken securityToken)
                {
                    var result = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) return new AuthResult
                    {
                        Error = new ErrorResponse(400, "The type of token does not match the type used within this system.")
                    };
                }

                // 03. Validate expiry time of token
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // Get the expiration date
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                // Check if the token has expired or not
                // If it hasn't expired, then don't refresh it
                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Error = new ErrorResponse(400, "Token has not yet expired.")
                    };
                }

                // 04. Check that the refresh token exists
                var storedToken = await data.GetRefreshTokenAsync(tokenRequest.RefreshToken);
                if (storedToken == null)
                {
                    return new AuthResult
                    {
                        Error = new ErrorResponse(404, "Could not find refresh token.")
                    };
                }

                // 05. Check if the token is used
                if (storedToken.IsUsed)
                {
                    return new AuthResult
                    {
                        Error = new ErrorResponse(400, "Refresh token has already been used.")
                    };
                }

                // 06. Ceck if the token has been revoked
                if (storedToken.IsRevoked)
                {
                    return new AuthResult
                    {
                        Error = new ErrorResponse(400, "Refresh token has been revoked.")
                    };
                }

                // 07. Make sure the JTI matches
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return new AuthResult
                    {
                        Error = new ErrorResponse(400, "Tokens does not match.")
                    };
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
                return new AuthResult
                {
                    Error = new ErrorResponse(500, ex.Message)
                };
            }
        }

        /// <summary>
        /// Get the expiry date out of the seconds in a tokens claims
        /// </summary>
        /// <param name="unixTimeStamp">The number of seconds registered in the token</param>
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Get the first day of Utc time
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);

            // Return the result
            return dateTimeVal;
        }

        #endregion

        #region Registration and Login

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
        public static void EncryptPassword(this string password, string salt)
        {
            // Create a hasher
            var hasher = SHA256.Create();

            // Encrypt password with an added on salt
            Convert.ToBase64String(hasher.ComputeHash(Encoding.Default.GetBytes(password + salt)));
        }

        public static async Task<UserModel> AttemptLogin(IRepository data, UserLoginRequest user)
        {
            // Get the users salt
            var salt = await data.GetUserSaltAsync(user.EmailAddress);

            user.Password.EncryptPassword(salt);

            return await data.AttemptLoginAsync(user.EmailAddress, user.Password);
        }

        /// <summary>
        /// Performing internal validation rules against the <see cref="RegisterUserRequest"/>
        /// </summary>
        /// <param name="registerUserRequest">The request sent as the registration</param>
        /// <param name="data">The repository instance to call the database</param>
        /// <returns>A boolean value indicating whether thhe calidation was successful and a reason string</returns>
        public static async Task<ErrorResponse> ValidateRegistrationRequest(this RegisterUserRequest registerUserRequest, IRepository data)
        {
            // 01. Verify that the email is not in use
            var emailExistenceResult = await data.VerifyEmailAddressExistence(registerUserRequest.EmailAddress);

            if (emailExistenceResult == 0) return new ErrorResponse(500, "Something went wrong on database level.");
            else if (emailExistenceResult != null) return new ErrorResponse(400, "Email address already exists");

            // 02. Verify that passwords match
            if (registerUserRequest.Password != registerUserRequest.ConfirmPassword) return new ErrorResponse(400, "Passwords don't match.");

            // If everything is ok, return null
            return null;
        }

        #endregion
    }
}
