using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IChannelCategoryService
    {
        Task<int> InsertAsync(ChannelCategory entity);

        Task<int> UpdateAsync(ChannelCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ChannelCategory> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<ChannelCategory> Get(ChannelCategorySearchContext ctx);

        IList<ChannelCategory> GetAll(bool showHidden = false);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
        
        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}