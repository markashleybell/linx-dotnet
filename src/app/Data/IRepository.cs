using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Linx.Domain;

namespace Linx.Data
{
    public interface IRepository
    {
        Task<Link> CreateLinkAsync(Guid userID, Link link);

        Task<Link> ReadLinkAsync(Guid userID, Guid id);

        Task<IEnumerable<ListViewLink>> ReadLinksAsync(
            Guid userID,
            int page,
            int pageSize,
            SortColumn sortBy,
            SortDirection sortDirection);

        Task<Link> UpdateLinkAsync(Guid userID, Link link);

        Task DeleteLinkAsync(Guid userID, Guid id);

        Task<IEnumerable<Tag>> ReadAllTagsAsync(Guid userID);

        Task MergeTagsAsync(Guid userID, Guid id, IEnumerable<Guid> tagIdsToMerge);

        Task<User> FindUserByEmail(string email);

        Task<User> FindUserByApiKey(string apiKey);
    }
}
