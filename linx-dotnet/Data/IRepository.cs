using System.Collections.Generic;
using System.Threading.Tasks;
using Linx.Domain;

namespace Linx.Data
{
    public interface IRepository
    {
        Task<User> FindUserByEmail(string email);

        Task<IEnumerable<Link>> GetLinksAsync();
    }
}
