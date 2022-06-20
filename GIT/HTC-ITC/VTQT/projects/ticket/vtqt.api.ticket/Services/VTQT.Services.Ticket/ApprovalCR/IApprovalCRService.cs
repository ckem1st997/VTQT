using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IApprovalCRService
    {
        Task<int> InsertAsync(ApprovalCR entity);

        Task<long> InsertsAsync(IEnumerable<ApprovalCR> entities);

        Task<int> UpdateAsync(ApprovalCR entity);

        Task<int> UpdatesAsync(IEnumerable<ApprovalCR> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ApprovalCR> GetByIdAsync(string id);

        IList<ApprovalCR> GetByCRIdAsync(string crId);

        IPagedList<ApprovalCR> Get(ApprovalCRSearchContext ctx);

        IList<Core.Domain.Ticket.ApprovalCR> GetByApprovalCRId(ApprovalCRSearchContext ctx);
    }
}