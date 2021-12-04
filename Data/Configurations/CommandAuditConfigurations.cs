using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class CommandAuditConfigurations : IEntityTypeConfiguration<CommandAudit>
    {
        public void Configure(EntityTypeBuilder<CommandAudit> builder)
        {
            ConfigureCommandAuditSchema(builder);
        }

        private static void ConfigureCommandAuditSchema(EntityTypeBuilder<CommandAudit> builder)
        {
            builder.Property(e => e.ExceptionMessage)
                .HasColumnType("varchar(max)");

            builder.Property(e => e.Type)
                .HasColumnType("varchar")
                .HasMaxLength(100);

            builder.Property(e => e.RequestUrl)
                .HasColumnType("varchar")
                .HasMaxLength(200);
        }
    }
}
