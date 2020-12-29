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

        Task<IEnumerable<Link>> ReadAllLinksAsync(Guid userID);

        Task<Link> UpdateLinkAsync(Guid userID, Link link);

        Task DeleteLinkAsync(Guid userID, Guid id);

        Task<IEnumerable<Tag>> ReadAllTagsAsync(Guid userID);

        Task MergeTagsAsync(Guid userID, Guid id, IEnumerable<Guid> tagIdsToMerge);

        Task<User> FindUserByEmail(string email);
    }
}
