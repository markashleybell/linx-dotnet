using System;
using Linx.Domain;

namespace Linx.Services
{
    public class SearchResult
    {
        public SearchResult(float score, ListViewLink document)
        {
            Score = score;
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public float Score { get; }

        public ListViewLink Document { get; }
    }
}
