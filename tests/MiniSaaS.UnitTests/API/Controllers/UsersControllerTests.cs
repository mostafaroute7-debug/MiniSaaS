using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniSaaS.API.Controllers;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Application.Users.Services;
using Moq;

namespace MiniSaaS.Tests.API.Controllers;

public sealed class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();

        _controller = new UsersController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenServiceSucceeds()
    {
        // Arrange
        var request = new PaginationRequest();

        var response = new PagedResultDto<UserResponse>
        {
            Items = new List<UserResponse>(),
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0
        };

        var serviceResult =ResultDto<PagedResultDto<UserResponse>>.Ok( response);

        _userServiceMock.Setup(x => x.GetAllAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetAll(request,CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        Assert.Same(serviceResult,okResult.Value);

        _userServiceMock.Verify(x => x.GetAllAsync(request,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenServiceSucceeds()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FullName = "John Doe",
            Email = "john@test.com",
            Password = "Password@123"
        };

        var userResponse = new UserResponse
        {
            Id = 1,
            TenantId = 1,
            FullName = "John Doe",
            Email = "john@test.com"
        };

        var serviceResult = ResultDto<UserResponse>.Ok( userResponse,"User created successfully.");

        _userServiceMock.Setup(x => x.CreateAsync( request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult =Assert.IsType<CreatedResult>(result);

        Assert.Equal(StatusCodes.Status201Created,createdResult.StatusCode);

        Assert.Equal("/api/users/1",createdResult.Location);

        Assert.Same(serviceResult,createdResult.Value);

        _userServiceMock.Verify(x => x.CreateAsync(request,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnErrorResult_WhenServiceFails()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FullName = "John Doe",
            Email = "john@test.com",
            Password = "Password@123"
        };

        var serviceResult =ResultDto<UserResponse>.Failure("A user with this email already exists.",ErrorCode.Conflict);

        _userServiceMock.Setup(x => x.CreateAsync( request,It.IsAny<CancellationToken>())) .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Create(request,CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        _userServiceMock.Verify(x => x.CreateAsync(request, It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenServiceSucceeds()
    {
        // Arrange
        var id = 1;

        var request = new UpdateUserRequest
        {
            FullName = "Updated User",
            Email = "updated@test.com"
        };

        var userResponse = new UserResponse
        {
            Id = id,
            TenantId = 1,
            FullName = "Updated User",
            Email = "updated@test.com"
        };

        var serviceResult =ResultDto<UserResponse>.Ok(userResponse,"User updated successfully.");

        _userServiceMock.Setup(x => x.UpdateAsync(id,request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Update(id,request,CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(StatusCodes.Status200OK,okResult.StatusCode);

        Assert.Same(serviceResult,okResult.Value);

        _userServiceMock.Verify(x => x.UpdateAsync(id, request,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnErrorResult_WhenServiceFails()
    {
        // Arrange
        var id = 999;

        var request = new UpdateUserRequest
        {
            FullName = "Updated User",
            Email = "updated@test.com"
        };

        var serviceResult =ResultDto<UserResponse>.Failure("User not found.", ErrorCode.NotFound);

        _userServiceMock.Setup(x => x.UpdateAsync(id,request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Update(id,request,CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        _userServiceMock.Verify( x => x.UpdateAsync(id,request,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenServiceSucceeds()
    {
        // Arrange
        var id = 1;

        var serviceResult = ResultDto<bool>.Ok( true,"User deleted successfully.");

        _userServiceMock.Setup(x => x.DeleteAsync( id,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Delete(id,CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(StatusCodes.Status200OK,okResult.StatusCode);

        Assert.Same(serviceResult,okResult.Value);

        _userServiceMock.Verify(x => x.DeleteAsync(id,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnErrorResult_WhenServiceFails()
    {
        // Arrange
        var id = 999;

        var serviceResult = ResultDto<bool>.Failure("User not found.",ErrorCode.NotFound);

        _userServiceMock.Setup(x => x.DeleteAsync(id,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.Delete(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        _userServiceMock.Verify(x => x.DeleteAsync(id,It.IsAny<CancellationToken>()),Times.Once);
    }
}