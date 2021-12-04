using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Extensions
{
    public static class CommandDependencyInjectionExtensions
    {
        public static void ConfigureDependecyInjectionForCommandPipelineBehaviours(this IServiceCollection services)
        {
            //Pipelines are executed in the order in which they are registered:

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandAuditPipelineBehaviour<,>));
        }
    }
}
