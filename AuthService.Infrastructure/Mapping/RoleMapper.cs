using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistance.Entities;

namespace AuthService.Infrastructure.Mapping
{
    public static class RoleMapper
    {
        // Domain → Data
        public static ApplicationRole ToData(Role domainRole)
        {
            if (domainRole == null) throw new ArgumentNullException(nameof(domainRole));

            return new ApplicationRole
            {
                Id = domainRole.Id,
                Name = domainRole.Name,
                NormalizedName = domainRole.Name.ToUpperInvariant(),
                CreatedAt = domainRole.CreatedAt,
                UpdatedAt = domainRole.UpdatedAt,
                IsDeleted = domainRole.IsDeleted
            };
        }

        // Data → Domain
        public static Role ToDomain(ApplicationRole dataRole)
        {
            if (dataRole == null) throw new ArgumentNullException(nameof(dataRole));

            return new Role
            {
                Id = dataRole.Id,
                Name = dataRole.Name,
                CreatedAt = dataRole.CreatedAt,
                UpdatedAt = dataRole.UpdatedAt,
                IsDeleted = dataRole.IsDeleted
            };
        }
    }


}
