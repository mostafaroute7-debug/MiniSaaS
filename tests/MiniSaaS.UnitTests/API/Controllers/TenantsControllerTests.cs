using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniSaaS.API.Controllers;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Tenants.DTOs;
using MiniSaaS.Application.Tenants.Services;
using Moq;

namespace MiniSaaS.Tests.API.Controllers;

public class TenantsControllerTests
{
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly TenantsController _controller;

    public TenantsControllerTests()
    {
        _tenantServiceMock = new Mock<ITenantService>();

        _controller = new TenantsController(_tenantServiceMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturn201_WhenTenantIsCreated()
    {
        // Arrange

        var request = new CreateTenantRequest
        {
            Name = "Demo Tenant",
            Slug = "demo-tenant"
        };

        var tenantResponse = new TenantResponse
        {
            Id = 1,
            Name = "Demo Tenant",
            Slug = "demo-tenant",
            IsActive = true
        };

        var serviceResult = ResultDto<TenantResponse>.Ok(tenantResponse,"Tenant created successfully.");

        _tenantServiceMock.Setup(x => x.CreateAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act

        var result = await _controller.Create( request,CancellationToken.None);

        // Assert

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal( StatusCodes.Status201Created,createdResult.StatusCode);

        Assert.Equal(nameof(TenantsController.GetById),createdResult.ActionName);

        var response =Assert.IsType<ResultDto<TenantResponse>>(createdResult.Value);

        Assert.True(response.Success);

        Assert.NotNull(response.Data);

        Assert.Equal(1,response.Data!.Id);

        Assert.Equal("Demo Tenant",response.Data.Name);

        Assert.Equal("demo-tenant",response.Data.Slug);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenSlugAlreadyExists()
    {
        // Arrange

        var request = new CreateTenantRequest
        {
            Name = "Another Tenant",
            Slug = "demo-tenant"
        };

        var serviceResult = ResultDto<TenantResponse>.Failure( "A tenant with this slug already exists.",ErrorCode.Conflict);

        _tenantServiceMock.Setup(x =>x.CreateAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act

        var result = await _controller.Create(request,CancellationToken.None);

        // Assert

        var conflictResult =Assert.IsType<ConflictObjectResult>(result);

        Assert.Equal(StatusCodes.Status409Conflict,conflictResult.StatusCode);

        var response =Assert.IsType<ResultDto<TenantResponse>>(conflictResult.Value);

        Assert.False(response.Success);

        Assert.Equal(ErrorCode.Conflict,response.ErrorCode);

        Assert.Equal("A tenant with this slug already exists.",response.Message);
    }

    [Fact]
    public async Task GetById_ShouldReturn200_WhenTenantExists()
    {
        // Arrange

        var tenantResponse = new TenantResponse
        {
            Id = 1,
            Name = "Demo Tenant",
            Slug = "demo-tenant",
            IsActive = true
        };

        var serviceResult = ResultDto<TenantResponse>.Ok(tenantResponse);

        _tenantServiceMock.Setup(x =>x.GetByIdAsync(1,It.IsAny<CancellationToken>())) .ReturnsAsync(serviceResult);

        // Act

        var result = await _controller.GetById(1,CancellationToken.None);

        // Assert

        var okResult =Assert.IsType<OkObjectResult>(result);

        Assert.Equal(StatusCodes.Status200OK,okResult.StatusCode);

        var response = Assert.IsType<ResultDto<TenantResponse>>(okResult.Value);

        Assert.True(response.Success);

        Assert.NotNull(response.Data);

        Assert.Equal(1,response.Data!.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenTenantDoesNotExist()
    {
        // Arrange

        var serviceResult = ResultDto<TenantResponse>.Failure("Tenant not found.",ErrorCode.NotFound);

        _tenantServiceMock.Setup(x =>x.GetByIdAsync(999,It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

        // Act

        var result = await _controller.GetById(999,CancellationToken.None);

        // Assert

        var notFoundResult =Assert.IsType<NotFoundObjectResult>(result);

        Assert.Equal(StatusCodes.Status404NotFound,notFoundResult.StatusCode);

        var response = Assert.IsType<ResultDto<TenantResponse>>( notFoundResult.Value);

        Assert.False(response.Success);

        Assert.Equal(ErrorCode.NotFound,response.ErrorCode);
    }
}