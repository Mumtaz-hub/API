using System;
using System.Linq;
using Common.Enum;

namespace Api.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params RoleType[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
