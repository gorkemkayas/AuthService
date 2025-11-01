using AuthService.Infrastructure.Persistance.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Persistance.DbContexts;
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var dbConfig = configuration.GetSection("DatabaseConfigurations").Get<DatabaseConfigurations>();

        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseNpgsql(dbConfig.ConnectionString);

        return new AuthDbContext(optionsBuilder.Options);
    }
}

