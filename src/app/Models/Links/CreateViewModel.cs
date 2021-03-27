using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Linx.Domain;
using static Linx.Functions.Functions;

namespace Linx.Models
{
    public class CreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        public string Abstract { get; set; }

        public string Tags { get; set; }

        public IEnumerable<Tag> AllTags { get; set; }

        public string TagDataJson =>
            AllTags?.AsTagJson(t => t.Label);

        public static CreateViewModel From(IEnumerable<Tag> allTags) =>
            new() {
                AllTags = allTags
            };

        public static Link ToLink(CreateViewModel model) =>
            new(
                Guid.NewGuid(),
                model.Title,
                model.Url,
                model.Abstract,
                (model.Tags ?? string.Empty)
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => new Tag(t))
            );
    }
}
