using AuthService.API.Controllers;
using AuthService.Application.Common;
using AuthService.Application.Dtos.Refresh;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces.Contexts;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AuthService.Tests.Controllers;

public class AuthControllerRefreshTests
{
    private static AuthController CreateController(Mock<ITokenService> tokenService, Mock<IUserService> userService, Mock<IClientContext> clientContext)
    {
        var controller = new AuthController(
            tokenService.Object,
            userService.Object,
            clientContext.Object,
            Options.Create(new TokenOptions { WebRefreshTokenLifetimeDays = 7, RefreshTokenLifetimeDays = 30, MobileRefreshTokenLifetimeDays = 30, AccessTokenLifetimeMinutes = 60 }));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Headers["User-Agent"] = "xunit";
        return controller;
    }

    [Fact]
    public async Task Refresh_ReturnsTokenPair_WhenServiceSucceeds()
    {
        var tokenService = new Mock<ITokenService>();
        var userService = new Mock<IUserService>();
        var clientContext = new Mock<IClientContext>();
        clientContext.SetupGet(x => x.ClientType).Returns(ClientTypes.Web);
        clientContext.SetupGet(x => x.DeviceId).Returns((string?)null);

        tokenService.Setup(x => x.RefreshAsync("refresh-1", It.IsAny<AuditInfo>(), ClientTypes.Web, null))
            .ReturnsAsync(ServiceResult<RefreshResponse>.Ok(new RefreshResponse
            {
                Token = "new-access-token",
                RefreshToken = "new-refresh-token"
            }));

        var controller = CreateController(tokenService, userService, clientContext);

        var result = await controller.Refresh(new RefreshRequest { RefreshToken = "refresh-1" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<RefreshResponse>(ok.Value);
        Assert.Equal("new-access-token", payload.Token);
        Assert.Equal("new-refresh-token", payload.RefreshToken);
        Assert.False(controller.HttpContext.Response.Headers.ContainsKey("Set-Cookie"));
    }

    [Fact]
    public async Task Login_DoesNotSetRefreshCookie_WhenServiceSucceeds()
    {
        var tokenService = new Mock<ITokenService>();
        var userService = new Mock<IUserService>();
        var clientContext = new Mock<IClientContext>();
        clientContext.SetupGet(x => x.ClientType).Returns(ClientTypes.Web);
        clientContext.SetupGet(x => x.DeviceId).Returns((string?)null);

        userService.Setup(x => x.LoginAsync(It.IsAny<LoginUserRequest>()))
            .ReturnsAsync(ServiceResult<LoginUserResponse>.Ok(new LoginUserResponse
            {
                UserId = "user-1",
                Email = "user@example.com",
                TenantId = 42,
                TenantDomain = "tenant.example"
            }));

        tokenService.Setup(x => x.CreateTenantUserTokenAsync(It.IsAny<CreateTenantUserTokenRequest>()))
            .ReturnsAsync(ServiceResult<CreateTenantUserTokenResponse>.Ok(new CreateTenantUserTokenResponse
            {
                Token = "access-token",
                RefreshToken = "refresh-token"
            }));

        var controller = CreateController(tokenService, userService, clientContext);

        var result = await controller.Login(new LoginUserRequest { Email = "user@example.com", Password = "P@ssw0rd!" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.False(controller.HttpContext.Response.Headers.ContainsKey("Set-Cookie"));
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Refresh_ReturnsUnauthorized_WhenServiceFails()
    {
        var tokenService = new Mock<ITokenService>();
        var userService = new Mock<IUserService>();
        var clientContext = new Mock<IClientContext>();
        clientContext.SetupGet(x => x.ClientType).Returns(ClientTypes.Web);
        clientContext.SetupGet(x => x.DeviceId).Returns((string?)null);

        tokenService.Setup(x => x.RefreshAsync("expired", It.IsAny<AuditInfo>(), ClientTypes.Web, null))
            .ReturnsAsync(ServiceResult<RefreshResponse>.Fail("Refresh token has expired.", ErrorCodes.Unauthorized));

        var controller = CreateController(tokenService, userService, clientContext);

        var result = await controller.Refresh(new RefreshRequest { RefreshToken = "expired" });

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var payload = unauthorized.Value!;
        var payloadType = payload.GetType();
        Assert.Equal("Refresh token has expired.", payloadType.GetProperty("message")!.GetValue(payload));
        Assert.Equal("Refresh token has expired.", payloadType.GetProperty("detail")!.GetValue(payload));
    }
}
