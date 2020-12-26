using System;
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

        public ClaimsPrincipal GetClaimsPrincipal(int id, string email)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Sid, id.ToString(), ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Member")
            };

            var identity = new ClaimsIdentity(
                claims: claims,
                authenticationType: CookieAuthenticationDefaults.AuthenticationScheme,
                nameType: ClaimTypes.Email,
                roleType: ClaimTypes.Role
            );

            return new ClaimsPrincipal(identity);
        }

        public async Task<(bool valid, int? id)> ValidateLogin(string email, string password)
        {
            var user = await _repository.FindUserByEmail(email);

            if (user == null)
            {
                return (false, default(int?));
            }

            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? (true, user.ID) : (false, default(int?));
        }
    }
}
