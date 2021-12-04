using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Behaviors.Authorization.Interfaces;
using MediatR;

namespace Queries.User
{
    //public class MustHaveCourseSubscriptionRequirement : IAuthorizationRequirement
    //{
    //    public long UserId { get; set; }
    //}

    //public class MustHaveCourseSubscriptionRequirementHandler : IAuthorizationHandler<MustHaveCourseSubscriptionRequirement>
    //{
    //    private readonly DatabaseContext context;

    //    public MustHaveCourseSubscriptionRequirementHandler(DatabaseContext context)
    //    {
    //        this.context = context;
    //    }

    //    public async Task<AuthorizationResult> Handle(MustHaveCourseSubscriptionRequirement request, CancellationToken cancellationToken)
    //    {
    //        var userCourseSubscription = await context.User
    //            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

    //        return userCourseSubscription != null
    //            ? AuthorizationResult.Succeed()
    //            : AuthorizationResult.Fail("You don't have a subscription to this course.");
    //    }

    //}


    public class RequestAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;

        public RequestAuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers)
        {
            _authorizers = authorizers;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            foreach (var authorizer in _authorizers)
            {

                //var result = await authorizer.Requirements
                //if (!result.IsAuthorized)
                //    throw new UnauthorizedException(result.FailureMessage);
            }

            return await next();
        }
    }
}
