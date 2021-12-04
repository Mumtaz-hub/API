using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Behaviors.Authorization.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
