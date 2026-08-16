using MiniSaaS.Domain.Common;
using MiniSaaS.Domain.Enums;

namespace MiniSaaS.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
