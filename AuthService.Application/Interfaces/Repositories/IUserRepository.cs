using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IUserRepository :IGenericRepository<User, User, string>
    {
        public Task<User?> GetByEmailAsync(string email);
        public Task<int> GetCountByTenantIdAsync(int id);
    }
}
