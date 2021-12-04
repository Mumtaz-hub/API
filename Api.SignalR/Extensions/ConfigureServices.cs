using Microsoft.Extensions.DependencyInjection;

namespace Api.SignalR.Extensions
{
    public static class ConfigureServices
    {
        public static void AddSignalRDependency(this IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}
