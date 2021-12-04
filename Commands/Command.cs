using Common;
using Common.Interface;
using MediatR;
using Newtonsoft.Json;

namespace Commands
{
    //Base class used to indicate to the mediatr pipeline that this is a command and this needs to be auditted.
    public abstract class Command<TResponse> : Message, IRequest<TResponse>
    {
        [JsonIgnore]
        public IResult Result { get; set; }
    }
}