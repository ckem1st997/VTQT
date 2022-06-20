using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.Services.Dashboard
{
    public partial interface ITypeValueService
    {
        Task<int> InsertAsync(TypeValue entity);

        Task<int> UpdateAsync(TypeValue entity);

        Task<int> DeleteAsync(IEnumerable<string> ids);

        Task<bool> ExistAsync(string code);

        Task<TypeValue> GetByIdAsync(string id);

        IList<TypeValue> GetAll(bool show);

        IList<TypeValue> GetListDropDownd();

        IPagedList<TypeValue> Get(TypeValueSearchContext ctx);
        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        IList<SelectListItem> GetMvcListItems();

        IList<TypeValue> GetByIdsAsync(IEnumerable<string> ids);
        
        string GetLastSelectedNodeTree(string appId, string userId, string path, string typeValueId);

    }
}