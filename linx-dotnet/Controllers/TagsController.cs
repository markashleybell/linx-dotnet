using System.Threading.Tasks;
using Linx.Data;
using Linx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Linx.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly IRepository _repository;

        public TagsController(IRepository repository) =>
            _repository = repository;

        public async Task<IActionResult> Index()
        {
            var model = new TagIndexViewModel {
                Tags = await _repository.GetTags()
            };

            return View(model);
        }

        public async Task<IActionResult> Manage()
        {
            var model = new TagMergeViewModel {
                Tags = await _repository.GetTags()
            };

            return View(model);
        }

        public async Task<IActionResult> Merge(TagMergeViewModel model)
        {
            await _repository.MergeTags(model.TagID, model.TagIDsToMerge);

            return RedirectToAction(nameof(Manage));
        }
    }
}
