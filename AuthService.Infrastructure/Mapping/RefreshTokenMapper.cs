namespace AuthService.Infrastructure.Mapping
{
    public static class RefreshTokenMapper
    {
        // Domain → Data
        public static AuthService.Infrastructure.Persistance.Entities.RefreshToken ToData(AuthService.Domain.Entities.RefreshToken domainToken)
        {
            if (domainToken == null) throw new ArgumentNullException(nameof(domainToken));

            return new AuthService.Infrastructure.Persistance.Entities.RefreshToken
            {
                Id = domainToken.Id,
                Token = domainToken.Token,
                Expires = domainToken.Expires,
                IsRevoked = domainToken.IsRevoked,
                CreatedAt = domainToken.CreatedAt,
                UpdatedAt = domainToken.UpdatedAt,
                IsDeleted = domainToken.IsDeleted,
                ApplicationUserId = domainToken.UserId,
                IpAddress = domainToken.IpAddress,
                UserAgent = domainToken.UserAgent,
                DeviceName = domainToken.DeviceName,
                RevokedAt = domainToken.RevokedAt,
                RevokedByIp = domainToken.RevokedByIp,
                ReplacedByToken = domainToken.ReplacedByToken,
                // Navigation property ApplicationUser repository tarafından yüklenir, burada maplemeye gerek yok
            };
        }

        // Data → Domain
        public static AuthService.Domain.Entities.RefreshToken ToDomain(AuthService.Infrastructure.Persistance.Entities.RefreshToken dataToken)
        {
            if (dataToken == null) throw new ArgumentNullException(nameof(dataToken));

            return new AuthService.Domain.Entities.RefreshToken
            {
                Id = dataToken.Id,
                Token = dataToken.Token,
                Expires = dataToken.Expires,
                IsRevoked = dataToken.IsRevoked,
                CreatedAt = dataToken.CreatedAt,
                UpdatedAt = dataToken.UpdatedAt,
                IsDeleted = dataToken.IsDeleted,
                UserId = dataToken.ApplicationUserId,
                User = dataToken.ApplicationUser != null ? UserMapper.ToDomain(dataToken.ApplicationUser) : null!,
                IpAddress = dataToken.IpAddress,
                UserAgent = dataToken.UserAgent,
                DeviceName = dataToken.DeviceName,
                RevokedAt = dataToken.RevokedAt,
                RevokedByIp = dataToken.RevokedByIp,
                ReplacedByToken = dataToken.ReplacedByToken,
            };
        }
    }

}
