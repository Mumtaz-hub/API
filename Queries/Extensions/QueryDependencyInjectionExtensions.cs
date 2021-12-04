using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Queries.Extensions
{
    public static class QueryDependencyInjectionExtensions
    {
        public static void ConfigureDependecyInjectionForQueryPipelineBehaviours(this IServiceCollection services)
        {
            //Pipelines are executed in the order in which they are registered:
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof( RequestAuthorizationBehavior<,>));
            //services.AddMediatorAuthorization(Assembly.GetExecutingAssembly());
            //services.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddTransient(typeof(IAuthorizationRequirement), typeof(MustHaveCourseSubscriptionRequirement));
 
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(QueryAuditPipelineBehaviour<,>));
        }
    }
}
