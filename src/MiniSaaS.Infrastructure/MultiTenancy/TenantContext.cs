
using MiniSaaS.Application.Common.Interfaces;

namespace MiniSaaS.Infrastructure.MultiTenancy;

public sealed class TenantContext : ITenantContext
{
    public int? TenantId { get; private set; }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(int tenantId)
    {
        TenantId = tenantId;
    }
}