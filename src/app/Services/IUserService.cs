using System.Security.Claims;
using System.Threading.Tasks;
using Linx.Domain;

namespace Linx.Services
{
    public interface IUserService
    {
        ClaimsPrincipal GetClaimsPrincipal(User user);

        Task<(bool valid, User user)> ValidateLogin(string email, string password);

        Task<(bool valid, User user)> ValidateApiKey(string apiKey);
    }
}
