using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Services.Warehouse
{
    public partial interface IVoucherWareHouseService
    {
        Task<IPagedList<VoucherWareHouseModel>> Get(VoucherWareHouseSearchContext ctx);

        Task DeletesAsync(IEnumerable<string> ids);
    }
}
