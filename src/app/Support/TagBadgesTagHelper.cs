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

        public bool Remove { get; set; }

        public string ContainerTag { get; set; }
            = "div";

        public string ContainerClasses { get; set; }

        public string BadgeClasses { get; set; }
            = "badge bg-secondary";

        public bool CopyButton { get; set; }

        public string CopyButtonClasses { get; set; }
            = "btn btn-link btn-copy btn-copy-tags";

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

            Func<Tag, TagBuilder> html =
                ReadOnly ? TagBadge : Remove ? RemoveTagLinkBadge : AddTagLinkBadge;

            foreach (var t in Tags.OrderBy(t => t.Label).Select(html))
            {
                output.Content.AppendHtml(t);
            }

            if (CopyButton)
            {
                var tagImg = new TagBuilder("img");

                tagImg.Attributes["src"] = "/img/clippy.svg";
                tagImg.Attributes["alt"] = "Copy Tags";

                var tagButton = new TagBuilder("button");

                tagButton.Attributes["class"] = CopyButtonClasses;
                tagButton.Attributes["data-clipboard-text"] = string.Join("|", Tags.OrderBy(t => t.Label).Select(t => t.Label));

                tagButton.InnerHtml.AppendHtml(tagImg);

                output.Content.AppendHtml(tagButton);
            }

            output.TagMode = TagMode.StartTagAndEndTag;
        }

        private TagBuilder AddTagLinkBadge(Tag tag)
        {
            var currentTags = Pagination?.Tags ?? Enumerable.Empty<Tag>();

            // Append the tag for this link to the tags already in
            // the query, but don't re-add it if it's already there
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

        private TagBuilder RemoveTagLinkBadge(Tag tag)
        {
            var currentTags = Pagination?.Tags ?? Enumerable.Empty<Tag>();

            // Remove this tag from the query
            var queryTags = currentTags.Where(t => !t.Label.Equals(tag.Label, StringComparison.OrdinalIgnoreCase));

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

            tagSpan.Attributes["class"] = BadgeClasses;

            tagSpan.InnerHtml.Append(tag.Label);

            return tagSpan;
        }
    }
}
