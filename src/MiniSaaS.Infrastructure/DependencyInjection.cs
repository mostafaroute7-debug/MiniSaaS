using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Infrastructure.Identity;
using MiniSaaS.Infrastructure.MultiTenancy;
using MiniSaaS.Infrastructure.Persistence.Contexts;
using UnitOfWorkImplementation = MiniSaaS.Infrastructure.UnitOfWork.UnitOfWork;
namespace MiniSaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<TenantContext>();

        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

        services.AddScoped<ITenantContextAccessor>(sp => sp.GetRequiredService<TenantContext>());

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantReader, TenantReader>();
        services.AddScoped<IUnitOfWork, UnitOfWorkImplementation>();

        return services;
    }
}
