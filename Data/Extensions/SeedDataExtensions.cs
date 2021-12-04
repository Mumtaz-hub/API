using System;
using System.Linq;
using Common.Enum;
using Domain.Entities;
using Extensions;

namespace Data.Extensions
{
    public static class SeedDataExtensions
    {
        public static void EnsureSeeded(this DatabaseContext context)
        {
            SeedUser(context);
        }

        private static void SeedUser(DatabaseContext context)
        {
            if (context.User.Any())
                return;

            context.User.Add(new User
            {
                FirstName = "Mumtaz",
                LastName = "Shaikh",
                EmailAddress = "mghata@gmail.com",
                Password = "123456".ToPasswordSha256Hash(),
                MobileNumber = "919898933180",
                CreationTs = DateTime.UtcNow,
                CreationUserId = "System",
                LastChangeTs = DateTime.UtcNow,
                LastChangeUserId = "System",
                Status = StatusType.Enabled,
                IsEmailVerified = true,
                IsAccountLocked = false,
                IsSystemUser = true,
                RoleType = RoleType.SuperAdmin
            });

            context.SaveChanges();
        }
    }
}
