using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public interface ICRHTCService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.CR_HTC entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.CR_HTC entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.CR_HTC> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.CR_HTC> GetByCRIdAsync(string crId);

        Task<IPagedList<CRHTCGridModel>> Get(CRHTCSearchContext ctx);

        IList<Core.Domain.Ticket.CR_HTC> GetByCRHTCId(CRHTCSearchContext ctx);
    }
}