using System;
using System.Collections.Generic;
using static Linx.Functions.Functions;

namespace Linx.Domain
{
    public class ListViewLink
    {
        public ListViewLink(
            Guid id,
            string title,
            string url,
            string @abstract,
            string tags,
            DateTime created,
            DateTime updated)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentOutOfRangeException(nameof(title), "Title cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentOutOfRangeException(nameof(url), "URL cannot be empty");
            }

            ID = id;
            Title = title;
            Url = url;
            Abstract = @abstract;
            Tags = TagList(tags);
            Created = created;
            Updated = updated;
        }

        public Guid ID { get; }

        public string Title { get; }

        public string Url { get; }

        public string Abstract { get; }

        public IEnumerable<Tag> Tags { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
