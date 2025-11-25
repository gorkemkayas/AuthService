using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistance.Entities;

namespace AuthService.Infrastructure.Mapping
{
    public static class UserMapper
    {
        // Domain → Data
        public static ApplicationUser ToData(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.Email,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                TenantId = user.TenantId,
                PasswordHash = user.PasswordHash,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted
            };
        }

        // Data → Domain
        public static User ToDomain(ApplicationUser applicationUser)
        {
            if (applicationUser == null) throw new ArgumentNullException(nameof(applicationUser));

            return new User
            {
                Id = applicationUser.Id,
                Name = applicationUser.Name,
                Surname = applicationUser.Surname,
                Email = applicationUser.Email,
                TenantId = applicationUser.TenantId,
                PasswordHash = applicationUser.PasswordHash,
                CreatedAt = applicationUser.CreatedAt,
                UpdatedAt = applicationUser.UpdatedAt,
                IsDeleted = applicationUser.IsDeleted
            };
        }
    }

}
