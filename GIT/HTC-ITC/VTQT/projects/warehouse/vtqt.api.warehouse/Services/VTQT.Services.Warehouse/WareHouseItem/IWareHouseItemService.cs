using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IWareHouseItemService
    {
        Task<int> InsertAsync(WareHouseItem entity);

        Task<long> InsertAsync(IEnumerable<WareHouseItem> entities);

        Task<int> UpdateAsync(WareHouseItem entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<WareHouseItem> GetAll(bool showHidden = false);

        IPagedList<WareHouseItem> Get(WareHouseItemSearchContext ctx);

        Task<WareHouseItem> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes);

        Task<WareHouseItem> GetByCodeAsync(string code);

        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
