
namespace MiniSaaS.Application.Auth.DTOs;

public sealed class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}