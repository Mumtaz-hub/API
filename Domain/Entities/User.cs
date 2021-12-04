using System.Collections.Generic;
using Common.Enum;

namespace Domain.Entities
{
    public partial class User : BaseEntity
    {
        public User()
        {
            Addresses = new HashSet<Address>();
        }

        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsAccountLocked { get; set; }
        public bool IsSystemUser { get; set; }
        public RoleType RoleType { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}
