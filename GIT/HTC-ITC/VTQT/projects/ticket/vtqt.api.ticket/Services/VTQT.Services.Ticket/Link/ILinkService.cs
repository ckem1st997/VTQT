using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ILinkService
    {
        Task<int> InsertAsync(Link entity);

        Task<int> UpdateAsync(Link entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Link> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Link> Get(LinkSearchContext ctx);

        IList<Link> GetAll(bool showHidden = false);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
    }
}