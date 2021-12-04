using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Api.Redis.PublishSubscribe
{
    public class RedisSubscriber : BackgroundService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public RedisSubscriber(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = connectionMultiplexer.GetSubscriber();

            return subscriber.SubscribeAsync("Messages", (channel, value) =>
            {
                Console.WriteLine($"This is for testing Redis Subscribe => {value}");
            });
        }
    }
}
