using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Interface;
using Data;
using Domain.Entities;
using Extensions;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using ElmahCore;

namespace Commands
{
    public class CommandAuditPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : Command<TResponse> where TResponse : Result, new()
    {
        private readonly DatabaseContext context;
        private readonly Stopwatch stopwatch = new();
        private readonly ILoggedOnUserProvider user;
        private readonly ILogger logger;

        public CommandAuditPipelineBehaviour(DatabaseContext context, ILogger logger, ILoggedOnUserProvider user)
        {
            this.context = context;
            this.logger = logger;
            this.user = user;
        }

        public async Task<TResponse> Handle(TRequest cmd, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var result = new TResponse();
            stopwatch.Start();

            try
            {
                cmd.Result = result = await next();
            }
            catch (AutoMapperMappingException ex)
            {
                cmd.Result = new FailureResult(ex.Demystify().Message);
                result.Exception = ex.Demystify();
                result.SetFailures(cmd.Result.Failures);

                logger.Error(result.Exception.Message);
                ElmahExtensions.RiseError(result.Exception);
            }
            catch (DbUpdateException ex)
            {
                cmd.Result = new FailureResult(PopulateDataBaseExceptionInfo(ex));
                result.Exception = ex.Demystify();
                result.SetFailures(cmd.Result.Failures);

                logger.Error(result.Exception.Message);
                ElmahExtensions.RiseError(result.Exception);
            }
            catch (Exception ex)
            {
                cmd.Result = new FailureResult(ex.Demystify().Message);
                result.Exception = ex.Demystify();
                result.SetFailures(cmd.Result.Failures);

                logger.Error(result.Exception.Message);
                ElmahExtensions.RiseError(result.Exception);
            }
            finally
            {
                stopwatch.Stop();
                if (cmd.AuditThisMessage)
                    await AuditCommand(cmd, stopwatch.Elapsed);
            }

            return result;
        }

        private async Task AuditCommand(TRequest cmd, TimeSpan timeTakenToExecuteCommand)
        {
            try
            {
                var audit = new CommandAudit
                {
                    LoggedOnUserId = cmd.LoggedOnUserId,
                    MessageId = cmd.MessageId,
                    IsSuccess = cmd.Result?.IsSuccess ?? false,
                    ExceptionMessage = GetExceptionMessageFrom(cmd),
                    Type = cmd.GetType().Name,
                    Data = JsonConvert.SerializeObject(cmd),
                    Milliseconds = (int)timeTakenToExecuteCommand.TotalMilliseconds,
                    RequestUrl = user?.RequestUrl
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

        private static string GetExceptionMessageFrom(TRequest cmd)
        {
            return cmd.Result?.Exception?.Message ?? cmd.Result?.Failures?.JoinWithComma().TakeCharacters(10000) ?? string.Empty;
        }

        private static string PopulateDataBaseExceptionInfo(Exception exception)
        {
            var errorMessage = string.Empty;
            var rootException = exception.GetBaseException();

            var sqlException = rootException as SqlException;
            var exceptionMessage = rootException.Message;

            if (sqlException == null)
                return exceptionMessage;

            const string sqlErrorMessage = "Cannot insert duplicate record in {0}";

            // cannot insert duplicate record
            switch (sqlException.Number)
            {
                case 515:
                    errorMessage = exceptionMessage[..exceptionMessage.IndexOf(@"INSERT", StringComparison.Ordinal)].TrimAll();
                    break;
                case 547:
                    errorMessage = "The record cannot be deleted as the information has been used in other screens.";
                    break;
                case 2601:
                    var startPos = exceptionMessage.IndexOf(@"with unique index '", StringComparison.Ordinal);
                    var endPos = exceptionMessage.IndexOf(@"'.", startPos, StringComparison.Ordinal);
                    startPos += "with unique index '".Length;
                    var indexName = exceptionMessage.Substring(startPos, (endPos - startPos));
                    var qualifiedIndexName = indexName.Substring(indexName.IndexOf('_') + 1).Replace('_', ' ');
                    errorMessage = string.Format(sqlErrorMessage, qualifiedIndexName);
                    break;

            }
            return errorMessage;
        }
    }
}
