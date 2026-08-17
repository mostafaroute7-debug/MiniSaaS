using MiniSaaS.Application.Auth.DTOs;
using MiniSaaS.Application.Auth.Services;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Domain.Entities;
using MiniSaaS.Domain.Enums;
using Moq;

namespace MiniSaaS.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _unitOfWorkMock =new Mock<IUnitOfWork>();

        _userRepositoryMock = new Mock<IRepository<User>>();

        _passwordHasherMock =new Mock<IPasswordHasher>();

        _tokenServiceMock = new Mock<ITokenService>();

        _tenantContextMock =new Mock<ITenantContext>();

        _unitOfWorkMock .Setup(x => x.Repository<User>()).Returns(_userRepositoryMock.Object);

        _service = new AuthService( _unitOfWorkMock.Object,_passwordHasherMock.Object,_tokenServiceMock.Object,_tenantContextMock.Object);
    }
    [Fact]
    public async Task LoginAsync_ShouldReturnTenantRequired_WhenTenantContextIsMissing()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "Admin@123456"
        };

        _tenantContextMock.Setup(x => x.HasTenant).Returns(false);

        // Act

        var result = await _service.LoginAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.TenantRequired,result.ErrorCode);

        Assert.Equal("A tenant context is required.",result.Message);

        Assert.Null(result.Data);

        _userRepositoryMock.Verify(
            x => x.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _passwordHasherMock.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<string>(),It.IsAny<string>()),Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "unknown@minisaas.com",
            Password = "Admin@123456"
        };

        _tenantContextMock.Setup(x => x.HasTenant) .Returns(true);

        _tenantContextMock.Setup(x => x.TenantId).Returns(1);

        _userRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),It.IsAny<CancellationToken>())) .ReturnsAsync((User?)null);

        // Act

        var result = await _service.LoginAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.Unauthorized,result.ErrorCode);

        Assert.Equal("Invalid email or password.",result.Message);

        Assert.Null(result.Data);

        _passwordHasherMock.Verify(x => x.Verify(It.IsAny<string>(),It.IsAny<string>()),Times.Never);

        _tokenServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = 1,
            TenantId = 1,
            FullName = "System Admin",
            Email = "admin@minisaas.com",
            PasswordHash = "HASHED_PASSWORD",
            Role = UserRole.Admin,
            IsActive = true
        };

        _tenantContextMock.Setup(x => x.HasTenant).Returns(true);

        _tenantContextMock.Setup(x => x.TenantId).Returns(1);

        _userRepositoryMock.Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.Verify( "WrongPassword","HASHED_PASSWORD")).Returns(false);

        // Act

        var result = await _service.LoginAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.Unauthorized,result.ErrorCode);

        Assert.Equal("Invalid email or password.",result.Message);

        Assert.Null(result.Data);

        _passwordHasherMock.Verify(x => x.Verify("WrongPassword","HASHED_PASSWORD"),Times.Once);

        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<string>(), It.IsAny<string>()),Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "Admin@123456"
        };

        var user = new User
        {
            Id = 1,
            TenantId = 1,
            FullName = "System Admin",
            Email = "admin@minisaas.com",
            PasswordHash = "HASHED_PASSWORD",
            Role = UserRole.Admin,
            IsActive = true
        };

        _tenantContextMock.Setup(x => x.HasTenant) .Returns(true);

        _tenantContextMock.Setup(x => x.TenantId).Returns(1);

        _userRepositoryMock .Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.Verify("Admin@123456","HASHED_PASSWORD")).Returns(true);

        _tokenServiceMock.Setup(x => x.GenerateToken(1,1,"admin@minisaas.com","Admin")).Returns("JWT_TOKEN");

        // Act

        var result = await _service.LoginAsync(request);

        // Assert

        Assert.True(result.Success);

        Assert.NotNull(result.Data);

        Assert.Equal("JWT_TOKEN",result.Data!.AccessToken);

        Assert.Equal("Login successful.",result.Message);

        Assert.True(result.Data.ExpiresAt > DateTime.UtcNow);

        _passwordHasherMock.Verify(x => x.Verify("Admin@123456","HASHED_PASSWORD"), Times.Once);

        _tokenServiceMock.Verify(x => x.GenerateToken(1,1,"admin@minisaas.com","Admin"),Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenUserIsInactive()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "admin@minisaas.com",
            Password = "Admin@123456"
        };

        _tenantContextMock.Setup(x => x.HasTenant) .Returns(true);

        _tenantContextMock.Setup(x => x.TenantId) .Returns(1);

        _userRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act

        var result = await _service.LoginAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal( ErrorCode.Unauthorized,result.ErrorCode);

        Assert.Equal("Invalid email or password.",result.Message);

        _passwordHasherMock.Verify(x => x.Verify(It.IsAny<string>(),It.IsAny<string>()),Times.Never);

        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<string>(),It.IsAny<string>()),Times.Never);
    }
}