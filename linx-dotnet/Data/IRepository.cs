using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Linx.Domain;

namespace Linx.Data
{
    public interface IRepository
    {
        Task<Link> CreateLinkAsync(Link link);

        Task<Link> ReadLinkAsync(Guid id);

        Task<IEnumerable<Link>> ReadAllLinksAsync();

        Task<Link> UpdateLinkAsync(Link link);

        Task DeleteLinkAsync(Guid id);

        Task<IEnumerable<Tag>> ReadAllTagsAsync();

        Task MergeTagsAsync(Guid id, IEnumerable<Guid> tagIdsToMerge);

        Task<User> FindUserByEmail(string email);
    }
}
