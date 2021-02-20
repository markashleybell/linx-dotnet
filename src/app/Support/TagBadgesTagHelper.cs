using System;
using System.Collections.Generic;
using System.Linq;
using Linx.Domain;
using Linx.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Linx.Support
{
    public class TagBadgesTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;

        public TagBadgesTagHelper(IHtmlGenerator htmlGenerator) =>
            _htmlGenerator = htmlGenerator;

        public PaginationDetails Pagination { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public bool ReadOnly { get; set; }

        public string ContainerTag { get; set; }
            = "div";

        public string ContainerClasses { get; set; }

        public string BadgeClasses { get; set; }
            = "badge bg-primary";

        public string BadgeClassesReadOnly { get; set; }
            = "badge bg-secondary";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = ContainerTag;

            if (!string.IsNullOrWhiteSpace(ContainerClasses))
            {
                output.Attributes.Add("class", ContainerClasses);
            }

            foreach (var link in Tags.OrderBy(t => t.Label).Select(t => ReadOnly ? TagBadge(t) : TagSearchActionLink(t)))
            {
                output.Content.AppendHtml(link);
            }

            output.TagMode = TagMode.StartTagAndEndTag;
        }

        private TagBuilder TagSearchActionLink(Tag tag)
        {
            // Append the tag for this link to the tags already being queried for
            var currentTags = Pagination?.Tags ?? Enumerable.Empty<Tag>();

            // Don't re-add this tag if it's already in the query
            var queryTags = !currentTags.Any(t => t.Label.Equals(tag.Label, StringComparison.OrdinalIgnoreCase))
                ? currentTags.Append(tag)
                : currentTags;

            return _htmlGenerator.GenerateActionLink(
                viewContext: ViewContext,
                linkText: tag.Label,
                actionName: "Index",
                controllerName: "Links",
                protocol: null,
                hostname: null,
                fragment: null,
                routeValues: new { query = string.Join(" ", queryTags.Select(t => $"[{t.Label}]")) },
                htmlAttributes: new { @class = BadgeClasses }
            );
        }

        private TagBuilder TagBadge(Tag tag)
        {
            var tagSpan = new TagBuilder("span");

            tagSpan.Attributes["class"] = BadgeClassesReadOnly;

            tagSpan.InnerHtml.Append(tag.Label);

            return tagSpan;
        }
    }
}
