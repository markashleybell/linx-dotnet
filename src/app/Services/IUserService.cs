using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Linx.Services
{
    public interface IUserService
    {
        ClaimsPrincipal GetClaimsPrincipal(Guid id, string email);

        Task<(bool valid, Guid? id)> ValidateLogin(string email, string password);
    }
}
