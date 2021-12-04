using System.ComponentModel;

namespace Common.Enum
{
    public enum AddressType : byte
    {
        [Description("Primary Address")]
        PrimaryAddress = 1,

        [Description("Billing Address")]
        BillingAddress,

        [Description("Office Address")]
        OfficeAddress
    }
}
