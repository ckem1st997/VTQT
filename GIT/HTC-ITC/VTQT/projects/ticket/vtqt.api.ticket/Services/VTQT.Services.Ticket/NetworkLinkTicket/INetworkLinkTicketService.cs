using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface INetworkLinkTicketService
    {
        Task<int> InsertAsync(NetworkLinkTicket entity);

        Task<long> InsertsAsync(IEnumerable<NetworkLinkTicket> entities);

        Task<int> UpdateAsync(NetworkLinkTicket entity);

        Task<int> UpdatesAsync(IEnumerable<NetworkLinkTicket> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<NetworkLinkTicket> GetByIdAsync(string id);

        IList<NetworkLinkTicket> GetByTicketIdAsync(string ticketId);

        IPagedList<NetworkLinkTicket> Get(NetworkLinkTicketSearchContext ctx);

        IList<Core.Domain.Ticket.NetworkLinkTicket> GetByNetworkLinkTicketId(NetworkLinkTicketSearchContext ctx);
    }
}
