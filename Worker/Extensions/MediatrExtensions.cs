using System;
using Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
 

namespace Worker.Extensions
{
    public static class MediatrExtensions
    {
        public static void AddMediatrWithHandlers(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var assembliesContainingMediatrHandlers = new[]
            {
                typeof(Command<>).Assembly
            };
           services.AddMediatR(assembliesContainingMediatrHandlers);
        }
    }
}
