using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Entities
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            // Name and Surname properties
            builder.Property(u => u.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Surname)
                   .HasMaxLength(100)
                   .IsRequired();

            // Tenant relationship
            builder.HasOne(u => u.Tenant)
                   .WithMany(t => t.ApplicationUsers)
                   .HasForeignKey(u => u.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);

            // IBaseEntity properties
            builder.Property(u => u.CreatedAt)
                   .IsRequired();

            builder.Property(u => u.UpdatedAt)
                   .IsRequired(false);

            builder.Property(u => u.IsDeleted)
                   .HasDefaultValue(false);

            // RefreshTokens navigation
            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(r => r.ApplicationUser)
                   .HasForeignKey(r => r.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
