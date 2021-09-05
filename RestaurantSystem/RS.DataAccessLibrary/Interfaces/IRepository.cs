using Dapper;
using Microsoft.Extensions.Configuration;
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
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetAllUsers();

        /// <summary>
        /// Get a single user from the database
        /// </summary>
        /// <returns></returns>
        Task<UserModel> GetSingleUser(int userId);

        /// <summary>
        /// Updates a user to the database
        /// </summary>
        Task UpdateUser(UserModel user);

        #endregion
    }
}