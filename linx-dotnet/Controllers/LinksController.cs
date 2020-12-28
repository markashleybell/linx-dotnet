using System;
using System.Threading.Tasks;
using Linx.Data;
using Linx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Linx.Controllers
{
    [Authorize]
    public class LinksController : Controller
    {
        private readonly IRepository _repository;

        public LinksController(IRepository repository) =>
            _repository = repository;

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel {
                Links = await _repository.ReadAllLinksAsync()
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allTags = await _repository.ReadAllTagsAsync();

            var model = CreateViewModel.From(allTags);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTags = await _repository.ReadAllTagsAsync();

                return View(model);
            }

            var create = CreateViewModel.ToLink(model);

            var link = await _repository.CreateLinkAsync(create);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var link = await _repository.ReadLinkAsync(id);

            if (link == null)
            {
                return NotFound();
            }

            var allTags = await _repository.ReadAllTagsAsync();

            return link != null
                ? (IActionResult)View(UpdateViewModel.From(link, allTags))
                : NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTags = await _repository.ReadAllTagsAsync();

                return View(model);
            }

            var update = UpdateViewModel.ToLink(model);

            var link = await _repository.UpdateLinkAsync(update);

            return RedirectToAction(nameof(Update), new { id = link.ID });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteLinkAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
