using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Data.Configurations
{
    public class UserConfigurations : BaseEntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> entityTypeBuilder)
        {
            ConfigureUserSchema(entityTypeBuilder);
            base.Configure(entityTypeBuilder);
        }

        private static void ConfigureUserSchema(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(e => e.MobileNumber).IsUnique();
            builder.HasIndex(e => e.EmailAddress).IsUnique();

            builder.Property(e => e.EmailAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Password)
                .IsRequired();

            builder.Property(e => e.MobileNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(e => e.Addresses)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
