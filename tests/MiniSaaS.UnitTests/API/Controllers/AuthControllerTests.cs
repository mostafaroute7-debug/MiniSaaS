using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniSaaS.API.Controllers;
using MiniSaaS.Application.Auth.DTOs;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Models;
using Moq;

namespace MiniSaaS.Tests.API.Controllers;

public sealed class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();

        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenLoginSucceeds()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "Admin@123456"
        };

        var authResponse = new AuthResponse
        {
            AccessToken = "fake-jwt-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        var serviceResult = ResultDto<AuthResponse>.Ok(authResponse,"Login successful.");

        _authServiceMock
            .Setup(x => x.LoginAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Login(request,CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(StatusCodes.Status200OK,okResult.StatusCode);

        Assert.Same(serviceResult,okResult.Value);

        _authServiceMock.Verify(x => x.LoginAsync(request,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "WrongPassword"
        };

        var serviceResult = ResultDto<AuthResponse>.Failure("Invalid email or password.",ErrorCode.Unauthorized);

        _authServiceMock.Setup(x => x.LoginAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Login(
            request,
            CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(StatusCodes.Status401Unauthorized,unauthorizedResult.StatusCode);

        Assert.Same(serviceResult,unauthorizedResult.Value);

        _authServiceMock.Verify(x => x.LoginAsync( request,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenTenantIsRequired()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "Admin@123456"
        };

        var serviceResult = ResultDto<AuthResponse>.Failure("A tenant context is required.",ErrorCode.TenantRequired);

        _authServiceMock.Setup(x => x.LoginAsync( request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Login(request,CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        _authServiceMock.Verify(x => x.LoginAsync( request,It.IsAny<CancellationToken>()),Times.Once);
    }
}