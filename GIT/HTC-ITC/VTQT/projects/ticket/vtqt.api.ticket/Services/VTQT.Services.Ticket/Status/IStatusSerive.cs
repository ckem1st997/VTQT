using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface IStatusSerive
    {
        Task<int> InsertAsync(Status entity);

        Task<int> UpdateAsync(Status entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<Status> GetByIdAsync(string id);

        Task<Status> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<Status> Get(StatusSearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId);

        IList<Status> GetAll(bool showHidden, string projectId);
    }
}