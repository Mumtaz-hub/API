using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Constants;
using Common.Enum;
using Data;
using Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<IList<Claim>>>
    {
        private readonly ILogger logger;
        private readonly DatabaseContext context;

        public LoginCommandHandler(DatabaseContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Result<IList<Claim>>> Handle(LoginCommand command, CancellationToken token)
        {
            var userNameUpperCase = command.UserName.ToUpper();
            var hashedPassword = command.Password.ToPasswordSha256Hash();

            var loginResult = await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(i => string.Equals(i.EmailAddress.ToUpper(), userNameUpperCase)
                                          && string.Equals(i.Password, hashedPassword)
                                          && i.Status == StatusType.Enabled, token);

            if (loginResult == null)
            {
                logger.Error("Invalid username or password - {@LoginCommand} ", command);
                return new FailureResult<IList<Claim>>("Invalid username or password");
            }

            if (!loginResult.IsEmailVerified)
            {
                logger.Error("Email Address is not verified");
                return new FailureResult<IList<Claim>>("Email Address is not verified");
            }

            var claims = new List<Claim>
            {
                new(ClaimConstants.UserId,  loginResult.Id.ToString()),
                new(ClaimConstants.FullName, loginResult.FullName),
                new(ClaimConstants.Email, loginResult.EmailAddress),
                new(ClaimConstants.Username, loginResult.EmailAddress),
                new(ClaimConstants.RoleId, Convert.ToInt32(loginResult.RoleType).ToString()),
                new(ClaimTypes.Role, loginResult.RoleType.ToString())
            };

            return new SuccessResult<IList<Claim>>(claims);
            // Credentials are invalid, or account doesn't exist
        }
    }
}