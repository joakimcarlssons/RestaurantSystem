using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.DTOs.Responses
{
    /// <summary>
    /// Model for error responses
    /// </summary>
    public class ErrorResponse
    {
        #region Public Properties

        /// <summary>
        /// The response status code
        /// </summary>
        public short Code { get; set; }

        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializing constructor
        /// </summary>
        public ErrorResponse(short code, string message)
        {
            Code = code;
            Message = message;
        }

        #endregion
    }
}
