using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IChannelService
    {
        Task<int> InsertAsync(Channel entity);

        Task<int> UpdateAsync(Channel entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Channel> GetByIdAsync(string id);

        Task<Channel> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Channel> Get(ChannelSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}