using System;
using System.Collections.Generic;
using Linx.Domain;

namespace Linx.Services
{
    public interface ISearchService
    {
        void DeleteAndRebuildIndex(Guid userID, IEnumerable<Link> links);

        void AddLink(Guid userID, Link link);

        void UpdateLink(Guid userID, Link link);

        void RemoveLink(Guid userID, Link link);

        void RemoveLink(Guid userID, Guid linkID);

        IEnumerable<SearchResult> Search(Guid userID, string query);
    }
}
