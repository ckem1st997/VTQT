using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;

namespace VTQT.Services.Dashboard
{
    public partial interface IStorageValueService
    {
        Task<int> InsertAsync(StorageValue entity);

        Task<int> UpdateAsync(StorageValue entity);

        Task<int> DeleteAsync(IEnumerable<string> ids);

        Task<bool> ExistAsync(string code);

        Task<StorageValue> GetByIdAsync(string id);
        Task<string> RunQueryAsync(string query);
        Task<object> RunQueryToObjectAsync(string query);
        Task<int> RunQueryCounttAsync(string query);

        IList<StorageValue> GetAll(int assetType);

        IList<StorageValue> GetListDropDownd();

        Task<IPagedList<StorageValue>> GetAsync(StorageValueSearchContext ctx);
        
        IList<SelectListItem> GetMvcListItems(string idTypeValue);

        IList<StorageValue> GetByIdsAsync(IEnumerable<string> ids);
    }
}