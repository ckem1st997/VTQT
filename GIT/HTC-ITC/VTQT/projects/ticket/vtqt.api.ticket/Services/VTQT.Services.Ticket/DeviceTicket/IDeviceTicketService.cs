using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IDeviceTicketService
    {
        Task<int> InsertAsync(DeviceTicket entity);

        Task<long> InsertsAsync(IEnumerable<DeviceTicket> entities);

        Task<int> UpdateAsync(DeviceTicket entity);

        Task<int> UpdatesAsync(IEnumerable<DeviceTicket> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<DeviceTicket> GetByIdAsync(string id);

        IList<DeviceTicket> GetByTicketIdAsync(string ticketId);

        IPagedList<DeviceTicket> Get(DeviceTicketSearchContext ctx);

        IList<Core.Domain.Ticket.DeviceTicket> GetByDeviceTicketId(DeviceTicketSearchContext ctx);
    }
}
