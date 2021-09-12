using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RS.DAL.Claims;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ClaimRequirement(ClaimTypes.Roles, "Administrator")]
    public class UsersController : ControllerBase
    {
        #region Private Members

        private readonly IRepository _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UsersController(IRepository data)
        {
            _data = data;
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Get all available users
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _data.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Get a single user
        /// </summary>
        /// <param name="userId">The id of the user to get</param>
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetSingleUser(int userId)
        {
            try
            {
                var user = await _data.GetSingleUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="user">The user</param>
        [HttpPatch]
        [Route("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody]UserModel user)
        {
            try
            {
                // Make sure it's the right user sent in
                if(userId == user.UserId)
                {
                    var succeeded = await _data.UpdateUserAsync(user);

                    if (succeeded) return Ok(user);
                    else return StatusCode(500, "Updated failed on database level.");
                }
                else
                {
                    return BadRequest("The UserId in the URL does not match the one in the body.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion
    }
}
