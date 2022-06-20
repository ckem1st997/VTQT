using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public partial interface ITroubleTicketService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.Ticket_VanDe entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.Ticket_VanDe entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.Ticket_VanDe> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.Ticket_VanDe> GetByTicketIdAsync(string ticketId);

        IPagedList<TroubleTicketGridModel> Get(TroubleTicketSearchContext ctx);

        IPagedList<TroubleTicketExcelModel> GetExcelData(TroubleTicketSearchContext ctx);
    }
}
