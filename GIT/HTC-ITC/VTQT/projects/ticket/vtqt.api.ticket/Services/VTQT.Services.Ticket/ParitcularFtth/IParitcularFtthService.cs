using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public interface IParitcularFtthService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.ParitcularFtth entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.ParitcularFtth entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.ParitcularFtth> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.ParitcularFtth> GetByFtthIdAsync(string ftthId);

        Task<IPagedList<ParitcularFtthGridModel>> Get(ParitcularFtthSearchContext ctx);

        IList<Core.Domain.Ticket.ParitcularFtth> GetByParitcularFtthId(ParitcularFtthSearchContext ctx);
    }
}