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

    public UserService(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
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
                "A tenant context is required.");
        }

        var tenantId = _tenantContext.TenantId!.Value;

        var userRepository =
            _unitOfWork.Repository<User>();

        var emailExists = await userRepository.ExistsAsync(
            x => x.Email == request.Email,
            cancellationToken);

        if (emailExists)
        {
            return ResultDto<UserResponse>.Failure(
                "A user with this email already exists.");
        }

        var user = new User
        {
            TenantId = tenantId,
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role,
            IsActive = true
        };

        await userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

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
                "User not found.");
        }

        var emailExists = await userRepository.ExistsAsync(
            x => x.Email == request.Email &&
                 x.Id != id,
            cancellationToken);

        if (emailExists)
        {
            return ResultDto<UserResponse>.Failure(
                "A user with this email already exists.");
        }

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Role = request.Role;

        userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

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
                "User not found.");
        }

        user.IsActive = false;

        userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ResultDto<bool>.Ok(
            true,
            "User deleted successfully.");
    }
}