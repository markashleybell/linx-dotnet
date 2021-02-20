using System.Collections.Generic;
using System.Linq;
using Linx.Data;
using Linx.Domain;

namespace Linx.Models
{
    public class PaginationDetails
    {
        public int Total { get; set; }

        public int PageSize { get; set; }

        public int Pages { get; set; }

        public int Page { get; set; }

        public SortColumn Sort { get; set; }

        public SortDirection SortDirection { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public string Query =>
            string.Join(" ", Tags.Select(t => $"[{t.Label}]"));
    }
}
