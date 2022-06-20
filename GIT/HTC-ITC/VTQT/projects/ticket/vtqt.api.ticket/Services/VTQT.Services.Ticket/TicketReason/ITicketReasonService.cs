using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface ITicketReasonService
    {
        Task<int> InsertAsync(TicketReason entity);

        Task<int> UpdateAsync(TicketReason entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<TicketReason> GetByIdAsync(string id);

        Task<TicketReason> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<TicketReason> Get(TicketReasonSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden);

        IList<SelectListItem> GetDetailReasonsByReasonId(string reasonId);

        IList<TicketReason> GetAll(bool showHidden = false);

        IList<SelectListItem> GetMvcList(bool showHidden, string projectId);

        IList<TicketReason> GetAvailable(bool showHidden, string projectId);
    }
}
