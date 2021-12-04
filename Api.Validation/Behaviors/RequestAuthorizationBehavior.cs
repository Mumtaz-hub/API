using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Api.Behaviors.Authorization.Exceptions;
using Common.Interface;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Behaviors.Authorization.Behaviors
{
    public class RequestAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILoggedOnUserProvider user;
        private readonly DatabaseContext context;

        public RequestAuthorizationBehavior(ILoggedOnUserProvider user, DatabaseContext context)
        {
            this.user = user;
            this.context = context;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if(!await IsValidUser(cancellationToken))
                throw new UnauthorizedException("Un Authorized");

            return await next();
        }

        private async Task<bool> IsValidUser(CancellationToken cancellationToken)
        {
            if (user.UserId == 0)
                return true;

            return await context.User
                .AnyAsync(x => x.Id == user.UserId, cancellationToken);
        }
    }
}
