namespace MiniSaaS.Application.Common.Models;

public sealed class PaginationRequest
{
    private const int MaxPageSize = 100;
    public int PageNumber { get; init; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize =value <= 0? 10 : Math.Min(value, MaxPageSize);
    }
    public int Skip =>(PageNumber - 1) * PageSize;
}
