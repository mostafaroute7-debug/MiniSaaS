

namespace MiniSaaS.Infrastructure.MultiTenancy;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TenantRequiredAttribute : Attribute
{
}