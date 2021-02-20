using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Linx.Data;
using Linx.Domain;
using Microsoft.AspNetCore.Html;

namespace Linx.Models
{
    public class IndexViewModel
    {
        public PaginationDetails Pagination { get; set; }

        public IEnumerable<ListViewLinkViewModel> Links { get; set; }

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

        public string PageSizeLinkClassesFor(int pageSize) =>
            pageSize == Pagination.PageSize
                ? "btn btn-primary active"
                : "btn btn-outline-primary";

        public string SortLinkClassesFor(SortColumn sort, SortDirection sortDirection) =>
            sort == Pagination.Sort && sortDirection == Pagination.SortDirection
                ? "btn btn-primary active"
                : "btn btn-outline-primary";
    }
}
