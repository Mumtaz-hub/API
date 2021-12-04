using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Constants;
using Common.Interface;
using Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TimeZoneConverter;

namespace Common
{
    public class LoggedOnUser : ILoggedOnUserProvider
    {
        private readonly IEnumerable<Claim> claims;

        private const string DefaultTimeZone = ApplicationConstants.DefaultTimeZone;

        public long UserId { get; }
        public string Username { get; }
        public string FullName { get; }
        public long RoleId { get; }
        public string Email { get; }
        public string TimeZone { get; }
        public string ConnectionId { get; }
        public string[] Roles { get; }

        public string RequestUrl { get; }

        protected TimeZoneInfo timeZoneInstance;

        [JsonIgnore]
        public TimeZoneInfo TimeZoneInstance => timeZoneInstance ?? GetLoggedOnTimeZoneInfo();

        public LoggedOnUser(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
                return;

            claims = httpContextAccessor.HttpContext.User?.Claims;
            UserId = GetIntClaimTypeValue(ClaimConstants.UserId);
            Username = GetStringClaimTypeValue(ClaimConstants.Username);
            RoleId = GetIntClaimTypeValue(ClaimConstants.RoleId);
            FullName = GetStringClaimTypeValue(ClaimConstants.FullName);
            Email = GetStringClaimTypeValue(ClaimConstants.Email);
            TimeZone = GetTimeZoneClaimTypeValue(ClaimConstants.TimeZone);
            Roles = GetRoles();
           
            ConnectionId = GetStringClaimTypeValue(ClaimConstants.ConnectionId);
            RequestUrl = httpContextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        private TimeZoneInfo GetLoggedOnTimeZoneInfo()
        {
            var timeZone = TimeZone.HasValue() ? TimeZone : DefaultTimeZone;
            return TZConvert.GetTimeZoneInfo(timeZone);
        }

        private Guid GetGuidClaimTypeValue(string claimType)
        {
            var claim = claims?.FirstOrDefault(c => c.Type == claimType);
            return claim == null ? Guid.Empty : new Guid(claim.Value);
        }

        private long GetIntClaimTypeValue(string claimType)
        {
            var claim = claims?.FirstOrDefault(c => c.Type == claimType);
            return claim == null ? 0 : Convert.ToInt64(claim.Value);
        }

        private string GetStringClaimTypeValue(string claimType)
        {
            return claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
        }

        private string GetTimeZoneClaimTypeValue(string claimType)
        {
            return claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? DefaultTimeZone;
        }

        private string[] GetRoles()
        {
            return claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
        }

        public DateTime DisplayUserTimeFromUtc(DateTime? utcTime = null)
        {
            utcTime ??= DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime.Value, TimeZoneInstance);
        }

        public DateTime GetUserTimeAsUtc(DateTime localTime)
        {
            return SaveUserTimeAsUtc(localTime);
        }

        public DateTime SaveUserTimeAsUtc(DateTime localTime)
        {
            if (localTime.Kind == DateTimeKind.Utc)
            {
                return localTime;
            }

            var offset = TimeZoneInstance.GetUtcOffset(localTime).TotalHours;
            var offset2 = TimeZoneInfo.Local.GetUtcOffset(localTime).TotalHours;
            return localTime.AddHours(offset2 - offset).ToUniversalTime();
        }
    }
}