using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTQT.Services.Ticket
{
    public partial interface ITicketService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.Ticket entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.Ticket entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.Ticket> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.Ticket> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);
    }
}
