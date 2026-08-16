
using MiniSaaS.Application.Tenants.DTOs;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Domain.Entities;

namespace MiniSaaS.Application.Common.Mapping;

public static class MappingExtensions
{
    public static TenantResponse ToResponse(
       this Tenant tenant)
    {
        return new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt
        };
    }

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            TenantId = user.TenantId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
