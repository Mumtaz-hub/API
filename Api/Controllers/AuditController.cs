using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Queries.Audits;
using ViewModel.Audits;

namespace Api.Controllers
{
    [ApiController]
    public class AuditController : ControllerBase 
    {
        private readonly IMediator mediatr;

        public AuditController(IMediator mediatr)
        {
            this.mediatr = mediatr;
        }

        [HttpGet]
        [Route("api/audits")]
        public async Task<IEnumerable<CommandAuditViewModel>> GetCommandAudits()
        {
            return await mediatr.Send(new CommandAuditsQuery());
        }
    }
}