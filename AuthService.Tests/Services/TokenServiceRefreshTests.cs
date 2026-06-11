using AuthService.Application.Common;
using AuthService.Application.Dtos.Refresh;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Common;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AuthService.Tests.Services;

public class TokenServiceRefreshTests
{
    private static TokenService CreateSut(
        Mock<IUnitOfWork> unitOfWork,
        Mock<IRoleService> roleService,
        Mock<ILogger<TokenService>> logger)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "THIS_IS_A_TEST_SECRET_KEY_FOR_JWT_123456789",
                ["Jwt:Issuer"] = "https://auth.test"
            })
            .Build();

        return new TokenService(
            config,
            unitOfWork.Object,
            Mock.Of<IEntityMapper>(),
            Options.Create(new TokenOptions
            {
                AccessTokenLifetimeMinutes = 60,
                RefreshTokenLifetimeDays = 30,
                WebRefreshTokenLifetimeDays = 7,
                MobileRefreshTokenLifetimeDays = 30
            }),
            logger.Object,
            roleService.Object);
    }

    private static Mock<IUnitOfWork> CreateUnitOfWorkMock(
        Mock<IRefreshTokenRepository> refreshTokens,
        Mock<IUserRepository> users,
        Mock<ITenantRepository> tenants)
    {
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.RefreshTokens).Returns(refreshTokens.Object);
        uow.SetupGet(x => x.Users).Returns(users.Object);
        uow.SetupGet(x => x.Tenants).Returns(tenants.Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return uow;
    }

    [Fact]
    public async Task RefreshAsync_ReturnsUnauthorized_WhenRefreshTokenIsMissing()
    {
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        var users = new Mock<IUserRepository>();
        var tenants = new Mock<ITenantRepository>();
        var roleService = new Mock<IRoleService>();
        var logger = new Mock<ILogger<TokenService>>();
        var sut = CreateSut(CreateUnitOfWorkMock(refreshTokens, users, tenants), roleService, logger);

        var result = await sut.RefreshAsync(null, new AuditInfo("127.0.0.1", "agent", "device"), ClientTypes.Web, null);

        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.Unauthorized, result.ErrorCode);
        Assert.Equal("Refresh token is required.", result.Message);
    }

    [Fact]
    public async Task RefreshAsync_ReturnsUnauthorized_WhenRefreshTokenIsExpired()
    {
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        var users = new Mock<IUserRepository>();
        var tenants = new Mock<ITenantRepository>();
        var roleService = new Mock<IRoleService>();
        var logger = new Mock<ILogger<TokenService>>();

        refreshTokens.Setup(x => x.FindByTokenAsync("expired", false))
            .ReturnsAsync(new RefreshToken
            {
                Token = "expired",
                UserId = "user-1",
                ClientType = ClientTypes.Web,
                Expires = DateTime.UtcNow.AddMinutes(-5)
            });

        var sut = CreateSut(CreateUnitOfWorkMock(refreshTokens, users, tenants), roleService, logger);

        var result = await sut.RefreshAsync("expired", new AuditInfo("127.0.0.1", "agent", "device"), ClientTypes.Web, null);

        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.Unauthorized, result.ErrorCode);
        Assert.Equal("Refresh token has expired.", result.Message);
    }

    [Fact]
    public async Task RefreshAsync_RotatesRefreshToken_AndReturnsNewTokenPair()
    {
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        var users = new Mock<IUserRepository>();
        var tenants = new Mock<ITenantRepository>();
        var roleService = new Mock<IRoleService>();
        var logger = new Mock<ILogger<TokenService>>();

        var existingToken = new RefreshToken
        {
            Token = "current-refresh-token",
            UserId = "user-1",
            ClientType = ClientTypes.Web,
            Expires = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        };

        refreshTokens.Setup(x => x.FindByTokenAsync("current-refresh-token", false))
            .ReturnsAsync(existingToken);
        refreshTokens.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        refreshTokens.Setup(x => x.Update(It.IsAny<RefreshToken>()));

        users.Setup(x => x.FindAsync("user-1", It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = "user-1",
                Email = "user@example.com",
                Name = "Ada",
                Surname = "Lovelace",
                TenantId = 42
            });

        tenants.Setup(x => x.GetByIdAsync(42)).ReturnsAsync(new Tenant { Id = 42, Name = "TestTenant" });
        roleService.Setup(x => x.GetUserRolesAsync("user-1"))
            .ReturnsAsync(ServiceResult<IList<string>>.Ok(new List<string> { "Customer" }));

        var sut = CreateSut(CreateUnitOfWorkMock(refreshTokens, users, tenants), roleService, logger);

        var result = await sut.RefreshAsync("current-refresh-token", new AuditInfo("127.0.0.1", "agent", "device"), ClientTypes.Web, null);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.False(string.IsNullOrWhiteSpace(result.Data!.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.Data.RefreshToken));
        Assert.NotEqual("current-refresh-token", result.Data.RefreshToken);

        refreshTokens.Verify(x => x.Update(It.Is<RefreshToken>(t => t.Token == "current-refresh-token" && t.IsRevoked)), Times.Once);
        refreshTokens.Verify(x => x.AddAsync(It.Is<RefreshToken>(t => t.UserId == "user-1" && t.ClientType == ClientTypes.Web), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(existingToken.IsRevoked);
        Assert.Equal(result.Data.RefreshToken, existingToken.ReplacedByToken);
    }
}
