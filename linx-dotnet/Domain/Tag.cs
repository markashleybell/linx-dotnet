using System;
using System.Text.RegularExpressions;

namespace Linx.Domain
{
    public class Tag
    {
        public Tag(string label)
            : this(default, label)
        {
        }

        public Tag(int? id, string label)
            : this(id, label, default)
        {
        }

        public Tag(int? id, string label, int? useCount)
        {
            if (label is null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentOutOfRangeException(nameof(label), "Tag labels cannot be empty");
            }

            ID = id;
            Label = Regex.Replace(label.Trim(), @"\s+", "-").ToLowerInvariant();
            UseCount = useCount ?? 0;
        }

        public int? ID { get; }

        public string Label { get; }

        public int UseCount { get; }
    }
}
