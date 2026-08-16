using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Domain.Common;
using MiniSaaS.Infrastructure.Persistence.Contexts;
using MiniSaaS.Infrastructure.Repositories;

namespace MiniSaaS.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = [];
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);

        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new Repository<T>(_context);
            _repositories[type] = repository;
        }

        return (IRepository<T>)repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
