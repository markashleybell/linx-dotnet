using System.Linq;
using System.Threading.Tasks;
using Linx.Data;
using Linx.Functions;
using Linx.Models;
using Linx.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
    public class ApiController : Controller
    {
        public ApiController(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository)
        {
            Settings = optionsMonitor.CurrentValue;
            Repository = repository;
        }

        protected Settings Settings { get; }

        protected IRepository Repository { get; }

        [RequireApiKey]
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.AsObject() });
            }

            var tmp = 0;

            return Json(new { success = true });
        }

        [RequireApiKey]
        [HttpPost]
        public async Task<IActionResult> Tags()
        {
            // We already know the key is present because of [RequireApiKey],
            // so we can ignore the first parameter (success) here
            var (_, apiKey) = HttpContext.TryGetApiKey();

            var user = await Repository.FindUserByApiKey(apiKey);

            var tags = await Repository.ReadAllTagsAsync(user.ID);

            var tagLabels = tags.Select(t => t.Label);

            return Json(tagLabels);
        }
    }
}
