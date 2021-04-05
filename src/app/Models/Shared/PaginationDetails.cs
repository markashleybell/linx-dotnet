using System.Collections.Generic;
using System.Linq;
using Linx.Data;
using Linx.Domain;

namespace Linx.Models.Shared
{
    public class PaginationDetails
    {
        public int Total { get; set; }

        public int PageSize { get; set; }

        public int Pages { get; set; }

        public int Page { get; set; }

        public int DirectPageLinksEitherSideOfCurrent { get; set; } = 5;

        public IEnumerable<int> DirectPageLinksBefore
        {
            get
            {
                var start = Page - DirectPageLinksEitherSideOfCurrent;

                return start < 1
                    ? Enumerable.Range(1, DirectPageLinksEitherSideOfCurrent + start - 1)
                    : Enumerable.Range(start, DirectPageLinksEitherSideOfCurrent);
            }
        }

        public IEnumerable<int> DirectPageLinksAfter
        {
            get
            {
                var end = Page + DirectPageLinksEitherSideOfCurrent;

                return end > Pages
                    ? Enumerable.Range(Page + 1, Pages - Page)
                    : Enumerable.Range(Page + 1, DirectPageLinksEitherSideOfCurrent);
            }
        }

        public SortColumn Sort { get; set; }

        public SortDirection SortDirection { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public string Query =>
            string.Join(" ", Tags.Select(t => $"[{t.Label}]"));
    }
}
