using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Infrastructure
{
    public class CommandHydratorAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            if (!(actionContext.HttpContext.RequestServices.GetService(typeof(ILoggedOnUserProvider)) is ILoggedOnUserProvider user))
                return;

            foreach (var cmd in actionContext.ActionArguments.Select(arg => arg.Value as Message))
                cmd?.SetUser(user);

            //To do : before the action executes  
            await next();
            //To do : after the action executes  
        }
    }
}
