using System;
using System.Collections.Generic;
using System.Linq;
using Linx.Domain;
using Linx.Functions;

namespace Linx.Models
{
    public class TagMergeViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }

        public Guid TagID { get; set; }

        public string MergeTagIDs { get; set; }

        public IEnumerable<Guid> TagIDsToMerge =>
            !string.IsNullOrWhiteSpace(MergeTagIDs)
                ? MergeTagIDs.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(id => Guid.Parse(id))
                : Enumerable.Empty<Guid>();

        public string TagDataJson =>
            Tags.AsTagJson(t => new { id = t.ID, label = t.Label });
    }
}
