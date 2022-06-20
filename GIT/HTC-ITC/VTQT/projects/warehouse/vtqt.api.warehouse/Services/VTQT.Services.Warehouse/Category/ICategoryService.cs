using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface ICategoryService
    {
        Task<int> InsertAsync(Category entity);

        Task<long> InsertWHAsync(IEnumerable<Category> entities);

        Task<int> UpdateAsync(Category entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<Category> GetAll(bool showHidden = false);

        IPagedList<Category> Get(CategorySearchContext ctx);

        Task<Category> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);


        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        IList<SelectListItem> GetMvcListItems(bool showHidden = false);

        Task<Category> GetByCodeAsync(string code);

        string GetLastSelectedNodeTree(string appId, string userId, string path, string categoryId);

        Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes);
       
    }
}
