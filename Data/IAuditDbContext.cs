using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public interface IAuditDbContext : IDisposable
    {
        DbSet<AuditTrail> AuditTrail { get; set; }
        DbSet<CommandAudit> CommandAudit { get; set; }

        Task<int> SaveChangesAsync(string loggedOnUserId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync<T>(T commandMessage, CancellationToken cancellationToken) where T : Message;
    }
}
