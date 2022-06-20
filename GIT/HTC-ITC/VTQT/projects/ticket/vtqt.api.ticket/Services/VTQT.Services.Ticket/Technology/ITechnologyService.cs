using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ITechnologyService
    {
        Task<Technology> GetByIdAsync(string id);

        Task<int> UpdateAsync(Technology entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<int> InsertAsync(Technology entity);

        Task<Core.Domain.Ticket.Technology> GetByIdFtthAsync(string id);
    }
}