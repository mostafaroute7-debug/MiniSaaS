using Microsoft.Extensions.Logging;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Common.Mapping;
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Domain.Entities;

namespace MiniSaaS.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ILogger<UserService> logger,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResultDto<PagedResultDto<UserResponse>>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork
            .Repository<User>()
            .GetPagedAsync(
                request,
                orderBy: x => x.Id,
                cancellationToken: cancellationToken);

        var response = new PagedResultDto<UserResponse>
        {
            Items = result.Items
                .Select(x => x.ToResponse())
                .ToList(),

            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return ResultDto<PagedResultDto<UserResponse>>.Ok(
            response);
    }

    public async Task<ResultDto<UserResponse>> CreateAsync(
     CreateUserRequest request,
     CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
        {
            return ResultDto<UserResponse>.Failure(
                "A tenant context is required.",
                ErrorCode.TenantRequired);
        }

        var tenantId = _tenantContext.TenantId!.Value;

        var userRepository =
            _unitOfWork.Repository<User>();

        var emailExists = await userRepository.ExistsAsync(
            x => x.Email == request.Email,
            cancellationToken);

        if (emailExists)
        {
            _logger.LogWarning(
                "User creation conflict. Email {Email} already exists for tenant {TenantId}.",
                request.Email,
                tenantId);

            return ResultDto<UserResponse>.Failure(
                "A user with this email already exists.",
                ErrorCode.Conflict);
        }

        var passwordHash = _passwordHasher.Hash(
            request.Password);

        var user = new User
        {
            TenantId = tenantId,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            IsActive = true
        };

        await userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "User {UserId} created successfully for tenant {TenantId}.",
            user.Id,
            tenantId);

        return ResultDto<UserResponse>.Ok(
            user.ToResponse(),
            "User created successfully.");
    }

    public async Task<ResultDto<UserResponse>> UpdateAsync(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var userRepository =
            _unitOfWork.Repository<User>();

        var user = await userRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return ResultDto<UserResponse>.Failure(
                "User not found.",
                ErrorCode.NotFound);
        }

        var emailExists = await userRepository.ExistsAsync(
            x => x.Email == request.Email &&
                 x.Id != id,
            cancellationToken);

        if (emailExists)
        {
            _logger.LogWarning(
                "User update conflict. Email {Email} already exists.",
                request.Email);

            return ResultDto<UserResponse>.Failure(
                "A user with this email already exists.",
                ErrorCode.Conflict);
        }

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Role = request.Role;

        userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "User {UserId} updated successfully.",
            user.Id);

        return ResultDto<UserResponse>.Ok(
            user.ToResponse(),
            "User updated successfully.");
    }

    public async Task<ResultDto<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var userRepository =
            _unitOfWork.Repository<User>();

        var user = await userRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return ResultDto<bool>.Failure(
                "User not found.",
                ErrorCode.NotFound);
        }

        user.IsActive = false;

        userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "User {UserId} soft-deleted successfully.",
            user.Id);

        return ResultDto<bool>.Ok(
            true,
            "User deleted successfully.");
    }
}