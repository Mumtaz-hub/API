using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;

namespace Api.Hangfire.MediatR.Infrastructure
{
    public class MediatRHangfireBridge
    {
        private readonly IMediator mediator;
      
        public MediatRHangfireBridge(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task SendToHangfire<T>(T request) where T : class
        {
            await mediator.Send(request);
        }

        [DisplayName("{0}")]
        public async Task SendToHangfire<T>(string jobName, T request) where T : class
        {
            await mediator.Send(request);
        }

        //public async Task Send(IRequest command)
        //{
        //    await mediator.Send(command);
        //}

        //[DisplayName("{0}")]
        //public async Task Send<T>(string jobName, T request) where T : class
        //{
        //    await mediator.Send(request);
        //}
    }
}
