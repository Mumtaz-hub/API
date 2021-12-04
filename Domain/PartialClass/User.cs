using Common.Enum;
using Extensions;

namespace Domain.Entities
{
    public partial class User
    {
        public string FullName => $"{FirstName} {LastName}";
        public string UserRole => RoleType.ToEnumDescription();

    }
}
