using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.API.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://auth.kayas.dev",
                    ValidateAudience = true,
                    ValidAudience = "https://auth.kayas.dev",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowKayasSubdomains", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        // origin: "https://tenant1.kayas.com"
                        return origin.EndsWith(".kayas.dev") || origin == "https://kayas.dev";
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            return services;
        }
    }
}
