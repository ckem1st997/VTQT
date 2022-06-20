using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface INetworkLinkCategoryService
    {
        Task<int> InsertAsync(NetworkLinkCategory entity);

        Task<int> UpdateAsync(NetworkLinkCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<NetworkLinkCategory> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<NetworkLinkCategory> Get(NetworkLinkCategorySearchContext ctx);

        IList<NetworkLinkCategory> GetAll(bool showHidden = false);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
        
        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}