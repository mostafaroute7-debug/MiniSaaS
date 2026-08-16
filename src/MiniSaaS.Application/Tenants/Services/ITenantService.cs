using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Tenants.DTOs;

namespace MiniSaaS.Application.Tenants.Services;

public interface ITenantService
{
    Task<ResultDto<TenantResponse>> CreateAsync(CreateTenantRequest request,CancellationToken cancellationToken = default);
    Task<ResultDto<TenantResponse>> GetByIdAsync(int id,CancellationToken cancellationToken = default);
}