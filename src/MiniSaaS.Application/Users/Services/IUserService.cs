
using MiniSaaS.Application.Common.Models;
using MiniSaaS.Application.Users.DTOs;

namespace MiniSaaS.Application.Users.Services;

public interface IUserService
{
    Task<ResultDto<PagedResultDto<UserResponse>>> GetAllAsync( PaginationRequest request, CancellationToken cancellationToken = default);
    Task<ResultDto<UserResponse>> CreateAsync(CreateUserRequest request,CancellationToken cancellationToken = default);
    Task<ResultDto<UserResponse>> UpdateAsync(int id,UpdateUserRequest request,CancellationToken cancellationToken = default);
    Task<ResultDto<bool>> DeleteAsync(int id,CancellationToken cancellationToken = default);
}