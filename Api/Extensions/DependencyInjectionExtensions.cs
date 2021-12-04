using Common;
using Common.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void ConfigureCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILoggedOnUserProvider, LoggedOnUser>();
        }
    }
}
