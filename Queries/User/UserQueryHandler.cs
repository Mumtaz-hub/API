using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewModel.Users;

namespace Queries.User
{
    public class UserQueryHandler : IRequestHandler<UserQuery, UserViewModel>
    {
        private readonly DatabaseContext context;
        private readonly IMapper mapper;

        public UserQueryHandler(DatabaseContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UserViewModel> Handle(UserQuery query, CancellationToken cancellationToken)
        {
            var data = await context.User
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == query.Id, cancellationToken);

            return mapper.Map<UserViewModel>(data);
        }
    }
}