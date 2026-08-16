using MiniSaaS.Application.Common.Pagination;
using MiniSaaS.Domain.Common;
using System.Linq.Expressions;

namespace MiniSaaS.Application.Common.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id,CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PaginationResult<T>> GetPagedAsync(
        PaginationRequest request,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(T entity,CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellationToken = default);
}