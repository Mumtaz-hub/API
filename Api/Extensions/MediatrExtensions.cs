using System;
using Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Queries;

namespace Api.Extensions
{
    public static class MediatrExtensions
    {
        public static void AddMediatrWithHandlers(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var assembliesContainingMediatrHandlers = new[]
            {
                typeof(Query<>).Assembly,
                typeof(Command<>).Assembly
            };
            services.AddMediatR(assembliesContainingMediatrHandlers);
            //services.AddMediatR(typeof(Startup));
        }
    }
}
