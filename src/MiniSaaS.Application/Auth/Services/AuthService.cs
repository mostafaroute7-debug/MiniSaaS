using MiniSaaS.Application.Auth.DTOs;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Domain.Entities;

namespace MiniSaaS.Application.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _jwtTokenService;
    private readonly ITenantContext _tenantContext;

    public AuthService(IUnitOfWork unitOfWork,IPasswordHasher passwordHasher,ITokenService jwtTokenService, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _tenantContext = tenantContext;
    }

    public async Task<ResultDto<AuthResponse>> LoginAsync(LoginRequest request,CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
        {
            return ResultDto<AuthResponse>.Failure( "A tenant context is required.",ErrorCode.TenantRequired);
        }

        var tenantId = _tenantContext.TenantId!.Value;

        var userRepository =_unitOfWork.Repository<User>();

        var user = await userRepository.FirstOrDefaultAsync(x =>x.Email == request.Email &&x.IsActive,cancellationToken);

        if (user is null)
        {
            return ResultDto<AuthResponse>.Failure( "Invalid email or password.", ErrorCode.Unauthorized);
        }

        var passwordValid = _passwordHasher.Verify(request.Password,user.PasswordHash);

        if (!passwordValid)
        {
            return ResultDto<AuthResponse>.Failure( "Invalid email or password.", ErrorCode.Unauthorized);
        }

        var token =_jwtTokenService.GenerateToken(user.Id,tenantId,user.Email,user.Role.ToString());

        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        return ResultDto<AuthResponse>.Ok(new AuthResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt
            },"Login successful.");
    }
}
