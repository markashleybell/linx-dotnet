using System;
using System.Collections.Generic;
using System.Linq;
using static Linx.Functions.Functions;

namespace Linx.Domain
{
    public class Link
    {
        public Link(Guid id, string title, string url, string @abstract, string tags)
            : this(id, title, url, @abstract, TagList(tags))
        {
        }

        public Link(Guid id, string title, string url, string @abstract, IEnumerable<Tag> tags)
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
            Tags = tags ?? Enumerable.Empty<Tag>();
        }

        public Guid ID { get; }

        public string Title { get; }

        public string Url { get; }

        public string Abstract { get; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}
