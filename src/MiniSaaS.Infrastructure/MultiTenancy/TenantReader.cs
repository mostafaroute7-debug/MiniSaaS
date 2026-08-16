
using Microsoft.EntityFrameworkCore;
using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Infrastructure.Persistence.Contexts;

namespace MiniSaaS.Infrastructure.MultiTenancy;

public class TenantReader : ITenantReader
{
    private readonly AppDbContext _dbContext;

    public TenantReader(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAndIsActiveAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Tenants
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == tenantId && x.IsActive,
                cancellationToken);
    }
}
