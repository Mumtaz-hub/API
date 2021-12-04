using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Caching.Extensions;
using AutoMapper;
using Common;
using Common.Constants;
using Data;
using Data.Extensions;
using Extensions;
using Hangfire.MediatR;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Commands.Users
{
    public class SaveUserCommandHandler : IRequestHandler<SaveUserCommand, Result<long>>
    {
        private readonly DatabaseContext context;
        private readonly IMapper mapper;
        private readonly IMediator mediatr;

        public SaveUserCommandHandler(DatabaseContext context, IMapper mapper, IMediator mediatr)
        {
            this.context = context;
            this.mapper = mapper;
            this.mediatr = mediatr;
        }

        public async Task<Result<long>> Handle(SaveUserCommand command, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.User();
            if (command.Id > 0)
            {
                entity = await context.User.SingleOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

                if (entity == null)
                    return new FailureResult<long>("Could not find user information");
            }

            mapper.Map(command, entity);
            entity.Password = entity.Password.ToPasswordSha256Hash();

            entity.PopulateMetaData(command.LoggedOnUserId.ToString());

            if (command.Id == 0)
                await context.User.AddAsync(entity, cancellationToken);

            await context.SaveChangesAsync(command, cancellationToken);
            return new SuccessResult<long>(entity.Id);
        }
    }


    public class SaveUserCommandPostHandler : IRequestPostProcessor<SaveUserCommand, Result<long>>
    {
        private readonly IDistributedCache cache;
        private readonly IMediator mediatr;

        public SaveUserCommandPostHandler(IDistributedCache cache, IMediator mediatr)
        {
            this.cache = cache;
            this.mediatr = mediatr;
        }

        public async Task Process(SaveUserCommand request, Result<long> response, CancellationToken cancellationToken)
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


    public class PlaceOrder : IRequest
    {
        public long OrderId { get; set; }
    }


    public class PlaceOrderHandler : IRequestHandler<PlaceOrder>
    {
        public Task<Unit> Handle(PlaceOrder request, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
           return Task.FromResult(Unit.Value);
        }
    }

}