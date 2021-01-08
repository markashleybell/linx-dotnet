using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Linx.Domain;

namespace Linx.Functions
{
    public static class Functions
    {
        public static IEnumerable<Tag> TagList(string tags) =>
            !string.IsNullOrWhiteSpace(tags)
                ? tags.Split('|').Select(t => new Tag(t))
                : Enumerable.Empty<Tag>();

        public static string AsTagJson(this IEnumerable<Tag> tags, Func<Tag, object> transform) =>
            JsonSerializer.Serialize(tags.Select(transform));
    }
}
