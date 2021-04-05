using System.Threading.Tasks;
using Linx.Data;
using Linx.Models.Tags;
using Linx.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel {
                Tags = await Repository.ReadAllTagsAsync(UserID)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Merge()
        {
            var model = new MergeViewModel {
                Tags = await Repository.ReadAllTagsAsync(UserID)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Merge(MergeViewModel model)
        {
            await Repository.MergeTagsAsync(UserID, model.TagID, model.TagIDsToMerge);

            return RedirectToAction(nameof(Merge));
        }
    }
}
