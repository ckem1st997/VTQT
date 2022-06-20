using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTQT.Services.Ticket
{
    public interface IRatingService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.Rating entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.Rating entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.Rating> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.Rating> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IList<Core.Domain.Ticket.Rating> GetAll(bool showHidden = false);
    }
}