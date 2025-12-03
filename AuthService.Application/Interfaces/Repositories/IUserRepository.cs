using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IUserRepository :IGenericRepository<User,string>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<int> GetCountByTenantIdAsync(int id);
        Task<bool> DeleteUserById(string id);
    }
}
