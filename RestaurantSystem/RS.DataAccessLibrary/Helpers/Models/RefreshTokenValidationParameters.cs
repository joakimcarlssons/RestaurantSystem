using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.DataAccessLibrary.Helpers.Models
{
    /// <summary>
    /// A separate instance of <see cref="TokenValidationParameters"/> used for token validation if the token has expired and the refresh token endpoint is called
    /// </summary>
    public class RefreshTokenValidationParameters : TokenValidationParameters
    {
        /// <summary>
        /// Constructor with mapping to take values from the access token
        /// </summary>
        /// <param name="token">The <see cref="TokenValidationParameters"/> used for access tokens</param>
        public RefreshTokenValidationParameters(TokenValidationParameters token)
        {
            ValidateIssuerSigningKey = token.ValidateIssuerSigningKey;
            IssuerSigningKey = token.IssuerSigningKey;
            ValidateIssuer = token.ValidateIssuer;
            ValidateAudience = token.ValidateAudience;
            ValidIssuer = token.ValidIssuer;
            ValidAudience = token.ValidAudience;
            ClockSkew = token.ClockSkew;
            RequireExpirationTime = token.RequireExpirationTime;

            // Turn of lifetime validation when validating a token before refresh
            ValidateLifetime = false;
        }
    }
}
