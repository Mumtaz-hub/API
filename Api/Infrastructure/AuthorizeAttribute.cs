using System;
using System.Linq;
using Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var connectionClaim = context.HttpContext.User.Claims.FirstOrDefault(i => i.Type == ClaimConstants.ConnectionId);

            if (connectionClaim == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            if (!context.HttpContext.Connection.Id.Equals(connectionClaim.Value))
            {
                //Generated Token from different client
                context.Result = new JsonResult(new { message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}