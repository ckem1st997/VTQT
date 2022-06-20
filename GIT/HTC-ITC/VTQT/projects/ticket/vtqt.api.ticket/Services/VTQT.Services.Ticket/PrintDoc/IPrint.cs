using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IPrint
    {
        Task<CR> GetByIdToWordCRAsync(string id);

        Task<VTQT.Core.Domain.Ticket.Ticket> GetByIdToWordTicketAsync(string id);
    }
}