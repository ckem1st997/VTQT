using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IChannelTicketService
    {
        Task<int> InsertAsync(ChannelTicket entity);

        Task<long> InsertsAsync(IEnumerable<ChannelTicket> entities);

        Task<int> UpdateAsync(ChannelTicket entity);

        Task<int> UpdatesAsync(IEnumerable<ChannelTicket> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ChannelTicket> GetByIdAsync(string id);

        IList<ChannelTicket> GetByTicketIdAsync(string ticketId);

        IPagedList<ChannelTicket> Get(ChannelTicketSearchContext ctx);

        IList<Core.Domain.Ticket.ChannelTicket> GetByChannelTicketId(ChannelTicketSearchContext ctx);
    }
}
