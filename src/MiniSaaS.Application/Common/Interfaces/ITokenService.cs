
namespace MiniSaaS.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(int userId,int tenantId,string email,string role);
}
