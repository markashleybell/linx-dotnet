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

        Task<(int total, int pageCount, IEnumerable<ListViewLink> page)> ReadLinksAsync(
            Guid userID,
            int page,
            int pageSize,
            SortColumn sortBy,
            SortDirection sortDirection,
            IEnumerable<Tag> tags = null);

        Task<(int total, int pageCount, IEnumerable<Link> page)> ReadLinksFullAsync(
            Guid userID,
            int page,
            int pageSize,
            SortColumn sortBy,
            SortDirection sortDirection,
            IEnumerable<Tag> tags = null);

        Task<Link> UpdateLinkAsync(Guid userID, Link link);

        Task DeleteLinkAsync(Guid userID, Guid id);

        Task<bool> CheckIfLinkExistsByUrlPrefix(Guid userID, string urlPrefix);

        Task<IEnumerable<Tag>> ReadAllTagsAsync(Guid userID);

        Task MergeTagsAsync(Guid userID, Guid id, IEnumerable<Guid> tagIdsToMerge);

        Task<User> FindUserByEmail(string email);

        Task<User> FindUserByApiKey(string apiKey);
    }
}
