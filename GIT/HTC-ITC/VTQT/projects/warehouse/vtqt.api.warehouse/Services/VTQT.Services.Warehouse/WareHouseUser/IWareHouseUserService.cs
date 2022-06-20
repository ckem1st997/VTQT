using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;

namespace VTQT.Services.Warehouse
{
    public partial interface IWareHouseUserService
    {
        //  Task<int> InsertAsync(WareHouseUser entity);

        Task<long> InsertRangeAsync(IEnumerable<Core.Domain.Warehouse.WareHouseUser> entities);


        //    Task<int> UpdateAsync(BeginningWareHouse entity);

        Task<int> UpdateRangeAsync(IEnumerable<Core.Domain.Warehouse.WareHouseUser> entities);
        Task<Core.Domain.Warehouse.WareHouseUser> GetByIdAsync(string id);

        Task<int> DeletesAsync(IEnumerable<string> ids);
        Task<int> DeletesAsync(string idUser, string idWareHouse);

        Task<bool> ExistAsync(string idWareHouse, string idUser);
        Task<bool> ExistAsync(string idUser);
        IQueryable<Core.Domain.Warehouse.WareHouseUser> GetListRole(string idWareHouse);

        Task<List<WareHouseModel>> GetListByUser(string idUser);
        Task<IPagedList<Core.Domain.Warehouse.WareHouseUser>> Get(WareHouseUserSearchContext ctx);
    }
}