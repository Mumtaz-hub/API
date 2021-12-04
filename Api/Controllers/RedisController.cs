using System.Threading.Tasks;
using Commands.Users;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Api.Controllers
{
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public RedisController(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
        }

        [HttpPost]
        [Route("api/redis/publish")]
        public async Task SaveUser([FromBody] SaveUserCommand command)
        {
            const string publishChannel = "Messages";
            var connection = connectionMultiplexer.GetSubscriber();
            await connection.PublishAsync(publishChannel, command.ToJson());
        }
    }
}
