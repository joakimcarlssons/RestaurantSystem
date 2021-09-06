using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.DTOs
{
    public class TokenResult
    {
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
