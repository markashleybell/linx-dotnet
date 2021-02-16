using System;
using System.Collections.Generic;
using Linx.Domain;

namespace Linx.Models
{
    public class IndexViewModel
    {
        public int Total { get; set; }

        public int PageSize { get; set; }

        public int Pages { get; set; }

        public int Page { get; set; }

        public IEnumerable<ListViewLink> Links { get; set; }
    }
}
