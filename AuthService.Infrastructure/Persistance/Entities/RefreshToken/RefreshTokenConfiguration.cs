using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistance.Entities
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

            // new update
            builder.Property(r => r.IpAddress)
                .HasMaxLength(100);

            builder.Property(r => r.UserAgent)
                .HasMaxLength(500);

            builder.Property(r => r.DeviceName)
                .HasMaxLength(200);

            builder.Property(r => r.RevokedAt);

            builder.Property(r => r.RevokedByIp)
                .HasMaxLength(100);

            builder.Property(r => r.ReplacedByToken)
                .HasMaxLength(500);
        }
    }
}
