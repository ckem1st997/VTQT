using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public partial interface IProblemTicketService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.Ticket_SuCo entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.Ticket_SuCo entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.Ticket_SuCo> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.Ticket_SuCo> GetByTicketIdAsync(string ticketId);

        Task<IPagedList<ProblemTicketGridModel>> Get(ProblemTicketSearchContext ctx);

        IList<Core.Domain.Ticket.Ticket_SuCo> GetByTicketSucoId(ProblemTicketSearchContext ctx);

        IPagedList<ProblemTicketExcelModel> GetExcelData(ProblemTicketSearchContext ctx);
    }
}
