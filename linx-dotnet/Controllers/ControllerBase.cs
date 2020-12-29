using System;
using System.Security.Claims;
using Linx.Data;
using Linx.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
    [Authorize]
    public class ControllerBase : Controller
    {
        public ControllerBase(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository)
        {
            Settings = optionsMonitor.CurrentValue;
            Repository = repository;
        }

        protected Settings Settings { get; }

        protected IRepository Repository { get; }

        protected Guid UserID =>
            Guid.TryParse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value, out var id) ? id : throw new Exception("User is not logged in");
    }
}
