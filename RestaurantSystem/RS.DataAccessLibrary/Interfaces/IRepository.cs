using Dapper;
using Microsoft.Extensions.Configuration;
using RS.DataAccessLibrary.DTOs.Requests;
using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.Interfaces
{
    public interface IRepository
    {
        #region Properties

        /// <summary>
        /// The current connection string to use
        /// </summary>
        string CurrentConnectionString { get; set; }

        #endregion

        #region Users

        /// <summary>
        /// Get all users from the database
        /// </summary>
        Task<IEnumerable<UserModel>> GetAllUsers();

        /// <summary>
        /// Get a single user from the database
        /// </summary>
        /// <param name="userId">The id of the user to get</param>
        Task<UserModel> GetSingleUser(int userId);

        /// <summary>
        /// Updates a user to the database
        /// </summary>
        /// <param name="user">The user to update</param>
        Task UpdateUser(UserModel user);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="user">The information of the user to create</param>
        /// <param name="salt">The salt generated for the user</param>
        Task<UserModel> CreateUser(RegisterUserRequest user, string salt);

        /// <summary>
        /// Get the users salt
        /// </summary>
        /// <param name="emailAddress">The email address of the user</param>
        Task<string> GetUserSalt(string emailAddress);

        /// <summary>
        /// Attempt to login as a user
        /// </summary>
        /// <param name="emailAddress">The email address of the user</param>
        /// <param name="password">The password of the user</param>
        Task<UserModel> AttemptLogin(string emailAddress, string password);

        #endregion
    }
}