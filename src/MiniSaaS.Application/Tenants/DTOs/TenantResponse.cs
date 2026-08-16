
namespace MiniSaaS.Application.Tenants.DTOs;

public sealed class TenantResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
