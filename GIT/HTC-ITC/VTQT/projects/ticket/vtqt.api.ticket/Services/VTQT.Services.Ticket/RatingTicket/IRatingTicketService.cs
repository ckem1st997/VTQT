using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IRatingTicketService
    {
        Task<int> InsertAsync(RatingTicket entity);

        Task<int> UpdateAsync(RatingTicket entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<RatingTicket> GetByIdAsync(string id);

        Task<IPagedList<RatingTicket>> Get(RatingTicketSearchContext ctx);
    }
}
