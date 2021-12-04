using System;

namespace Common.Interface
{
    public interface ILoggedOnUserProvider
    {
        long UserId { get; }
        string Username { get; }
        string FullName { get; }
        string Email { get; }
        string TimeZone { get; }
        string RequestUrl { get; }
        string ConnectionId { get; }
        string[] Roles { get; }

        TimeZoneInfo TimeZoneInstance { get; }
        DateTime DisplayUserTimeFromUtc(DateTime? utcTime = null);
        DateTime SaveUserTimeAsUtc(DateTime localTime);
        DateTime GetUserTimeAsUtc(DateTime localTime);
    }
}
