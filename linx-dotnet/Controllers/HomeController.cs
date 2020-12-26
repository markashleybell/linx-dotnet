using Linx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Linx.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) =>
            _logger = logger;

        [Authorize]
        public IActionResult Index()
        {
            var model = new IndexViewModel();

            return View(model);
        }
    }
}
