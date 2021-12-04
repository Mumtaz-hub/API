using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public abstract class BaseEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TBase> entityTypeBuilder)
        {
            //Base Configuration
            entityTypeBuilder.Property(e => e.CreationUserId)
                .HasColumnType("varchar")
                .HasMaxLength(50);
        }
    }

}