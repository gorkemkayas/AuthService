using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Entities
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            // Primary Key
            builder.HasKey(ar => ar.Id);

            // Description property
            builder.Property(r => r.Description)
                   .HasMaxLength(250)
                   .IsRequired(false);    

            // IBaseEntity properties
            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.Property(r => r.UpdatedAt)
                   .IsRequired(false);

            builder.Property(r => r.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
