using AutoMapper;
using Domain.Entities;
using ViewModel.Audits;

namespace Api.MappingProfiles
{
    public class CommandAuditMapping : Profile
    {
        public CommandAuditMapping()
        {
            CreateMap<CommandAudit, CommandAuditViewModel>();
        }
    }
}
