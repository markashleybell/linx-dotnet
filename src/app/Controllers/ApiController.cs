using System;
using System.Linq;
using System.Threading.Tasks;
using Linx.Data;
using Linx.Functions;
using Linx.Models;
using Linx.Models.Links;
using Linx.Support;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Linx.Domain.Constants;

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

            var apiKey = GetApiKey();

            var user = await Repository.FindUserByApiKey(apiKey);

            if (user is null)
            {
                return Unauthorized();
            }

            var create = CreateViewModel.ToLink(model);

            await Repository.CreateLinkAsync(user.ID, create);

            return Json(new { success = true });
        }

        [RequireApiKey]
        [HttpPost]
        public async Task<IActionResult> Tags()
        {
            var apiKey = GetApiKey();

            var user = await Repository.FindUserByApiKey(apiKey);

            var tags = await Repository.ReadAllTagsAsync(user.ID);

            var tagLabels = tags.Select(t => t.Label);

            return Json(tagLabels);
        }

        [EnableCors(AllowAnyOrigin)]
        [RequireApiKey]
        [HttpPost]
        public async Task<IActionResult> Check(CheckViewModel model)
        {
            var apiKey = GetApiKey();

            var user = await Repository.FindUserByApiKey(apiKey);

            var exists = await Repository.CheckIfLinkExistsByUrlPrefix(user.ID, model.Url);

            return Json(new { exists });
        }

        private string GetApiKey()
        {
            var (success, apiKey) = HttpContext.TryGetApiKey();

            return !success ? throw new UnauthorizedAccessException() : apiKey;
        }
    }
}
