
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Infrastructure.Persistence.Contexts;

namespace MiniSaaS.Infrastructure.BackgroundJobs;

public class ActiveUsersJob : IActiveUsersJob
{
    private readonly AppDbContext _context;
    private readonly ILogger<ActiveUsersJob> _logger;

    public ActiveUsersJob(AppDbContext context,ILogger<ActiveUsersJob> logger)
    {
        _context = context;
        _logger = logger;
    }
    [DisableConcurrentExecution(55)]
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Active users job started.");

        var results = await _context.Tenants
                    .AsNoTracking()
                    .Select(tenant => new
                    {
                        TenantId = tenant.Id,
                        ActiveUsersCount = _context.Users
                        .IgnoreQueryFilters()
                        .Count(user =>
                        user.TenantId == tenant.Id &&
                        user.IsActive)
                    }).OrderBy(x => x.TenantId)
                    .ToListAsync(cancellationToken);

        foreach (var result in results)
        {
            _logger.LogInformation("Tenant {TenantId} has {ActiveUsersCount} active users.", result.TenantId,result.ActiveUsersCount);
        }

        _logger.LogInformation("Active users job completed. Processed {TenantCount} tenants.",results.Count);
    }
}
