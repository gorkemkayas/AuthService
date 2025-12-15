using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class RefreshTokenRepository(AuthDbContext context, IEntityMapper mapper) : GenericRepository<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken, int>(context, mapper), IRefreshTokenRepository
    {
        public async Task<bool> DeleteRefreshTokenById(int id)
        {
            var refreshToken = await FindAsync(id);

            if (refreshToken is null)
                return false;

            refreshToken.IsDeleted = true;
            refreshToken.UpdatedAt = DateTime.UtcNow;
            return true;

        }
        public async override Task AddAsync(AuthService.Domain.Entities.RefreshToken entity, CancellationToken cancellationToken = default)
        {
            var mappedRefreshToken = _mapper.MapToData<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(entity);
            var result = await _context.RefreshTokens.AddAsync(mappedRefreshToken, cancellationToken);
        }
        public async Task<AuthService.Domain.Entities.RefreshToken?> FindByTokenAsync(string refreshToken, bool isTracked = true)
        {
            var query = _context.RefreshTokens.Where(rt => rt.Token == refreshToken).AsQueryable();
            if (!isTracked)
                query = query.AsNoTracking();

            var dbRefreshToken = await query.FirstOrDefaultAsync();
            if (dbRefreshToken is null) return null;

            return _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(dbRefreshToken);
        }
        public IEnumerable<AuthService.Domain.Entities.RefreshToken> GetActiveRefreshTokensByUserId(string userId, bool isTracked = true)
        {
            var query = _context.RefreshTokens
                .Where(rt => rt.ApplicationUserId == userId && !rt.IsRevoked && !rt.IsDeleted && rt.Expires > DateTime.UtcNow);
            if (!isTracked)
                query = query.AsNoTracking();
                
                
                var refreshTokens = query.AsEnumerable();

            var mappedRefreshTokens = _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(refreshTokens);
            return mappedRefreshTokens;
        }
        public async Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByUserDeviceAsync(string userId, string deviceName)
        {
            var refreshToken = await _context.RefreshTokens
                .Where(rt => rt.ApplicationUserId == userId && rt.DeviceName == deviceName && !rt.IsRevoked && !rt.IsDeleted && rt.Expires > DateTime.UtcNow).FirstOrDefaultAsync();

            if (refreshToken is null)
                return null;

            var mappedRefreshToken = _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(refreshToken);
            return mappedRefreshToken;
        }

        // Deprecated
        public async Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByDeviceAsync(string userId, string deviceName)
        {
            var lastRefreshToken = await _context.RefreshTokens.AsNoTracking()
                .Where(rt => rt.ApplicationUserId == userId
                          && rt.DeviceName == deviceName
                          && !rt.IsRevoked
                          && !rt.IsDeleted)
                .OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync();

            return lastRefreshToken is null ? null : _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(lastRefreshToken);
        }
        public async Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByClientTypeAsync(string userId, string clientType)
        {
            var lastRefreshToken = await _context.RefreshTokens.AsNoTracking()
                .Where(rt => rt.ApplicationUserId == userId
                          && rt.ClientType == clientType
                          && !rt.IsRevoked
                          && !rt.IsDeleted)
                .OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync();

            return lastRefreshToken is null ? null : _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(lastRefreshToken);
        }
        public async Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByDeviceIdAsync(string userId, string clientType, string deviceId)
        {
            var lastRefreshToken = await _context.RefreshTokens.AsNoTracking()
                .Where(rt => rt.ApplicationUserId == userId
                          && rt.ClientType == clientType
                          && rt.DeviceId == deviceId
                          && !rt.IsRevoked
                          && !rt.IsDeleted)
                .OrderByDescending(rt => rt.CreatedAt)
                .FirstOrDefaultAsync();

            return lastRefreshToken is null ? null : _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(lastRefreshToken);
        }

        public async Task<AuthService.Domain.Entities.RefreshToken?> GetActiveWebRefreshTokenAsync(string userId, bool isTracked = true)
        {
            var query = _context.RefreshTokens.Where(rt => rt.ApplicationUserId == userId && rt.IsRevoked == false).AsNoTracking();
            if (!isTracked)
                query = query.AsNoTracking();

            var result = await query.FirstOrDefaultAsync();

            if (result == null) return null;
            return _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken,AuthService.Infrastructure.Persistance.Entities.RefreshToken>(result);
        }
        
        public async Task<AuthService.Domain.Entities.RefreshToken?> GetActiveDeviceRefreshTokenAsync(string userId, string clientType, string deviceId, bool isTracked = true)
        {
            var query = _context.RefreshTokens.Where(rt => rt.ApplicationUserId == userId && rt.ClientType == clientType && rt.DeviceId == deviceId && rt.IsRevoked == false).AsQueryable();
            if (!isTracked)
                query = query.AsNoTracking();

            var result = await query.FirstOrDefaultAsync();
            if (result == null) return null;
            return _mapper.MapToDomain<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken>(result);
        }
    
    }
}