using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTQT.Services.Ticket
{
    public interface IFtthService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.Ftth entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.Ftth entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.Ftth> GetByIdAsync(string id);
        Core.Domain.Ticket.Ftth GetById(string id);

        Task<Core.Domain.Ticket.Ftth> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);
    }
}