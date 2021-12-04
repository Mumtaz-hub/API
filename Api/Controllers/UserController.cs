using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Extensions;
using Api.Infrastructure;
using Commands.Users;
using Common.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Queries.User;
using ViewModel.Users;

namespace Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediatr;
        private readonly ILoggedOnUserProvider user;

        public UserController(IMediator mediatr, ILoggedOnUserProvider user)
        {
            this.mediatr = mediatr;
            this.user = user;
        }

        [HttpPost]
        [Route("api/user")]
        public async Task<IActionResult> SaveUser([FromBody] SaveUserCommand command)
        {
            var result = await mediatr.Send(command);
            return result.ToActionResult();
        }

        [HttpGet]
        [Route("api/users")]
        //[Authorize]
        //[AuthorizeRole(RoleType.SuperAdmin, RoleType.Admin)]
        public async Task<IEnumerable<UsersViewModel>> GetUsers(CancellationToken cancellationToken)
        {
            return await mediatr.Send(new UsersQuery(), cancellationToken);
        }

        [HttpGet]
        [Route("api/user/{id}")]
        //[Authorize]
        public async Task<UserViewModel> GetUser(long id)
        {
            return await mediatr.Send(new UserQuery(id));
        }

        [HttpDelete]
        [Route("api/user/{id}")]
        //[AuthorizeRole(RoleType.SuperAdmin, RoleType.Admin)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var result = await mediatr.Send(new DeleteUserCommand(id));
            return result.ToActionResult();
        }

    }
}