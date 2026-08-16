using MiniSaaS.Application.Common.Interfaces;

namespace MiniSaaS.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    public string UserId => "system";

    public string UserName => "system";

    public bool IsAuthenticated => false;
}
