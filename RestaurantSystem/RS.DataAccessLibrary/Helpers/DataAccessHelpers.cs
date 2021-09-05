using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.Helpers
{
    public static class DataAccessHelpers
    {
        /// <summary>
        /// Call a stored procedure withouth any callback
        /// </summary>
        /// <typeparam name="TModel">The model to be returned</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters of the stored procedure to be called</param>
        /// <param name="config">The config file to get the connection string from</param>
        /// <param name="connectionStringName">The name of stored procedure stored in the injected config file</param>
        public static async Task CallProcedureAsync<TModel, TParams>(string storedProcedure, TParams parameters, IConfiguration config, string connectionStringName)
        {
            using var connection = new SqlConnection(config.GetConnectionString(connectionStringName));
            await connection.QueryAsync<TModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Call a stored procedure with a callback
        /// </summary>
        /// <typeparam name="TModel">The model to be returned</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters of the stored procedure to be called</param>
        /// <param name="config">The config file to get the connection string from</param>
        /// <param name="connectionStringName">The name of stored procedure stored in the injected config file</param>
        public static async Task<IEnumerable<TModel>> CallProcedureWithCallbackAsync<TModel, TParams>(string storedProcedure, TParams parameters, IConfiguration config, string connectionStringName)
        {
            using var connection = new SqlConnection(config.GetConnectionString(connectionStringName));
            return await connection.QueryAsync<TModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
