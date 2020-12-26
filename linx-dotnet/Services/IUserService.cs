using System.Security.Claims;
using System.Threading.Tasks;

namespace Linx.Services
{
    public interface IUserService
    {
        ClaimsPrincipal GetClaimsPrincipal(int id, string email);

        Task<(bool valid, int? id)> ValidateLogin(string email, string password);
    }
}
