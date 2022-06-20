using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public interface ICRCategoryService
    {
        Task<int> InsertAsync(CRCategory entity);

        Task<int> UpdateAsync(CRCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<CRCategory> GetByIdAsync(string id);

        Task<CRCategory> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<CRCategory> Get(CRCategorySearchContext ctx);

        IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId);

        IList<CRCategory> GetAll(bool showHidden, string projectId);
    }
}