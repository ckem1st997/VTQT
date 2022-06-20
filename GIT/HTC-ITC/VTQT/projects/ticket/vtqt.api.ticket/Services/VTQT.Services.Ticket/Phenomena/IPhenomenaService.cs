using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IPhenomenaService
    {
        Task<int> InsertAsync(Phenomenon entity);

        Task<int> UpdateAsync(Phenomenon entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Phenomenon> GetByIdAsync(string id);

        Task<Phenomenon> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Phenomenon> Get(PhenomenaSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden);

        IList<Phenomenon> GetAll(bool showHidden = false);
    }
}
