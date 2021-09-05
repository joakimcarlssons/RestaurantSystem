using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.DTOs.Responses
{
    public class UserLoginResponse
    {
        public UserModel User { get; set; }
        public string Token { get; set; }

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserLoginResponse()
        {

        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        public UserLoginResponse(UserModel user, string token)
        {
            User = user;
            Token = token;
        }

        #endregion
    }
}
