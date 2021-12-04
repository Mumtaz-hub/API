using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class AddressConfigurations : BaseEntityTypeConfiguration<Address>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            ConfigureAddressSchema(builder);
            base.Configure(builder);
        }

        private static void ConfigureAddressSchema(EntityTypeBuilder<Address> builder)
        {
            builder.Property(e => e.UserId)
                .IsRequired();

            builder.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Number)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(e => e.Street)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.City)
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}
