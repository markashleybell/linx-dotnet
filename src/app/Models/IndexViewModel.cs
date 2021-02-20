using System;
using System.Collections.Generic;
using System.Linq;
using Linx.Data;
using Linx.Domain;
using Microsoft.AspNetCore.Html;

namespace Linx.Models
{
    public class IndexViewModel
    {
        public int Total { get; set; }

        public int PageSize { get; set; }

        public int Pages { get; set; }

        public int Page { get; set; }

        public SortColumn Sort { get; set; }

        public SortDirection SortDirection { get; set; }

        public string Query { get; set; }

        public IEnumerable<ListViewLink> Links { get; set; }

        public HtmlString PageLinkWith(
            int? page = null,
            int? pageSize = null,
            SortColumn? sort = null,
            SortDirection? sortDirection = null,
            string query = null)
        {
            var parameters = new Dictionary<string, object> {
                { "page", page ?? Page },
                { "pageSize", pageSize ?? PageSize },
                { "sort", sort ?? Sort },
                { "sortDirection", sortDirection ?? SortDirection },
                { "query", query ?? Query }
            };

            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));

            return new HtmlString("?" + queryString);
        }

        public string PageSizeLinkClassesFor(int pageSize) =>
            pageSize == PageSize
                ? "btn btn-primary active"
                : "btn btn-outline-primary";

        public string SortLinkClassesFor(SortColumn sort, SortDirection sortDirection) =>
            sort == Sort && sortDirection == SortDirection
                ? "btn btn-primary active"
                : "btn btn-outline-primary";
    }
}
