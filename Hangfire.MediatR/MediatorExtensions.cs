using MediatR;

namespace Hangfire.MediatR
{
    public static class MediatorExtensions
    {
        public static void Enqueue<T>(this IMediator mediator, T request) where T : class
        {
            //var client = new BackgroundJobClient();
            //client.Enqueue<MediatRHangfireBridge>(bridge => bridge.Send(jobName, request));
            BackgroundJob.Enqueue<MediatRHangfireBridge>(m => m.Send(request));
        }

        public static void Enqueue<T>(this IMediator mediator, string jobName, T request) where T : class
        {
            //var client = new BackgroundJobClient();
            //client.Enqueue<MediatRHangfireBridge>(bridge => bridge.Send(request));
            BackgroundJob.Enqueue<MediatRHangfireBridge>(m => m.Send(jobName, request));
        }


        public static void Enqueue(this IMediator mediator, string jobName, IRequest request)
        {
            var client = new BackgroundJobClient();
            client.Enqueue<MediatRHangfireBridge>(bridge => bridge.Send(jobName, request));
        }

        public static void Enqueue(this IMediator mediator, IRequest request)
        {
            var client = new BackgroundJobClient();
            client.Enqueue<MediatRHangfireBridge>(bridge => bridge.Send(request));
        }
    }
}