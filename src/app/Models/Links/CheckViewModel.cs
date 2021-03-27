using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Linx.Domain;
using static Linx.Functions.Functions;

namespace Linx.Models
{
    public class CheckViewModel
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        public string Abstract { get; set; }

        public string Tags { get; set; }

        public IEnumerable<Tag> AllTags { get; set; }

        public string TagDataJson =>
            AllTags?.AsTagJson(t => t.Label);

        public static UpdateViewModel From(Link link, IEnumerable<Tag> allTags) =>
            new() {
                ID = link.ID,
                Title = link.Title,
                Url = link.Url,
                Abstract = link.Abstract,
                Tags = string.Join("|", link.Tags.Select(t => t.Label)),
                AllTags = allTags
            };

        public static Link ToLink(UpdateViewModel model) =>
            new(
                model.ID,
                model.Title,
                model.Url,
                model.Abstract,
                (model.Tags ?? string.Empty)
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => new Tag(t))
            );
    }
}
