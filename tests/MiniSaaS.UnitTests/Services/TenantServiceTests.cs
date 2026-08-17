using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Tenants.DTOs;
using MiniSaaS.Application.Tenants.Services;
using MiniSaaS.Domain.Entities;
using Moq;

namespace MiniSaaS.Application.Tests.Services;

public class TenantServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Tenant>> _tenantRepositoryMock;
    private readonly Mock<IValidator<CreateTenantRequest>> _validatorMock;
    private readonly Mock<ILogger<TenantService>> _loggerMock;
    private readonly TenantService _service;

    public TenantServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantRepositoryMock = new Mock<IRepository<Tenant>>();
        _validatorMock = new Mock<IValidator<CreateTenantRequest>>();
        _loggerMock = new Mock<ILogger<TenantService>>();
        _unitOfWorkMock.Setup(x => x.Repository<Tenant>()).Returns(_tenantRepositoryMock.Object);
        _service = new TenantService(_unitOfWorkMock.Object,_validatorMock.Object,_loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTenant_WhenRequestIsValid()
    {
        // Arrange

        var request = new CreateTenantRequest
        {
            Name = "Demo Tenant",
            Slug = "demo-tenant"
        };

        _validatorMock.Setup(x => x.ValidateAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        _tenantRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),It.IsAny<CancellationToken>())).ReturnsAsync(false);

        _tenantRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Tenant>(),It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act

        var result = await _service.CreateAsync(request);

        // Assert

        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        Assert.Equal("Demo Tenant", result.Data!.Name);
        Assert.Equal("demo-tenant", result.Data.Slug);
        Assert.True(result.Data.IsActive);

        Assert.Equal("Tenant created successfully.",result.Message);

        _tenantRepositoryMock.Verify(x => x.AddAsync(It.Is<Tenant>(t =>
                    t.Name == "Demo Tenant" && t.Slug == "demo-tenant" && t.IsActive),
                    It.IsAny<CancellationToken>()),Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }
    [Fact]
    public async Task CreateAsync_ShouldReturnValidationError_WhenRequestIsInvalid()
    {
        // Arrange

        var request = new CreateTenantRequest
        {
            Name = "",
            Slug = ""
        };

        var validationResult = new ValidationResult(new[]{
            new ValidationFailure("Name","Tenant name is required."),
            new ValidationFailure("Slug","Tenant slug is required.")});

        _validatorMock.Setup(x => x.ValidateAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act

        var result = await _service.CreateAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.Validation,result.ErrorCode);

        Assert.Equal("One or more validation errors occurred.",result.Message);

        Assert.NotNull(result.Errors);

        Assert.Contains("Tenant name is required.",result.Errors);

        Assert.Contains("Tenant slug is required.",result.Errors);

        _tenantRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),It.IsAny<CancellationToken>()),Times.Never);

        _tenantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tenant>(),It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflict_WhenSlugAlreadyExists()
    {
        // Arrange

        var request = new CreateTenantRequest
        {
            Name = "Another Tenant",
            Slug = "demo-tenant"
        };

        _validatorMock.Setup(x => x.ValidateAsync(request,It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        _tenantRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act

        var result = await _service.CreateAsync(request);

        // Assert

        Assert.False(result.Success);

        Assert.Equal(ErrorCode.Conflict,result.ErrorCode);

        Assert.Equal("A tenant with this slug already exists.",result.Message);

        _tenantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tenant>(),It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTenant_WhenTenantExists()
    {
        // Arrange

        var tenant = new Tenant
        {
            Id = 1,
            Name = "Demo Tenant",
            Slug = "demo-tenant",
            IsActive = true
        };

        _tenantRepositoryMock.Setup(x => x.GetByIdAsync(1,It.IsAny<CancellationToken>())).ReturnsAsync(tenant);

        // Act

        var result = await _service.GetByIdAsync(1);

        // Assert

        Assert.True(result.Success);

        Assert.NotNull(result.Data);

        Assert.Equal(1, result.Data!.Id);
        Assert.Equal("Demo Tenant", result.Data.Name);
        Assert.Equal("demo-tenant", result.Data.Slug);
        Assert.True(result.Data.IsActive);
    }

}