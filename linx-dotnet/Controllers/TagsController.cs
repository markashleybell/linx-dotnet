using System.Threading.Tasks;
using Linx.Data;
using Linx.Models;
using Linx.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
    [Authorize]
    public class TagsController : ControllerBase
    {
        public TagsController(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository)
            : base(
                optionsMonitor,
                repository)
        {
        }

        public async Task<IActionResult> Index()
        {
            var model = new TagIndexViewModel {
                Tags = await Repository.ReadAllTagsAsync(UserID)
            };

            return View(model);
        }

        public async Task<IActionResult> Manage()
        {
            var model = new TagMergeViewModel {
                Tags = await Repository.ReadAllTagsAsync(UserID)
            };

            return View(model);
        }

        public async Task<IActionResult> Merge(TagMergeViewModel model)
        {
            await Repository.MergeTagsAsync(UserID, model.TagID, model.TagIDsToMerge);

            return RedirectToAction(nameof(Manage));
        }
    }
}
