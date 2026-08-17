using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Infrastructure.BackgroundJobs;
using MiniSaaS.Infrastructure.Identity;
using MiniSaaS.Infrastructure.MultiTenancy;
using MiniSaaS.Infrastructure.Persistence.Contexts;
using UnitOfWorkImplementation = MiniSaaS.Infrastructure.UnitOfWork.UnitOfWork;
using Microsoft.Extensions.Options;
using MiniSaaS.Infrastructure.Authentication;
namespace MiniSaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITenantContext, TenantContext>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantReader, TenantReader>();
        services.AddScoped<IUnitOfWork, UnitOfWorkImplementation>();

        services.AddScoped<IActiveUsersJob, ActiveUsersJob>();

        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();

            config.UseSqlServerStorage(
                configuration.GetConnectionString(
                    "DefaultConnection"));
        });

        services.AddHangfireServer();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<ITokenService,JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        return services;
    }
}
