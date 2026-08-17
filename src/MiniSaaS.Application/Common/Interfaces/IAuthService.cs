
using MiniSaaS.Application.Auth.DTOs;
using MiniSaaS.Application.Common.Models;

namespace MiniSaaS.Application.Common.Interfaces;

public interface IAuthService
{
    Task<ResultDto<AuthResponse>> LoginAsync(LoginRequest request,CancellationToken cancellationToken = default);
}