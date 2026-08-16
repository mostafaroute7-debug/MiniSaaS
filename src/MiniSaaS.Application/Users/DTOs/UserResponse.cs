namespace MiniSaaS.Application.Users.DTOs;

public sealed class UserResponse
{
    public int Id { get; init; }
    public int TenantId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}