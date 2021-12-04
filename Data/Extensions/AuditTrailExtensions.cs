using System;
using System.Collections.Generic;
using System.Linq;
using Data.Helpers;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Extensions
{
    public static class AuditTrailExtensions
    {
        public static void AddAuditTrailLogs(this DatabaseContext context, string userName)
        {
            var auditEntries = GetAuditEntries(context, userName);

            if (!auditEntries.Any())
                return;

            var logs = auditEntries.Select(x => x.ToAudit());
            context.AuditTrail.AddRange(logs);
        }

        public static void AddAuditTrailLogs(this DatabaseContext context, string userName, Guid id)
        {
            var auditEntries = GetAuditEntries(context, userName);

            if (!auditEntries.Any())
                return;

            var logs = auditEntries.Select(x => x.ToAudit(id));
            context.AuditTrail.AddRange(logs);
        }

        private static List<AuditTrailHelper> GetAuditEntries(DatabaseContext context, string userName)
        {
            context.ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditTrailHelper>();
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditTrail || entry.Entity is CommandAudit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var auditEntry = new AuditTrailHelper(entry, userName);
                auditEntries.Add(auditEntry);
            }
            return auditEntries;
        }
    }
}
