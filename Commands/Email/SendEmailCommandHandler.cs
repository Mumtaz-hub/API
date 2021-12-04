using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MediatR;

namespace Commands.Email
{
    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result>
    {
        private readonly EmailSettings emailSettings;


        public SendEmailCommandHandler(EmailSettings emailSettings)
        {
            this.emailSettings = emailSettings;
        }

        public async Task<Result> Handle(SendEmailCommand command, CancellationToken token)
        {
            using var smtp = new SmtpClient(emailSettings.SmtpServer)
            {
                Port = emailSettings.SmtpPort, 
                EnableSsl = emailSettings.SmtpEnableSsl, 
                Credentials = new NetworkCredential(emailSettings.SmtpUsername, emailSettings.SmtpPassword),
            };
            
            try
            {
                await smtp.SendMailAsync(command.MailMessage, token);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                return new FailureResult(e.Message);
            }
        }
    }
}
