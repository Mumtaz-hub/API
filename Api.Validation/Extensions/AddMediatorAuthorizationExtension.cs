using Api.Behaviors.Authorization.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Behaviors.Authorization.Extensions
{
    public static class AddMediatorAuthorizationExtension
    {
        public static IServiceCollection AddMediatorAuthorization(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestAuthorizationBehavior<,>));
            return services;
        }
    }
}
