using Common;

namespace Commands.Users
{
    public class DeleteUserCommand : Command<Result<long>>
    {
        public DeleteUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
