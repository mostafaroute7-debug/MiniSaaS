using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MiniSaaS.Application.Tenants.Services;
namespace MiniSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddScoped<ITenantService,TenantService>();

        services.AddScoped<
            Users.Services.IUserService,
            Users.Services.UserService>();

        return services;
    }
}
