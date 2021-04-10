using System.Threading.Tasks;
using Linx.Data;
using Linx.Models.Tags;
using Linx.Services;
using Linx.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
    public class TagsController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public TagsController(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository,
            ISearchService searchService)
            : base(
                optionsMonitor,
                repository) =>
            _searchService = searchService;

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

            var (_, _, links) = await Repository.ReadLinksFullAsync(UserID, 1, 9999, SortColumn.Created, SortDirection.Descending);

            _searchService.DeleteAndRebuildIndex(UserID, links);

            return RedirectToAction(nameof(Merge));
        }
    }
}
