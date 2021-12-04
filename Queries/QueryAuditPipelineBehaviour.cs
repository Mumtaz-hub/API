using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common.Interface;
using Data;
using ElmahCore;
using Extensions;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace Queries
{
    public class QueryAuditPipelineBehaviour<TRequest, T> : IPipelineBehavior<TRequest, T> where TRequest : Query<T> where T : class
    {
        private readonly DatabaseContext context;
        private readonly ILoggedOnUserProvider user;
        private readonly ILogger logger;
        private readonly Stopwatch stopwatch = new();

        public QueryAuditPipelineBehaviour(DatabaseContext context, ILoggedOnUserProvider user, ILogger logger)
        {
            this.context = context;

            this.user = user;
            this.logger = logger;
        }

        public async Task<T> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<T> next)
        {
            var exceptionMessage = string.Empty;
            stopwatch.Start();
            T result;
            try
            {
                result = await next();
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Demystify().Message;
                logger.Error(exceptionMessage);
                ElmahExtensions.RiseError(ex.Demystify());
                throw;
            }
            finally
            {
                stopwatch.Stop();
                if (request.AuditThisMessage)
                    await AuditQuery(request, stopwatch.Elapsed, exceptionMessage);
            }
            return result;
        }

        private async Task AuditQuery(TRequest query, TimeSpan timeTakenToExecuteCommand, string exceptionMessage)
        {
            try
            {
                var audit = new Domain.Entities.CommandAudit
                {
                    LoggedOnUserId = user?.UserId ?? 0,
                    UtcTimeStamp = DateTime.Now,
                    MessageId = query.MessageId,
                    IsSuccess = !exceptionMessage.HasValue(),
                    ExceptionMessage = exceptionMessage,
                    Type = query.GetType().Name,
                    Data = JsonConvert.SerializeObject(query),
                    Milliseconds = (int)timeTakenToExecuteCommand.TotalMilliseconds,
                    RequestUrl = user?.RequestUrl,
                };

                context.Add(audit);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Demystify().Message);
                ElmahExtensions.RiseError(ex.Demystify());
            }
        }
    }
}