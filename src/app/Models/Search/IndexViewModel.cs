using System;
using System.Collections.Generic;
using System.Linq;
using Linx.Data;
using Linx.Domain;
using Linx.Models.Shared;
using Linx.Services;

namespace Linx.Models.Search
{
    public class IndexViewModel
    {
        public string Query { get; set; }

        public PaginationDetails Pagination =>
            new() {
                Total = Results.Count(),
                Pages = 1,
                PageSize = 100,
                Page = 1,
                Sort = SortColumn.Created,
                SortDirection = SortDirection.Descending,
                Tags = Enumerable.Empty<Tag>()
            };

        public IEnumerable<SearchResult> Results { get; set; }
            = Enumerable.Empty<SearchResult>();

        public string PageLinkWith(
            int? page = null,
            int? pageSize = null,
            SortColumn? sort = null,
            SortDirection? sortDirection = null,
            string query = null)
        {
            var parameters = new Dictionary<string, object> {
                { "page", page ?? Pagination.Page },
                { "pageSize", pageSize ?? Pagination.PageSize },
                { "sort", sort ?? Pagination.Sort },
                { "sortDirection", sortDirection ?? Pagination.SortDirection },
                { "query", Uri.EscapeDataString(query ?? Pagination.Query) }
            };

            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));

            return "?" + queryString;
        }
    }
}
