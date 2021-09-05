using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RS.DataAccessLibrary.Configuration;
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
    public class AuthController : ControllerBase
    {
        #region Private Members

        private readonly IRepository _data;
        private readonly JWTConfig _jwtConfig;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AuthController(IRepository data, IOptionsMonitor<JWTConfig> optionsMonitor)
        {
            _data = data;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        #endregion

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="user">The users credentials</param>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            if (ModelState.IsValid)
            {
                // Attempt to log in the user
                var loggedInUser = await AuthHelpers.AttemptLogin(_data, user);

                if(loggedInUser != null)
                {
                    var token = AuthHelpers.GenerateToken(_jwtConfig, loggedInUser);
                    return Ok(new UserLoginResponse(loggedInUser, token));
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
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest user)
        {
            if (ModelState.IsValid) 
            {
                // Generate salt
                var salt = AuthHelpers.GenerateSalt();

                // Encrypt password
                user.Password = user.Password.EncryptPassword(salt);

                // Create the user and retrieve it as a model
                var newUser = await _data.CreateUser(user, salt);

                // Generate a token for the user
                var token = AuthHelpers.GenerateToken(_jwtConfig, newUser);

                // Return the registered user and a token
                return Ok(new RegisterUserResponse(newUser, token));
            }
            else
            {
                return BadRequest();
            }           
        }
    }
}
