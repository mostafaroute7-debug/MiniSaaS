
using MiniSaaS.Domain.Enums;

namespace MiniSaaS.Application.Users.DTOs;

public sealed class CreateUserRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
}
