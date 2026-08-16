
namespace MiniSaaS.Application.Common.Interfaces;

public interface ITenantContext
{
    int? TenantId { get; }
    bool HasTenant { get; }
}
