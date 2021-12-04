using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewModel.Audits;

namespace Queries.Audits
{
    public class CommandAuditsQueryHandler : IRequestHandler<CommandAuditsQuery, IEnumerable<CommandAuditViewModel>>
    {
        private readonly DatabaseContext context;
        private readonly IMapper mapper;

        public CommandAuditsQueryHandler(DatabaseContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        public async Task<IEnumerable<CommandAuditViewModel>> Handle(CommandAuditsQuery request, CancellationToken cancellationToken)
        {
            var data = await context.CommandAudit
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return data.Select(mapper.Map<CommandAuditViewModel>).ToList();
        }
    }
}