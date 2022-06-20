using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public interface ICRMxService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.CRMx entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.CRMx entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.CRMx> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.CRMx> GetByCRMxIdAsync(string crId);

        Task<IPagedList<CRMxGridModel>> Get(CRMxSearchContext ctx);

        IList<Core.Domain.Ticket.CRMx> GetByCRMxId(CRMxSearchContext ctx);
    }
}