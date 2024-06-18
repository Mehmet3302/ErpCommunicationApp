using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Erp___Kurum_Ici_Haberlesme.Models
{
    public class ExcludeRolesAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public ExcludeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user != null && _roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
