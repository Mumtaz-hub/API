using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;

namespace Hangfire.MediatR
{
    public class MediatRHangfireBridge
    {
        private readonly IMediator _mediator;

        public MediatRHangfireBridge(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Send<T>(T command) where T : class
        {
            await _mediator.Send(command);
        }

        [DisplayName("{0}")]
        public async Task Send<T>(string jobName, T command) where T : class
        {
            await _mediator.Send(command);
        }
    }
}