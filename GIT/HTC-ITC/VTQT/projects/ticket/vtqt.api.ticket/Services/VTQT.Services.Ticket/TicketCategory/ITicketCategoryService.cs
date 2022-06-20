using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface ITicketCategoryService
    {
        Task<int> InsertAsync(TicketCategory entity);

        Task<int> UpdateAsync(TicketCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<TicketCategory> GetByIdAsync(string id);

        Task<TicketCategory> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<TicketCategory> Get(TicketCategorySearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
