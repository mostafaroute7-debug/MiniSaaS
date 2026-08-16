using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Mapping;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Tenants.DTOs;
using MiniSaaS.Domain.Entities;

namespace MiniSaaS.Application.Tenants.Services;

public sealed class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;

    public TenantService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto<TenantResponse>> CreateAsync(
        CreateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantRepository =
            _unitOfWork.Repository<Tenant>();

        var slugExists = await tenantRepository.ExistsAsync(
            x => x.Slug == request.Slug,
            cancellationToken);

        if (slugExists)
        {
            return ResultDto<TenantResponse>.Failure(
                "A tenant with this slug already exists.");
        }

        var tenant = new Tenant
        {
            Name = request.Name,
            Slug = request.Slug,
            IsActive = true
        };

        await tenantRepository.AddAsync(
            tenant,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ResultDto<TenantResponse>.Ok(
            tenant.ToResponse(),
            "Tenant created successfully.");
    }

    public async Task<ResultDto<TenantResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tenantRepository =
            _unitOfWork.Repository<Tenant>();

        var tenant = await tenantRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (tenant is null)
        {
            return ResultDto<TenantResponse>.Failure(
                "Tenant not found.");
        }

        return ResultDto<TenantResponse>.Ok(
            tenant.ToResponse());
    }
}
