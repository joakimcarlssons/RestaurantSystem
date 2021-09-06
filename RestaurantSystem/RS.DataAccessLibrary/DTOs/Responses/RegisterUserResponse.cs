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

        /// <summary>
        /// The registered user
        /// </summary>
        public UserModel User { get; set; }

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
        public RegisterUserResponse(UserModel user)
        {
            User = user;
        }

        #endregion
    }
}
