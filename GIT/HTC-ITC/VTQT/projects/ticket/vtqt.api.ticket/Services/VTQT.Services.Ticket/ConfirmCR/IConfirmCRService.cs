using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IConfirmCRService
    {
        Task<int> InsertAsync(ConfirmCR entity);

        Task<long> InsertsAsync(IEnumerable<ConfirmCR> entities);

        Task<int> UpdateAsync(ConfirmCR entity);

        Task<int> UpdatesAsync(IEnumerable<ConfirmCR> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<ConfirmCR> GetByIdAsync(string id);

        IList<ConfirmCR> GetByCRIdAsync(string crId);

        IPagedList<ConfirmCR> Get(ConfirmCRSearchContext ctx);

        IList<Core.Domain.Ticket.ConfirmCR> GetByConfirmCRId(ConfirmCRSearchContext ctx);
    }
}