using System.Security.Claims;
using System.Threading.Tasks;
using Linx.Data;
using Linx.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Linx.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository) =>
            _repository = repository;

        public ClaimsPrincipal GetClaimsPrincipal(User user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Sid, user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Member"),
                new Claim(ClaimTypes.UserData, user.ApiKey)
            };

            var identity = new ClaimsIdentity(
                claims: claims,
                authenticationType: CookieAuthenticationDefaults.AuthenticationScheme,
                nameType: ClaimTypes.Email,
                roleType: ClaimTypes.Role
            );

            return new ClaimsPrincipal(identity);
        }

        public async Task<(bool valid, User user)> ValidateLogin(string email, string password)
        {
            var user = await _repository.FindUserByEmail(email);

            if (user is null)
            {
                return (false, default(User));
            }

            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? (true, user) : (false, default(User));
        }

        public async Task<(bool valid, User user)> ValidateApiKey(string apiKey)
        {
            var user = await _repository.FindUserByApiKey(apiKey);

            return user is null
                ? (false, default(User))
                : (true, user);
        }
    }
}
