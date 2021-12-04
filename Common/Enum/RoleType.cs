using System.ComponentModel;

namespace Common.Enum
{
    public enum RoleType : byte
    {
        [Description("Super Admin")]
        SuperAdmin = 1,

        [Description("Admin")]
        Admin,

        [Description("Leader")]
        Leader,

        [Description("Participant")]
        Participant,

        [Description("Sponsors")]
        Sponsors,

        [Description("Developer")]
        Developer,

        [Description("Manager")]
        Manager,

        [Description("CEO")]
        Ceo,
    }
}
