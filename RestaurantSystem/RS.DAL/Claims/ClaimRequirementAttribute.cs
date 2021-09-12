using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RS.DAL.Claims
{
    /// <summary>
    /// An attribute holding specific claim requirement for authorization
    /// </summary>
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="claimType">The type of claim</param>
        /// <param name="claimValue">The value of the claim</param>
        public ClaimRequirementAttribute(ClaimTypes claimType, string claimValue) : base(typeof(ClaimRequirementAttributeFilter))
        {
            Arguments = new object[] { new Claim(claimType.ToString(), claimValue) };
        }
    }

    /// <summary>
    /// The filter authorizing the user against the given claim
    /// </summary>
    public class ClaimRequirementAttributeFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="claim">the injected claim</param>
        public ClaimRequirementAttributeFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if the user has the claim
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);

            // If not return status code 403 - Forbidden
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
