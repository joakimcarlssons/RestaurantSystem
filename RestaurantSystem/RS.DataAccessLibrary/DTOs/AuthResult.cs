using RS.DataAccessLibrary.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.DTOs
{
    public class AuthResult
    {
        /// <summary>
        /// The error if any, containing the error code and the error message
        /// </summary>
        public ErrorResponse Error { get; set; }

        /// <summary>
        /// The token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
