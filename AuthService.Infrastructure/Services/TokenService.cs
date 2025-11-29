using AuthService.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secret;
        private readonly string _issuer;

        public TokenService(IConfiguration _config)
        {
            _configuration = _config;
            _secret = _configuration["Jwt:Secret"]!;
            _issuer = _configuration["Jwt:Issuer"]!;
        }
        public string CreateTenantUserToken(string userId, string email, string tenantId, string tenantDomain)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("tenantId", tenantId),
                new Claim("role", "tenantUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = GetSigningCredentials(_secret);

            var token = GetJwtSecurityToken(_issuer, tenantDomain, claims, DateTime.UtcNow.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
        private JwtSecurityToken GetJwtSecurityToken(string issuer,string audience, IEnumerable<Claim> claims, DateTime? expires, SigningCredentials credentials)
        {
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims: claims,
                expires: expires,
                signingCredentials : credentials
            );
            return token;
        }
        private SigningCredentials GetSigningCredentials(string secretKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}
