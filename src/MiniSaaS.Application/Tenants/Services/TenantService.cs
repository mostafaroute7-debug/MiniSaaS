using FluentValidation;
using Microsoft.Extensions.Logging;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Mapping;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Tenants.DTOs;
using MiniSaaS.Domain.Entities;

namespace MiniSaaS.Application.Tenants.Services;

public sealed class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTenantRequest> _createTenantValidator;
    private readonly ILogger<TenantService> _logger;

    public TenantService(IUnitOfWork unitOfWork,IValidator<CreateTenantRequest> createTenantValidator,ILogger<TenantService> logger)
    {
        _unitOfWork = unitOfWork;
        _createTenantValidator = createTenantValidator;
        _logger = logger;
    }

    public async Task<ResultDto<TenantResponse>> CreateAsync( CreateTenantRequest request,CancellationToken cancellationToken = default)
    {
        var validationResult =await _createTenantValidator.ValidateAsync( request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

            return ResultDto<TenantResponse>.Failure("One or more validation errors occurred.",ErrorCode.Validation,errors);
        }

        var tenantRepository =_unitOfWork.Repository<Tenant>();

        var slugExists =await tenantRepository.ExistsAsync(x => x.Slug == request.Slug,cancellationToken);

        if (slugExists)
        {
            _logger.LogWarning("Tenant creation conflict. Slug {Slug} already exists.",request.Slug);

            return ResultDto<TenantResponse>.Failure("A tenant with this slug already exists.",ErrorCode.Conflict);
        }

        var tenant = new Tenant
        {
            Name = request.Name,
            Slug = request.Slug,
            IsActive = true
        };

        await tenantRepository.AddAsync(tenant,cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tenant {TenantId} created successfully with slug {Slug}.",tenant.Id,tenant.Slug);

        return ResultDto<TenantResponse>.Ok(tenant.ToResponse(),"Tenant created successfully.");
    }

    public async Task<ResultDto<TenantResponse>> GetByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        var tenantRepository = _unitOfWork.Repository<Tenant>();

        var tenant = await tenantRepository.GetByIdAsync(id,cancellationToken);

        if (tenant is null)
        {
            return ResultDto<TenantResponse>.Failure("Tenant not found.",ErrorCode.NotFound);
        }

        return ResultDto<TenantResponse>.Ok(tenant.ToResponse());
    }
}
