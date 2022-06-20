using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IWarehouseBalanceService
    {

        Task<int> InsertAsync(WarehouseBalance entity);

        Task<int> UpdateAsync(WarehouseBalance entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IPagedList<WarehouseBalance> Get(WarehouseBalanceSearchContext ctx);

        Task<IPagedList<WarehouseBalance>> GetTableToHome(WarehouseBalanceSearchContext ctx);

        Task<WarehouseBalance> GetByIdAsync(string id);



    }
}
