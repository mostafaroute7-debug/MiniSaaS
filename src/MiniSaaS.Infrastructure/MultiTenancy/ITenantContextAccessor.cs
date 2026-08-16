namespace MiniSaaS.Infrastructure.MultiTenancy;

public interface ITenantContextAccessor
{
    void SetTenant(int tenantId);
}
