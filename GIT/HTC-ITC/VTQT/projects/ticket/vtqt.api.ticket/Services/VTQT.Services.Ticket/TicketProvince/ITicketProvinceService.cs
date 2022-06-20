using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface ITicketProvinceService
    {
        Task<int> InsertAsync(TicketProvince entity);

        Task<int> UpdateAsync(TicketProvince entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<TicketProvince> GetByIdAsync(string id);

        Task<TicketProvince> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<TicketProvince> Get(TicketProvinceSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden);

        IList<TicketProvince> GetAll(bool showHidden = false);
    }
}
