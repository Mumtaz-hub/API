using Api.Caching.Caching;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Caching.Extensions
{
    public static class ConfigureServices
    {
        public static void AddCachingBehavior(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            //  Microsoft.Extensions.Caching.Memory
            //services.AddDistributedMemoryCache();

            services.AddStackExchangeRedisCache(options =>
              {
                  options.Configuration = configuration.GetConnectionString("Redis");
                  options.InstanceName = $"{configuration["AppSettings:Environment"]}_";
              });
        }
    }
}
