using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IApprovalCRMxService
    {
        Task<int> InsertAsync(ApprovalCRMx entity);

        Task<long> InsertsAsync(IEnumerable<ApprovalCRMx> entities);

        Task<int> UpdateAsync(ApprovalCRMx entity);

        Task<int> UpdatesAsync(IEnumerable<ApprovalCRMx> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ApprovalCRMx> GetByIdAsync(string id);

        IList<ApprovalCRMx> GetByCRMxIdAsync(string crMxId);

        IPagedList<ApprovalCRMx> Get(ApprovalCRMxSearchContext ctx);

        IList<Core.Domain.Ticket.ApprovalCRMx> GetByApprovalCRMxId(ApprovalCRMxSearchContext ctx);
    }
}