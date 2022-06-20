using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public interface ICRPartnerService
    {
        Task<int> InsertAsync(Core.Domain.Ticket.CRPartner entity);

        Task<int> UpdateAsync(Core.Domain.Ticket.CRPartner entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Core.Domain.Ticket.CRPartner> GetByIdAsync(string id);

        Task<Core.Domain.Ticket.CRPartner> GetByCRPartnerIdAsync(string crPartnerId);

        Task<IPagedList<CRPartnerGridModel>> Get(CRPartnerSearchContext ctx);

        IList<Core.Domain.Ticket.CRPartner> GetByCRPartnerId(CRPartnerSearchContext ctx);
    }
}