using Microsoft.EntityFrameworkCore;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Domain.Entities;
using MiniSaaS.Domain.Enums;
using MiniSaaS.Infrastructure.Persistence.Contexts;

namespace MiniSaaS.Infrastructure.Persistence.Seed;


public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context,IPasswordHasher passwordHasher,CancellationToken cancellationToken = default)
    {
        var migrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        var pendingMigrations = migrations.ToList();
        if (pendingMigrations.Count>0)
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
            


        var tenant = await context.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Slug == "demo-tenant",cancellationToken);

        if (tenant is null)
        {
            tenant = new Tenant
            {
                Name = "Demo Tenant",
                Slug = "demo-tenant",
                IsActive = true
            };

            await context.Tenants.AddAsync(tenant,cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        var adminExists = await context.Users.IgnoreQueryFilters().AnyAsync(x =>x.Email == "admin@minisaas.com" &&x.TenantId == tenant.Id, cancellationToken);

        if (!adminExists)
        {
            var adminUser = new User
            {
                TenantId = tenant.Id,
                FullName = "System Admin",
                Email = "admin@minisaas.com",
                PasswordHash = passwordHasher.Hash( "Admin@123456"),
                Role = UserRole.Admin,
                IsActive = true
            };

            await context.Users.AddAsync(adminUser,cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}