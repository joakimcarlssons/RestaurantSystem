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
        /// <summary>
        /// The logged in user
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// The JWT tokens of the user
        /// </summary>
        public TokenResult Tokens { get; set; }

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
        public UserLoginResponse(UserModel user, TokenResult tokens)
        {
            User = user;
            Tokens = tokens;
        }

        #endregion
    }
}
