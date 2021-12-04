using Common.Enum;

namespace Domain.Entities
{
    public partial class Address : BaseEntity
    {
        public AddressType? AddressType { get; set; }
        public string Company { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
