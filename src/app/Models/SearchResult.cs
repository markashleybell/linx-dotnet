using System;
using Linx.Domain;

namespace Linx.Models
{
    public class SearchResult
    {
        public SearchResult(float score, Link document)
        {
            Score = score;
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public float Score { get; }

        public Link Document { get; }
    }
}
