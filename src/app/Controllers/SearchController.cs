using Linx.Data;
using Linx.Models.Search;
using Linx.Services;
using Linx.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Linx.Controllers
{
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(
            IOptionsMonitor<Settings> optionsMonitor,
            IRepository repository,
            ISearchService searchService)
            : base(
                optionsMonitor,
                repository) =>
            _searchService = searchService;

        [HttpPost]
        public IActionResult Index(IndexViewModel model)
        {
            model.Results = _searchService.Search(UserID, model.Query);

            return View(model);
        }
    }
}
