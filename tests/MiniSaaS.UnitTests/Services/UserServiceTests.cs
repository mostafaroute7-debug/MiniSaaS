using Microsoft.Extensions.Logging;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Application.Users.Services;
using MiniSaaS.Domain.Entities;
using MiniSaaS.Domain.Enums;
using Moq;

namespace MiniSaaS.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly UserService _service;
    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _userRepositoryMock =new Mock<IRepository<User>>();

        _tenantContextMock =new Mock<ITenantContext>();

        _loggerMock = new Mock<ILogger<UserService>>();

        _passwordHasherMock =new Mock<IPasswordHasher>();

        _unitOfWorkMock.Setup(x => x.Repository<User>()).Returns(_userRepositoryMock.Object);

        _service = new UserService(_unitOfWorkMock.Object, _tenantContextMock.Object, _loggerMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTenantRequired_WhenTenantContextIsMissing()
    {
        // Arrange

        var request = new CreateUserRequest
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "Password@123",
            Role = UserRole.Member
        };

        _tenantContextMock.Setup(x => x.HasTenant).Returns(false);

        // Act

        var result = await _service.CreateAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.TenantRequired,result.ErrorCode);

        Assert.Equal("A tenant context is required.", result.Message);

        Assert.Null(result.Data);

        _userRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),It.IsAny<CancellationToken>()),Times.Never);

        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify( x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange

        var request = new CreateUserRequest
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "Password@123",
            Role = UserRole.Member
        };

        _tenantContextMock.Setup(x => x.HasTenant).Returns(true);

        _tenantContextMock.Setup(x => x.TenantId).Returns(1);

        _userRepositoryMock.Setup(x => x.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        // Act

        var result = await _service.CreateAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.Conflict,result.ErrorCode);

        Assert.Equal("A user with this email already exists.",result.Message);

        _passwordHasherMock.Verify(x => x.Hash(It.IsAny<string>()),Times.Never);

        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify( x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }
    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenRequestIsValid()
    {
        // Arrange

        var request = new CreateUserRequest
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "Password@123",
            Role = UserRole.Member
        };

        _tenantContextMock.Setup(x => x.HasTenant).Returns(true);

        _tenantContextMock.Setup(x => x.TenantId).Returns(1);

        _userRepositoryMock.Setup(x => x.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        _passwordHasherMock.Setup(x => x.Hash("Password@123")).Returns("HASHED_PASSWORD");

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(),It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act

        var result = await _service.CreateAsync(request);

        // Assert

        Assert.True(result.Success);

        Assert.NotNull(result.Data);

        Assert.Equal("John Doe",result.Data!.FullName);

        Assert.Equal("john@example.com",result.Data.Email);

        Assert.Equal(UserRole.Member.ToString(),result.Data.Role);

        Assert.Equal( "User created successfully.",result.Message);

        _passwordHasherMock.Verify(x => x.Hash("Password@123"),Times.Once);

        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<User>(u =>
                    u.TenantId == 1 &&
                    u.FullName == "John Doe" &&
                    u.Email == "john@example.com" &&
                    u.PasswordHash == "HASHED_PASSWORD" &&
                    u.Role == UserRole.Member &&
                    u.IsActive),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }
    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange

        var request = new UpdateUserRequest
        {
            FullName = "Updated Name",
            Email = "updated@example.com",
            Role = UserRole.Member
        };

        _userRepositoryMock .Setup(x => x.GetByIdAsync(999,It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act

        var result = await _service.UpdateAsync(999, request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.NotFound,result.ErrorCode);

        Assert.Equal( "User not found.",result.Message);

        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange

        var request = new UpdateUserRequest
        {
            FullName = "Updated Name",
            Email = "existing@example.com",
            Role = UserRole.Member
        };

        var user = new User
        {
            Id = 1,
            TenantId = 1,
            FullName = "Old Name",
            Email = "old@example.com",
            Role = UserRole.Member,
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1,It.IsAny<CancellationToken>())).ReturnsAsync(user);

        _userRepositoryMock.Setup(x => x.ExistsAsync( It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act

        var result = await _service.UpdateAsync(1, request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.Conflict,result.ErrorCode);

        Assert.Equal( "A user with this email already exists.",result.Message);

        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenRequestIsValid()
    {
        // Arrange

        var request = new UpdateUserRequest
        {
            FullName = "Updated Name",
            Email = "updated@example.com",
            Role = UserRole.Admin
        };

        var user = new User
        {
            Id = 1,
            TenantId = 1,
            FullName = "Old Name",
            Email = "old@example.com",
            Role = UserRole.Member,
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act

        var result = await _service.UpdateAsync(1, request);

        // Assert

        Assert.True(result.Success);

        Assert.NotNull(result.Data);

        Assert.Equal( "Updated Name",result.Data!.FullName);

        Assert.Equal("updated@example.com",result.Data.Email);

        Assert.Equal( UserRole.Admin.ToString(),result.Data.Role);

        Assert.Equal("User updated successfully.",result.Message);

        _userRepositoryMock.Verify(
            x => x.Update(
                It.Is<User>(u =>
                    u.Id == 1 &&
                    u.FullName == "Updated Name" &&
                    u.Email == "updated@example.com" &&
                    u.Role == UserRole.Admin)),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange

        _userRepositoryMock.Setup(x => x.GetByIdAsync( 999,It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act

        var result = await _service.DeleteAsync(999);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.NotFound, result.ErrorCode);

        Assert.Equal("User not found.",result.Message);

        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()),Times.Never);

        _unitOfWorkMock.Verify( x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteUser_WhenUserExists()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            TenantId = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Member,
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(1,It.IsAny<CancellationToken>())).ReturnsAsync(user);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act

        var result = await _service.DeleteAsync(1);

        // Assert

        Assert.True(result.Success);

        Assert.True(result.Data);

        Assert.Equal("User deleted successfully.",result.Message);

        Assert.False(user.IsActive);

        _userRepositoryMock.Verify(x => x.Update(It.Is<User>(u => u.Id == 1 &&!u.IsActive)),Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }
}