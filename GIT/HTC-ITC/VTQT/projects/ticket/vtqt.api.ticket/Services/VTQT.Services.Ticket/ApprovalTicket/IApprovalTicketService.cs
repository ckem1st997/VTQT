using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IApprovalTicketService
    {
        Task<int> InsertAsync(ApprovalTicket entity);

        Task<long> InsertsAsync(IEnumerable<ApprovalTicket> entities);

        Task<int> UpdateAsync(ApprovalTicket entity);

        Task<int> UpdatesAsync(IEnumerable<ApprovalTicket> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ApprovalTicket> GetByIdAsync(string id);

        IList<ApprovalTicket> GetByTicketIdAsync(string ticketId);

        IPagedList<ApprovalTicket> Get(ApprovalTicketSearchContext ctx);

        IPagedList<ApprovalTicket> GetDetail(ApprovalTicketSearchContext ctx);

        IList<Core.Domain.Ticket.ApprovalTicket> GetByApprovalTicketId(ApprovalTicketSearchContext ctx);
    }
}
