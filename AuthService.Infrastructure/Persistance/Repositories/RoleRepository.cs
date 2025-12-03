using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class RoleRepository(RoleManager<ApplicationRole> roleManager, AuthDbContext context, IEntityMapper mapper) : GenericRepository<Domain.Entities.Role, ApplicationRole, string>(context, mapper), IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        public async Task<Role?> GetByNameAsync(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            return (role is null) ? null : _mapper.MapToDomain<Role, ApplicationRole>(role);
        }

        public async Task<Role?> GetByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return (role is null) ? null : _mapper.MapToDomain<Role, ApplicationRole>(role);
        }

        public async Task<IList<Role>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(role => _mapper.MapToDomain<Role, ApplicationRole>(role)).ToList();
        }

        public async Task<bool> CreateRoleAsync(Role role)
        {
            var applicationRole = _mapper.MapToData<Role, ApplicationRole>(role);
            var result = await _roleManager.CreateAsync(applicationRole);
            if(!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            var role = await FindAsync(id);
            if (role == null) 
                return false;

            role.IsDeleted = true;
            role.UpdatedAt = DateTime.UtcNow;

            return true;
        }

    }
}
