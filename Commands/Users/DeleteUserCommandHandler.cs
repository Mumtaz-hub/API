using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Caching.Extensions;
using Common;
using Common.Constants;
using Data;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Commands.Users
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<long>>
    {
        private readonly DatabaseContext context;

        public DeleteUserCommandHandler(DatabaseContext context)
        {
            this.context = context;
        }

        public async Task<Result<long>> Handle(DeleteUserCommand command, CancellationToken token)
        {
            var data = await context.User
                .SingleOrDefaultAsync(i => i.Id == command.Id, token);

            if (data == null)
                return new FailureResult<long>("No record found");

            if (data.IsSystemUser)
                return new FailureResult<long>($"You can't delete this user {data.EmailAddress}. Please contact administrator");

            context.User.Remove(data);
            await context.SaveChangesAsync(token);
            return new SuccessResult<long>(data.Id);
        }
    }

    public class DeleteUserCommandPostHandler : IRequestPostProcessor<DeleteUserCommand, Result<long>>
    {
        private readonly IDistributedCache cache;

        public DeleteUserCommandPostHandler(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task Process(DeleteUserCommand request, Result<long> response, CancellationToken cancellationToken)
        {
            if (response.IsSuccess)
            {
                var keys = new List<string>
                {
                    ApiRouteConstants.Users.GetUsersRoute,
                    ApiRouteConstants.Users.GetUserByIdRoute.Replace("{id}", response.Value.ToString())
                };
                await cache.RemoveKeyAsync(keys, cancellationToken);
            }
        }
    }
}
