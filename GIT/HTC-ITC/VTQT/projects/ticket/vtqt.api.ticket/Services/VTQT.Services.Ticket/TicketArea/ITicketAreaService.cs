using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ITicketAreaService
    {
        Task<int> InsertAsync(TicketArea entity);

        Task<int> UpdateAsync(TicketArea entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<TicketArea> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<TicketArea> Get(TicketAreaSearchContext ctx);

        IList<TicketArea> GetAll(bool showHidden=false);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
