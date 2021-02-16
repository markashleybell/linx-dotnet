using System;
using System.Threading.Tasks;
using Linx.Data;
using Linx.Functions;
using Linx.Models;
using Linx.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
    public class LinksController : ControllerBase
    {
        public LinksController(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository)
            : base(
                optionsMonitor,
                repository)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page = 1)
        {
            var links = await Repository.ReadLinksAsync(UserID, page.Value, 5, SortColumn.Created, SortDirection.Descending);

            var model = new IndexViewModel {
                Links = links
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
                ? (IActionResult)View(UpdateViewModel.From(link, allTags))
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

            return RedirectToAction(nameof(Update), new { id = link.ID });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.DeleteLinkAsync(UserID, id);

            return RedirectToAction(nameof(Index));
        }
    }
}
