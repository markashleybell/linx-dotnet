using System;
using System.Linq;
using System.Threading.Tasks;
using Linx.Data;
using Linx.Models.Links;
using Linx.Models.Shared;
using Linx.Services;
using Linx.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Linx.Functions.Functions;

namespace Linx.Controllers
{
    public class LinksController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public LinksController(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository,
            ISearchService searchService)
            : base(
                optionsMonitor,
                repository) =>
            _searchService = searchService;

        [HttpGet]
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            SortColumn sort = SortColumn.Created,
            SortDirection sortDirection = SortDirection.Descending,
            string query = null)
        {
            var (parsed, terms, tags) = ParseSearchQuery(query);

            var (total, pageCount, links) = await Repository.ReadLinksAsync(UserID, page, pageSize, sort, sortDirection, tags);

            var pagination = new PaginationDetails {
                Total = total,
                Pages = pageCount,
                PageSize = pageSize,
                Page = page,
                Sort = sort,
                SortDirection = sortDirection,
                Tags = tags
            };

            var model = new IndexViewModel {
                Pagination = pagination,
                Links = links.Select(l => new ListViewLinkViewModel { Link = l, Pagination = pagination })
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allTags = await Repository.ReadAllTagsAsync(UserID);

            var model = CreateViewModel.From(allTags);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTags = await Repository.ReadAllTagsAsync(UserID);

                return View(model);
            }

            var create = CreateViewModel.ToLink(model);

            var link = await Repository.CreateLinkAsync(UserID, create);

            _searchService.AddLink(UserID, link);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var link = await Repository.ReadLinkAsync(UserID, id);

            if (link == null)
            {
                return NotFound();
            }

            var allTags = await Repository.ReadAllTagsAsync(UserID);

            return link != null
                ? View(UpdateViewModel.From(link, allTags))
                : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTags = await Repository.ReadAllTagsAsync(UserID);

                return View(model);
            }

            var update = UpdateViewModel.ToLink(model);

            var link = await Repository.UpdateLinkAsync(UserID, update);

            _searchService.UpdateLink(UserID, link);

            return RedirectToAction(nameof(Update), new { id = link.ID });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.DeleteLinkAsync(UserID, id);

            _searchService.RemoveLink(UserID, id);

            return RedirectToAction(nameof(Index));
        }
    }
}
