
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Security
{
    public class RequireConfirmedEmailAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var user = userManager.GetUserAsync(context.HttpContext.User).Result;

            if (user == null || !user.EmailConfirmed)
            {
                context.Result = new ForbidResult();
            }
        }
    }

}
