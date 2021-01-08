using System.Threading.Tasks;
using Linx.Models;
using Linx.Services;
using Linx.Support;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Linx.Domain.Constants;

namespace Linx.Controllers
{
    public class UsersController : Controller
    {
        private readonly Settings _cfg;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUserService _userService;

        public UsersController(
            IOptionsMonitor<Settings> optionsMonitor,
            IDateTimeService dateTimeService,
            IUserService userService)
        {
            _cfg = optionsMonitor.CurrentValue;
            _dateTimeService = dateTimeService;
            _userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (valid, id) = await _userService.ValidateLogin(model.Email, model.Password);

            if (!valid)
            {
                return View(model);
            }

            var principal = _userService.GetClaimsPrincipal(id.Value, model.Email);

            var authenticationProperties = new AuthenticationProperties {
                IsPersistent = true,
                ExpiresUtc = _dateTimeService.Now.AddDays(_cfg.PersistentSessionLengthInDays)
            };

            await HttpContext.SignInAsync(principal, authenticationProperties);

            var returnUrl = !string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
                ? model.ReturnUrl
                : SiteRootUrl;

            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect(SiteRootUrl);
        }
    }
}
