using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Entities
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Token property
            builder.Property(r => r.Token)
                   .HasMaxLength(500)
                   .IsRequired();

            // Expires property
            builder.Property(r => r.Expires)
                   .IsRequired();

            // IsRevoked property
            builder.Property(r => r.IsRevoked)
                   .HasDefaultValue(false);

            // IBaseEntity properties
            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.Property(r => r.UpdatedAt)
                   .IsRequired(false);

            builder.Property(r => r.IsDeleted)
                   .HasDefaultValue(false);

            // User relationship
            builder.HasOne(r => r.ApplicationUser)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(r => r.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
