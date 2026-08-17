using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MiniSaaS.Application.Auth.Services;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Application.Tenants.Services;
using MiniSaaS.Application.Users.Services;
namespace MiniSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ITenantService,TenantService>();

        services.AddScoped<IUserService,UserService>();

        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
