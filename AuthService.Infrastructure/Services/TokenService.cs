using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string _secret;
        private readonly string _issuer;

        public TokenService(IConfiguration _config, IUnitOfWork unitOfWork, IEntityMapper mapper)
        {
            _configuration = _config;
            _secret = _configuration["Jwt:Secret"]!;
            _issuer = _configuration["Jwt:Issuer"]!;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResult<CreateTenantUserTokenResponse>> CreateTenantUserTokenAsync(CreateTenantUserTokenRequest request)
        {
            var token = GenerateAccessToken(request.UserId, request.Email, request.TenantId, request.TenantDomain);
            var refreshToken = await RotateRefreshTokenAsync(request);

            return ServiceResult<CreateTenantUserTokenResponse>.Ok(new CreateTenantUserTokenResponse
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }

        public string CreateAdminToken(string adminId, string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, adminId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("role", "admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = GetSigningCredentials(_secret);
            var token = GetJwtSecurityToken(_issuer, _issuer, claims, DateTime.UtcNow.AddMinutes(30), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string CreateTenantToken(string userId, string email, string tenantId, string tenantDomain)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("tenantId", tenantId),
                new Claim("role", "tenant"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = GetSigningCredentials(_secret);
            var token = GetJwtSecurityToken(_issuer, tenantDomain, claims, DateTime.UtcNow.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public IEnumerable<RefreshToken> GetActiveRefreshTokensByUserId(string userId) => _unitOfWork.RefreshTokens.GetActiveRefreshTokensByUserId(userId);
        public async Task<RefreshToken?> GetActiveRefreshTokenByDeviceNameAsync(string userId, string deviceName) => await _unitOfWork.RefreshTokens.GetActiveRefreshTokenByUserDeviceAsync(userId, deviceName);
        private JwtSecurityToken GetJwtSecurityToken(string issuer, string audience, IEnumerable<Claim> claims, DateTime? expires, SigningCredentials credentials)
        {
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );
            return token;
        }
        private SigningCredentials GetSigningCredentials(string secretKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
        private string GenerateAccessToken(string userId, string email, int tenantId, string tenantDomain)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("tenantId", tenantId.ToString()),
                new Claim("role", "tenantUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = GetSigningCredentials(_secret);

            var jwtToken = GetJwtSecurityToken(_issuer, tenantDomain, claims, DateTime.UtcNow.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        private async Task<string> RotateRefreshTokenAsync(CreateTenantUserTokenRequest request)
        {
            var newRefreshToken = GenerateRefreshToken();

            var lastRefreshToken = await _unitOfWork.RefreshTokens
                .GetLastRefreshTokenByDeviceAsync(request.UserId, request.DeviceName);

            if (lastRefreshToken != null)
            {
                lastRefreshToken.IsRevoked = true;
                lastRefreshToken.RevokedAt = DateTime.UtcNow;
                lastRefreshToken.RevokedByIp = request.IpAddress;
                lastRefreshToken.ReplacedByToken = newRefreshToken;

                _unitOfWork.RefreshTokens.Update(lastRefreshToken);
            }

            await _unitOfWork.RefreshTokens.AddAsync(new Domain.Entities.RefreshToken
            {
                Token = newRefreshToken,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                IpAddress = request.IpAddress,
                DeviceName = request.DeviceName,
                UserAgent = request.UserAgent
            });

            await _unitOfWork.SaveChangesAsync();

            return newRefreshToken;
        }



    }
}
