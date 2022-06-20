using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ICableService
    {
        Task<int> InsertAsync(Cable entity);

        Task<int> UpdateAsync(Cable entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Cable> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Cable> Get(CableSearchContext ctx);

        IList<Cable> GetAll(bool showHidden = false);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}