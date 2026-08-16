
using MiniSaaS.Application.Common.Interfaces;

namespace MiniSaaS.Infrastructure.MultiTenancy;

public sealed class TenantContext : ITenantContext, ITenantContextAccessor
{
    public int? TenantId { get; private set; }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(int tenantId)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException( nameof(tenantId),"Tenant ID must be greater than zero.");
        }

        TenantId = tenantId;
    }
}