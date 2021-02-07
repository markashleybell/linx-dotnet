using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Linx.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static Linx.Domain.Constants;

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

        public static object AsObject(this ModelStateDictionary modelState) =>
            modelState
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                .Where(m => m.Value.Any());

        public static (bool success, string apiKey) TryGetApiKey(this HttpContext ctx) =>
            ctx.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey)
                ? (true, apiKey)
                : (false, default);
    }
}
