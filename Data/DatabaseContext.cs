using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Interface;
using Data.Configurations;
using Data.Extensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data
{
    public class DatabaseContext : DbContext, IAuditDbContext
    {
        public DatabaseContext() { }

        public DatabaseContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<CommandAudit> CommandAudit { get; set; }
        public virtual DbSet<AuditTrail> AuditTrail { get; set; }

        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }


        public Task<int> SaveChangesAsync(string loggedOnUserId, CancellationToken cancellationToken)
        {
            this.AddAuditTrailLogs(loggedOnUserId);
            return base.SaveChangesAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync<T>(T commandMessage, CancellationToken cancellationToken) where T : Message
        {
            this.AddAuditTrailLogs(commandMessage.LoggedOnUserId.ToString(), commandMessage.MessageId);
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
      
            //modelBuilder.ApplyConfiguration(new UserConfigurations());
            // modelBuilder.ApplyConfiguration(new AddressConfigurations());
            //modelBuilder.ApplyConfiguration(new AuditTrailConfigurations());
           // modelBuilder.ApplyConfiguration(new CommandAuditConfigurations());
        }
    }
}
