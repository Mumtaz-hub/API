using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Api.Redis.PublishSubscribe.Extensions
{
    public static class ConfigureServices
    {
        public static void AddConnectionMultiplexerDependency(this IServiceCollection services,  IConfigurationRoot configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

            services.AddHostedService<RedisSubscriber>();
        }
    }
}
