using Dapper;
using Microsoft.Extensions.Configuration;
using RS.DataAccessLibrary.DTOs.Requests;
using RS.DataAccessLibrary.Helpers;
using RS.DataAccessLibrary.Interfaces;
using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.MSSQL
{
    public class MSSQL : IRepository
    {
        #region Private Members

        private readonly IConfiguration _config;

        #endregion

        #region Public Properties

        public string CurrentConnectionString { get; set; } = "Main";

        #endregion

        #region Constructor

        public MSSQL(IConfiguration config)
        {
            _config = config;
        }

        #endregion

        #region Users

        public async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            return await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.GetUsers", new { }, _config, CurrentConnectionString);
        }

        public async Task<UserModel> GetSingleUser(int userId)
        {
            return (await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.GetUsers", new { UserId = userId }, _config, CurrentConnectionString)).FirstOrDefault();
        }

        public async Task UpdateUser(UserModel user)
        {
            await DataAccessHelpers.CallProcedureAsync<UserModel, dynamic>("dbo.UpdateUser", user, _config, CurrentConnectionString);
        }

        public async Task<UserModel> CreateUser(RegisterUserRequest user, string salt)
        {
            return (await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.CreateUser", 
                new 
                { 
                    @EmailAddress   = user.EmailAddress,
                    @Password       = user.Password,
                    @Salt           = salt,
                    @FirstName      = user.FirstName,
                    @LastName       = user.LastName
                }, _config, CurrentConnectionString)).FirstOrDefault();
        }

        public async Task<string> GetUserSalt(string emailAddress)
            => (await DataAccessHelpers.CallProcedureWithCallbackAsync<string, dynamic>("dbo.GetUserSalt", new { emailAddress }, _config, CurrentConnectionString)).FirstOrDefault();

        public async Task<UserModel> AttemptLogin(string emailAddress, string password)
            => (await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.AttemptLogin", new { emailAddress, password }, _config, CurrentConnectionString)).FirstOrDefault();

        #endregion
    }
}
