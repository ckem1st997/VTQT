using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IConfirmCRMxService
    {
        Task<int> InsertAsync(ConfirmCRMx entity);

        Task<long> InsertsAsync(IEnumerable<ConfirmCRMx> entities);

        Task<int> UpdateAsync(ConfirmCRMx entity);

        Task<int> UpdatesAsync(IEnumerable<ConfirmCRMx> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ConfirmCRMx> GetByIdAsync(string id);

        IList<ConfirmCRMx> GetByCRMxIdAsync(string crMxId);

        IPagedList<ConfirmCRMx> Get(ConfirmCRMxSearchContext ctx);

        IList<Core.Domain.Ticket.ConfirmCRMx> GetByConfirmCRMxId(ConfirmCRMxSearchContext ctx);
    }
}