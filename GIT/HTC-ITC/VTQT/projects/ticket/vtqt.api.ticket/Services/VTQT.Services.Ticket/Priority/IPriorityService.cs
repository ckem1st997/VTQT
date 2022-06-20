using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IPriorityService
    {
        Task<int> InsertAsync(Priority entity);

        Task<int> UpdateAsync(Priority entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Priority> GetByIdAsync(string id);

        Priority GetByIdName(string id);

        Task<Priority> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Priority> Get(PrioritySearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId);

        IList<Priority> GetAll(bool showHidden, string projectId);
    }
}