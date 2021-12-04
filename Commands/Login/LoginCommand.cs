using System.Collections.Generic;
using System.Security.Claims;
using Common;
using Newtonsoft.Json;

namespace Commands.Login
{
    public class LoginCommand : Command<Result<IList<Claim>>>
    {
        public string UserName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string Param { get; set; }
    }
}