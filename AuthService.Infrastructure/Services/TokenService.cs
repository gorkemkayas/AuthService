using AuthService.Application.Common;
using AuthService.Application.Dtos.Refresh;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Shared.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenService> _logger;
        private readonly IEntityMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly TokenOptions _tokenOptions;
        private readonly string _secret;
        private readonly string _issuer;

        public TokenService(IConfiguration _config, IUnitOfWork unitOfWork, IEntityMapper mapper, IOptions<TokenOptions> options, ILogger<TokenService> logger)
        {
            _configuration = _config;
            _secret = _configuration["Jwt:Secret"]!;
            _issuer = _configuration["Jwt:Issuer"]!;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenOptions = options.Value;
            _logger = logger;
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
            var token = GetJwtSecurityToken(_issuer, Audiences.AuthService, claims, DateTime.UtcNow.AddMinutes(30), creds);

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
            var token = GetJwtSecurityToken(_issuer, Audiences.TenantApi, claims, DateTime.UtcNow.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public IEnumerable<RefreshToken> GetActiveRefreshTokensByUserId(string userId) => _unitOfWork.RefreshTokens.GetActiveRefreshTokensByUserId(userId);
        public async Task<RefreshToken?> GetActiveRefreshTokenByDeviceNameAsync(string userId, string deviceName) => await _unitOfWork.RefreshTokens.GetActiveRefreshTokenByUserDeviceAsync(userId, deviceName);
        public DateTime GetRefreshTokenExpiryByClient(string clientType)
        {
            return clientType switch
            {
                ClientTypes.Web =>
                    DateTime.UtcNow.AddDays(_tokenOptions.WebRefreshTokenLifetimeDays),

                ClientTypes.Mobile =>
                    DateTime.UtcNow.AddDays(_tokenOptions.MobileRefreshTokenLifetimeDays),

                _ =>
                    DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenLifetimeDays)
            };
        }
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

            var jwtToken = GetJwtSecurityToken(_issuer, Audiences.TenantApi, claims, DateTime.UtcNow.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        private async Task<string> RotateRefreshTokenAsync(CreateTenantUserTokenRequest request)
        {
            var newRefreshToken = GenerateRefreshToken();

            RefreshToken? lastRefreshToken;

            if (request.ClientType == ClientTypes.Web)
                lastRefreshToken = await _unitOfWork.RefreshTokens.GetActiveRefreshTokenByClientTypeAsync(request.UserId, ClientTypes.Web);
            else
            {
                lastRefreshToken = await _unitOfWork.RefreshTokens.GetActiveRefreshTokenByDeviceIdAsync(request.UserId, request.ClientType, request.DeviceId!);
            }


            if (lastRefreshToken != null)
            {
                lastRefreshToken.IsRevoked = true;
                lastRefreshToken.RevokedAt = DateTime.UtcNow;
                lastRefreshToken.UpdatedAt = DateTime.UtcNow;        ////// domain entitye ClientType ve DeviceId eklenecek, mappingi yapılıp db ye aktarımda proplar güncellenecek.
                lastRefreshToken.RevokedByIp = request.IpAddress;
                lastRefreshToken.ReplacedByToken = newRefreshToken;
                lastRefreshToken.ClientType = request.ClientType;
                lastRefreshToken.DeviceId = request.DeviceId;

                _unitOfWork.RefreshTokens.Update(lastRefreshToken);
            }

            await _unitOfWork.RefreshTokens.AddAsync(new Domain.Entities.RefreshToken
            {
                Token = newRefreshToken,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                Expires = GetRefreshTokenExpiryByClient(request.ClientType),
                IpAddress = request.IpAddress,
                DeviceName = request.DeviceName,
                UserAgent = request.UserAgent,
                ClientType = request.ClientType,
                DeviceId = request.DeviceId,
            });

            await _unitOfWork.SaveChangesAsync();

            return newRefreshToken;
        }

        public async Task<ServiceResult<RefreshResponse>> RefreshAsync(string? refreshToken, AuditInfo clientInformations, string clientType, string? deviceId)
        {
            if (string.IsNullOrEmpty(refreshToken)) return ServiceResult<RefreshResponse>.Fail("Refresh Token Required", ErrorCodes.ValidationError);

            var dbRefreshToken = await _unitOfWork.RefreshTokens.FindByTokenAsync(refreshToken, false);

            if (dbRefreshToken == null) return ServiceResult<RefreshResponse>.Fail("Invalid refresh token.", ErrorCodes.Unauthorized);

            if (!ValidateRefreshToken(dbRefreshToken, clientType, deviceId))
            {
                _logger.LogWarning(
                    "Suspicious refresh token usage. UserId={UserId}, TokenClientType={TokenClientType}, RequestClientType={RequestClientType}, DeviceId={DeviceId}",
                    dbRefreshToken.UserId,
                    dbRefreshToken.ClientType,
                    clientType,
                    deviceId
                );

                if (clientType == ClientTypes.Web)
                {
                    await RevokeWebRefreshTokensAsync(dbRefreshToken.UserId, clientInformations);
                }
                else
                {
                    await RevokeDeviceRefreshTokensAsync(dbRefreshToken.UserId, clientType, deviceId!, clientInformations);
                }
                await _unitOfWork.SaveChangesAsync(); // yapılan revoke'ları kaydediyorum.

                // refreshTokenların hepsini yada cihaza bağlı olan tokenları revoke et.
                return ServiceResult<RefreshResponse>.Fail("Suspicious login detected!", ErrorCodes.Unauthorized);
            }
            if (dbRefreshToken.Expires < DateTime.UtcNow) return ServiceResult<RefreshResponse>.Fail("Refresh Token expired. Please try login", ErrorCodes.Unauthorized);

            var ownerOfRefreshToken = await _unitOfWork.Users.FindAsync(dbRefreshToken.UserId);
            var tenant = await _unitOfWork.Tenants.GetByIdAsync(ownerOfRefreshToken!.TenantId);

            var newRefreshToken = await RotateRefreshTokenByRefreshTokenAsync(dbRefreshToken, clientInformations, clientType, deviceId);
            var newAccessToken = GenerateAccessToken(ownerOfRefreshToken.Id, ownerOfRefreshToken.Email, ownerOfRefreshToken.TenantId, tenant!.Domain);

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<RefreshResponse>.Ok(new RefreshResponse() { RefreshToken = newRefreshToken, AccessToken = newAccessToken });


        }
        private bool ValidateRefreshToken(RefreshToken token, string clientType, string? deviceId)
        {
            if (token.IsRevoked)
                return false;

            if (token.ClientType != clientType)
                return false;

            if (clientType == ClientTypes.Web)
                return true;

            // Mobile / Desktop
            return !string.IsNullOrEmpty(token.DeviceId)
                && token.DeviceId == deviceId;
        }

        private async Task<string?> RotateRefreshTokenByRefreshTokenAsync(RefreshToken refreshToken, AuditInfo clientInformations, string clientType, string? deviceId)
        {
            var newRefreshToken = GenerateRefreshToken();


            // revoking old refreshToken
            refreshToken.ReplacedByToken = newRefreshToken;
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = clientInformations.IpAddress;
            refreshToken.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.RefreshTokens.Update(refreshToken);

            // new refreshToken

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken()
            {
                IpAddress = clientInformations.IpAddress,
                Token = newRefreshToken,
                ClientType = clientType,
                DeviceId = deviceId,
                CreatedAt = DateTime.UtcNow,
                DeviceName = clientInformations.DeviceName,
                UserAgent = clientInformations.UserAgent,
                Expires = GetRefreshTokenExpiryByClient(clientType),
                UserId = refreshToken.UserId
            });
            return newRefreshToken;

        }

        public async Task RevokeWebRefreshTokensAsync(string userId, AuditInfo clientInformations)
        {
            var activeWebToken = await _unitOfWork.RefreshTokens.GetActiveWebRefreshTokenAsync(userId, false);
            if (activeWebToken == null) return;

            activeWebToken.RevokedAt = DateTime.UtcNow;
            activeWebToken.UpdatedAt = DateTime.UtcNow;
            activeWebToken.IsRevoked = true;
            activeWebToken.RevokedByIp = clientInformations.IpAddress;

            _unitOfWork.RefreshTokens.Update(activeWebToken);
        }
        public async Task<ServiceResult> RevokeDeviceRefreshTokensAsync(string userId, string clientType, string deviceId, AuditInfo clientInformations)
        {
            var activeDeviceToken = await _unitOfWork.RefreshTokens.GetActiveDeviceRefreshTokenAsync(userId, clientType, deviceId, false);
            if (activeDeviceToken == null)
            {
                _logger.LogWarning(
                "Refresh token not found or device mismatch. UserId: {UserId}, DeviceId: {DeviceId}",
                userId, deviceId);

                return ServiceResult.Fail("Session is no longer valid. Please log in again.", ErrorCodes.Unauthorized);
            }
            activeDeviceToken.RevokedAt = DateTime.UtcNow;
            activeDeviceToken.UpdatedAt = DateTime.UtcNow;
            activeDeviceToken.IsRevoked = true;
            activeDeviceToken.RevokedByIp = clientInformations.IpAddress;

            _unitOfWork.RefreshTokens.Update(activeDeviceToken);
            return ServiceResult.Ok();

        }
        public async Task RevokeAllDevicesAsync(string userId, string clientIp)
        {
            var allRefreshTokens = _unitOfWork.RefreshTokens.GetActiveRefreshTokensByUserId(userId,false);
            foreach (var refreshToken in allRefreshTokens)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.UpdatedAt = DateTime.UtcNow;
                refreshToken.RevokedByIp = clientIp;

                _unitOfWork.RefreshTokens.Update(refreshToken);
            }
        }
    }
}
