using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IWareHouseItemCategoryService
    {
        Task<int> InsertAsync(WareHouseItemCategory entity);

        Task<long> InsertWHitemCategoryAsync(IEnumerable<WareHouseItemCategory> entities);

        Task<int> UpdateAsync(WareHouseItemCategory entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<WareHouseItemCategory> GetAll(bool showHidden = false);

        IPagedList<WareHouseItemCategory> Get(WareHouseItemCategorySearchContext ctx);

        Task<WareHouseItemCategory> GetByIdAsync(string id);

        void UpdatePath(WareHouseItemCategory item);

        void UpdateAllPath();

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes);
    }
}
