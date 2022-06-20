using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public partial interface IStationTicketService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.Ticket_Tram entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.Ticket_Tram entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.Ticket_Tram> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.Ticket_Tram> GetByTicketIdAsync(string ticketId);

        IPagedList<StationTicketGridModel> Get(StationTicketSearchContext ctx);

        IPagedList<StationTicketExcelModel> GetExcelData(StationTicketSearchContext ctx);
    }
}
