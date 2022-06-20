using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public interface IWideFtthService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.WideFtth entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.WideFtth entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.WideFtth> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.WideFtth> GetByWideFtthIdAsync(string ftthId);

        Task<IPagedList<WideFtthGridModel>> Get(WideFtthSearchContext ctx);

        IList<Core.Domain.Ticket.WideFtth> GetByWideFtthId(WideFtthSearchContext ctx);
    }
}