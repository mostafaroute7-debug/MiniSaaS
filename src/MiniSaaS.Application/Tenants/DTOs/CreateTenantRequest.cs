namespace MiniSaaS.Application.Tenants.DTOs;

public sealed class CreateTenantRequest
{
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
}