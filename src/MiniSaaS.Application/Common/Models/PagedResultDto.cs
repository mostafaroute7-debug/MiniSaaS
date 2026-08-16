
namespace MiniSaaS.Application.Common.Models;

public sealed class PagedResultDto<T>
{
    public IReadOnlyCollection<T> Items { get; init; }= Array.Empty<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages =>PageSize <= 0? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage =>PageNumber < TotalPages;
}
