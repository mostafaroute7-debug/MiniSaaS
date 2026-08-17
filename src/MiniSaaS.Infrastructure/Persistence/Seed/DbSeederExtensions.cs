
using Microsoft.Extensions.DependencyInjection;
using MiniSaaS.Infrastructure.Persistence.Contexts;

namespace MiniSaaS.Infrastructure.Persistence.Seed;

public static class DbSeederExtensions
{
    public static async Task SeedDatabaseAsync(this IServiceProvider services,CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var passwordHasher = scope.ServiceProvider.GetRequiredService<Application.Common.Interfaces.IPasswordHasher>();

        await DbSeeder.SeedAsync(context,passwordHasher,cancellationToken);
    }
}