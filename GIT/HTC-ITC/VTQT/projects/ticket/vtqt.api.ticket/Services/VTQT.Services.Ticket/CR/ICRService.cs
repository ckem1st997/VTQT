using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VTQT.Services.Ticket
{
    public interface ICRService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.CR entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.CR entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.CR> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.CR> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        Task<Core.Domain.Ticket.CR> GetById(IEnumerable<string> ids);
    }
}
