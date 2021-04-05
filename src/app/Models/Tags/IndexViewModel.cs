using System.Collections.Generic;
using Linx.Domain;

namespace Linx.Models.Tags
{
    public class IndexViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }

#pragma warning disable IDE0046 // Convert to conditional expression
        public string GetClass(Tag tag)
        {
            if (tag.UseCount >= 20)
            {
                return "badge-tag-xl";
            }

            if (tag.UseCount >= 15)
            {
                return "badge-tag-lg";
            }

            if (tag.UseCount >= 10)
            {
                return "badge-tag-md";
            }

            if (tag.UseCount >= 5)
            {
                return "badge-tag-sm";
            }

            return "badge-tag-xs";
        }
#pragma warning restore IDE0046 // Convert to conditional expression
    }
}
