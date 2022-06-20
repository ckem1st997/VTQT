using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface ICommentService
    {
        Task<int> InsertAsync(Comment entity);

        Task<long> InsertsAsync(IEnumerable<Comment> entities);

        Task<int> UpdateAsync(Comment entity);

        Task<int> UpdatesAsync(IEnumerable<Comment> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Comment> GetByIdAsync(string id);

        IList<Comment> GetByTicketIdAsync(string ticketId);

        IList<Comment> GetByCRIdAsync(string crId);

        IList<Comment> GetByCRMxIdAsync(string crMxId);

        IList<Comment> GetByCRPartnerIdAsync(string crPartnerId);

        IList<Comment> GetByFTTHIdAsync(string ftthId);

        IList<Comment> GetByWideFtthIdAsync(string wideFtthId);
    }
}
