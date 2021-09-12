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

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.GetUsers", new { }, _config, CurrentConnectionString);
        }

        public async Task<UserModel> GetSingleUserAsync(int userId)
        {
            return (await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.GetUsers", new { UserId = userId }, _config, CurrentConnectionString)).FirstOrDefault();
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            return (await DataAccessHelpers.CallProcedureWithCallbackAsync<bool, dynamic>("dbo.UpdateUser", user, _config, CurrentConnectionString)).FirstOrDefault();
        }

        public async Task<UserModel> CreateUserAsync(RegisterUserRequest user, string salt)
        {
            return (await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.CreateUser",
                new
                {
                    user.EmailAddress,
                    user.Password,
                    @Salt = salt,
                    user.FirstName,
                    user.LastName
                }, _config, CurrentConnectionString)).FirstOrDefault();
        }

        public async Task<string> GetUserSaltAsync(string emailAddress)
            => (await DataAccessHelpers.CallProcedureWithCallbackAsync<string, dynamic>("dbo.GetUserSalt", new { emailAddress }, _config, CurrentConnectionString)).FirstOrDefault();

        public async Task<UserModel> AttemptLoginAsync(string emailAddress, string password)
            => (await DataAccessHelpers.CallProcedureWithCallbackAsync<UserModel, dynamic>("dbo.AttemptLogin", new { emailAddress, password }, _config, CurrentConnectionString)).FirstOrDefault();

        #endregion

        #region Tokens

        public async Task<RefreshTokenModel> SaveRefreshTokenAsync(RefreshTokenModel refreshToken)
            => (await DataAccessHelpers.CallProcedureWithCallbackAsync<RefreshTokenModel, dynamic>(
                "dbo.SaveRefreshToken", 
                new 
                { 
                    refreshToken.UserId, 
                    refreshToken.Token,
                    refreshToken.JwtId,
                    refreshToken.IsUsed,
                    refreshToken.IsRevoked,
                    refreshToken.AddedDate,
                    refreshToken.ExpiryDate
                }, _config, CurrentConnectionString)).FirstOrDefault();

        public async Task<RefreshTokenModel> GetRefreshTokenAsync(string refreshToken)
            => (await DataAccessHelpers.CallProcedureWithCallbackAsync<RefreshTokenModel, dynamic>("dbo.GetRefreshToken", new { @Token = refreshToken }, _config, CurrentConnectionString)).FirstOrDefault();

        #endregion
    }
}
