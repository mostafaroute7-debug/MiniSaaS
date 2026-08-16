namespace MiniSaaS.Application.Common.Interfaces
{
    public interface ITenantReader
    {
        Task<bool> ExistsAndIsActiveAsync(int tenantId,CancellationToken cancellationToken = default);
    }
}
