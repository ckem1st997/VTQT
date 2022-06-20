using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IBeginningWareHouseService
    {
        Task<int> InsertAsync(BeginningWareHouse entity);

        Task<long> InsertRangeAsync(IEnumerable<BeginningWareHouse> entities);


        Task<int> UpdateAsync(BeginningWareHouse entity);

        Task<int> UpdateRangeAsync(IEnumerable<BeginningWareHouse> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<bool> ExistAsync(string idWareHouse, string idItem);

        IList<BeginningWareHouse> GetAll();

        Task<IPagedList<BeginningWareHouse>> Get(BeginningWareHouseSearchContext ctx);

        Task<BeginningWareHouse> GetByIdAsync(string id);

        Task<IEnumerable<WareHouseItem>> GetByWareHouseIdAsync(string id);

    }
}
