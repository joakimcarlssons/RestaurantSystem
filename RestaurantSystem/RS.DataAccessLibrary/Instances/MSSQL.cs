using Dapper;
using Microsoft.Extensions.Configuration;
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
            return await DataAccessHelpers.CallProcedureWithCallback<UserModel, dynamic>("dbo.GetUsers", new { }, _config, CurrentConnectionString);
        }

        public async Task<UserModel> GetSingleUser()
        {
            return (await DataAccessHelpers.CallProcedureWithCallback<UserModel, dynamic>("dbo.GetUsers", new { Id = 1 }, _config, CurrentConnectionString)).FirstOrDefault();
        }

        public async Task SaveUser(UserModel user)
        {
            await DataAccessHelpers.CallProcedure<UserModel, dynamic>("dbo.SaveUser", user, _config, CurrentConnectionString);
        }

        #endregion
    }
}
