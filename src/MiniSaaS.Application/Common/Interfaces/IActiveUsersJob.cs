
namespace MiniSaaS.Application.Common.Interfaces;

public interface IActiveUsersJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
