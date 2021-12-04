using Api.Hangfire.MediatR.Infrastructure;
using Hangfire;
using MediatR;

namespace Api.Hangfire.MediatR.Extensions
{
    public static class MediatrExtensions
    {
        //public static void Enqueue<T>(this IMediator mediator, T request) where T : class
        //{
        //    BackgroundJob.Enqueue<MediatRHangfireBridge>(m => m.SendToHangfire(request));
        //}

        //public static void Enqueue<T>(this IMediator mediator, string jobName, T request) where T : class
        //{
        //    BackgroundJob.Enqueue<MediatRHangfireBridge>(m => m.SendToHangfire(jobName, request));
        //}

        public static void Enqueue<T>(this IMediator mediator, string jobName, T request) where T : class
        {
            var client = new BackgroundJobClient();
            client.Enqueue<MediatRHangfireBridge>(bridge => bridge.SendToHangfire(jobName, request));
        }

        public static void Enqueue<T>(this IMediator mediator, T request) where T : class
        {
            var client = new BackgroundJobClient();
            client.Enqueue<MediatRHangfireBridge>(bridge => bridge.SendToHangfire(request));
        }
    }
}