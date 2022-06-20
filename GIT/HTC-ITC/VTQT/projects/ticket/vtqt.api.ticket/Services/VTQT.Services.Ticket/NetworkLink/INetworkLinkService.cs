using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface INetworkLinkService
    {
        Task<int> InsertAsync(NetworkLink entity);

        Task<int> UpdateAsync(NetworkLink entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<NetworkLink> GetByIdAsync(string id);

        Task<NetworkLink> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<NetworkLink> Get(NetworkLinkSearchContext ctx);

        IList<NetworkLink> GetAll(bool showHidden = false);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}