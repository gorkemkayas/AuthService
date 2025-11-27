using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class UserRepository(UserManager<ApplicationUser> userManager,AuthDbContext context,IEntityMapper mapper) : GenericRepository<User, ApplicationUser, string>(context,mapper), IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<User?> GetByEmailAsync(string email)
        {
           var user = await _userManager.FindByEmailAsync(email);
           return (user is null) ? null : _mapper.MapToDomain<User,ApplicationUser>(user);

        }
        public async Task<int> GetCountByTenantIdAsync(int id) => await _context.Users.CountAsync(e => e.TenantId == id);
    }
}
