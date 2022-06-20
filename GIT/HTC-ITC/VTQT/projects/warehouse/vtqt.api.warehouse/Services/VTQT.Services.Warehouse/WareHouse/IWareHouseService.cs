using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IWareHouseService
    {
        Task<int> InsertAsync(WareHouse entity);

        Task<long> InsertWHAsync(IEnumerable<WareHouse> entities);

        Task<int> UpdateAsync(WareHouse entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<WareHouse> GetAll(bool showHidden = false);

        IPagedList<WareHouse> Get(WareHouseSearchContext ctx);

        Task<WareHouse> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        void UpdatePath(WareHouse item);

        void UpdateAllPath();

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        IList<SelectListItem> GetMvcListItems(bool showHidden = false);

        Task<WareHouse> GetByCodeAsync(string code);

        string GetLastSelectedNodeTree(string appId, string userId, string path, string warehouseId);

        Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes);
    }    
}
