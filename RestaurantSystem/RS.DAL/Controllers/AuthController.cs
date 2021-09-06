using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RS.DataAccessLibrary.DTOs.Requests;
using RS.DataAccessLibrary.DTOs.Responses;
using RS.DataAccessLibrary.Helpers;
using RS.DataAccessLibrary.Interfaces;
using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.DAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        #region Private Members

        private readonly IRepository _data;
        private readonly IConfiguration _config; 
        private readonly TokenValidationParameters _tokenValidationParams;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AuthController(
            IRepository data,
            IConfiguration config,
            TokenValidationParameters tokenValidationParams)
        {
            _data = data;
            _config = config;
            _tokenValidationParams = tokenValidationParams;
        }

        #endregion

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="user">The users credentials</param>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            if (ModelState.IsValid)
            {
                // Attempt to log in the user
                var loggedInUser = await AuthHelpers.AttemptLogin(_data, user);

                if(loggedInUser != null)
                {
                    var tokens = await AuthHelpers.GenerateTokens(_config, loggedInUser, _data);

                    return Ok(new UserLoginResponse(loggedInUser, tokens));
                }
                else
                {
                    return NotFound("Could not find user.");
                }
            }
            else
            {
                return BadRequest();
            }

        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="user">The users credentials</param>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest user)
        {
            if (ModelState.IsValid) 
            {
                // Generate salt
                var salt = AuthHelpers.GenerateSalt(16);

                // Encrypt password
                user.Password = user.Password.EncryptPassword(salt);

                // Create the user and retrieve it as a model
                var newUser = await _data.CreateUserAsync(user, salt);

                // Return the registered user and a token
                return Ok(new RegisterUserResponse(newUser));
            }
            else
            {
                return BadRequest();
            }           
        }

        /// <summary>
        /// Refreshes a token for a user
        /// </summary>
        /// <param name="tokenRequest">The <see cref="TokenRequest"/> object containing the tokens</param>
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await AuthHelpers.VerifyAndGenerateToken(tokenRequest, _tokenValidationParams, _data, _config);

                // If the validation fails, return a bad request
                if (result == null) return BadRequest("Invalid Tokens");

                // If everything is good, return the result
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
