using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IWareHouseLimitService
    {
        Task<int> InsertAsync(WareHouseLimit entity);

        Task<long> InsertRangeAsync(IEnumerable<WareHouseLimit> entities);

        Task<int> UpdateAsync(WareHouseLimit entity);

        Task<int> UpdateRangeAsync(IEnumerable<WareHouseLimit> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<WareHouseLimit> GetAll();

        IList<WareHouseLimit> GetSelect();

        Task<IList<WareHouseLimit>> Get(WareHouseLimitSearchContext ctx);
        Task<IPagedList<WareHouseLimit>> GetToHome(WareHouseLimitSearchContext ctx);

        Task<WareHouseLimit> GetByIdAsync(string id);

        Task<bool> ExistAsync(string idWareHouse, string idItem);
    }
}