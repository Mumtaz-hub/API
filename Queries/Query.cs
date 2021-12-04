using Common;
using Common.Interface;
using MediatR;

namespace Queries
{
    public abstract class Query<T> : Message, IRequest<T>
    {
        protected Query()
        {
        }

        protected Query(ILoggedOnUserProvider user)
        {
            SetUser(user);
        }

        protected Query(Message parentMessage) : this(parentMessage.User)
        {
            MessageId = parentMessage.MessageId;
        }
    }
}
