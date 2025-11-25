using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class UserRepository(UserManager<ApplicationUser> userManager,AuthDbContext context,IEntityMapper mapper) : GenericRepository<User, ApplicationUser, string>(context,mapper), IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
    }
}
