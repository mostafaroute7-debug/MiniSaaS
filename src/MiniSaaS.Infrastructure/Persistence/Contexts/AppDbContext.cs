
using Microsoft.EntityFrameworkCore;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Domain.Common;
using MiniSaaS.Domain.Entities;

namespace MiniSaaS.Infrastructure.Persistence.Contexts;

public class AppDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options,ICurrentUserService currentUserService,
                                ITenantContext tenantContext) : base(options)
    {
        _currentUserService =  currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<User>()
         .HasQueryFilter(x =>
             x.TenantId == _tenantContext.TenantId &&
             x.IsActive);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        var currentUser = _currentUserService.UserName;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = string.IsNullOrWhiteSpace(currentUser)? "system" : currentUser;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = string.IsNullOrWhiteSpace(currentUser) ? "system" : currentUser;
            }
        }
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
}
