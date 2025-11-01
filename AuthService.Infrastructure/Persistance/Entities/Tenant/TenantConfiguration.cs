using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistance.Entities
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            // Name property
            builder.Property(t => t.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            // IsActive property
            builder.Property(t => t.IsActive)
                   .HasDefaultValue(true);

            // IsSystem property
            builder.Property(t => t.IsSystem)
                   .HasDefaultValue(false);

            // IBaseEntity properties
            builder.Property(t => t.CreatedAt)
                   .IsRequired();

            builder.Property(t => t.UpdatedAt)
                   .IsRequired(false);

            builder.Property(t => t.IsDeleted)
                   .HasDefaultValue(false);

            // ApplicationUsers navigation
            builder.HasMany(t => t.ApplicationUsers)
                   .WithOne(u => u.Tenant)
                   .HasForeignKey(u => u.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
