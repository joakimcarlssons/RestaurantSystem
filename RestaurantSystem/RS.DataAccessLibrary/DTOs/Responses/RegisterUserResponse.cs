using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.DTOs.Responses
{
    public class RegisterUserResponse
    {
        #region Public Properties

        public UserModel User { get; set; }
        public string Token { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RegisterUserResponse()
        {
                
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        public RegisterUserResponse(UserModel user, string token)
        {
            User = user;
            Token = token; 
        }

        #endregion
    }
}
