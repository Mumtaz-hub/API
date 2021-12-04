using Common;
using Common.Enum;

namespace Commands.Users
{
    public class SaveUserCommand : Command<Result<long>>
    {
        public long Id { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string MobileNumber { get; set; }
        public RoleType? RoleType { get; set; }
    }
}