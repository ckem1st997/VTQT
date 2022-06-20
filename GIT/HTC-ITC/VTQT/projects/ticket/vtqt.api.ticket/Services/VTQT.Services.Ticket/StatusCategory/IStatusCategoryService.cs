using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;

namespace VTQT.Services.Ticket
{
    public partial interface IStatusCategoryService
    {
        Task<int> InsertAsync(StatusCategory entity);

        Task<int> UpdateAsync(StatusCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<StatusCategory> GetByIdAsync(string id);

        Task<StatusCategory> GetByCodeAsync(string code);

        Task<bool> ExistedAsync(string code);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IPagedList<StatusCategory> Get(StatusCategorySearchContext ctx);

        IList<SelectListItem> GetMvcListAsync(bool showHidden);

        IList<StatusCategory> GetAll(bool showHidden = false);
    }
}
