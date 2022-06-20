using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IWareHouseItemUnitService
    {
        Task InsertAsync(WareHouseItemUnit entity);
        Task<long> InsertAsync(IEnumerable<WareHouseItemUnit> entity);

        Task UpdateAsync(WareHouseItemUnit entity);

        Task DeletesAsync(IEnumerable<string> ids);

        Task DeletesAsync(IEnumerable<WareHouseItemUnit> wareHouseItemUnits);

        IList<WareHouseItemUnit> GetByWareHouseItemUnitId(WareHouseItemUnitSearchContext ctx);

        Task<WareHouseItemUnit> GetByIdAsync(string id);

        IQueryable<WareHouseItemUnit> GetListById(string id);

        IQueryable<WareHouseItemUnit> GetListShowNameById(string id);

        Task<bool> ExistsAsync(string unitId, string itemId);
        bool Exists(string unitId, string itemId);

        IPagedList<WareHouseItemUnit> Get(WareHouseItemUnitPagingContext ctx);

        Task<int> GetConvertRate(string idItem, string UnitId);
        
        IList<WareHouseItemUnit> GetAll(bool showHidden = false);

    }
}