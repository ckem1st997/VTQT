using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public interface IAdminRoleWareHouseService
    {
        Task<int> InsertAsync(AdminRoleWareHouse entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<AdminRoleWareHouse> GetAll(bool showHidden = false);

        IPagedList<AdminRoleWareHouse> Get(AdminRoleWareHouseContext ctx);

        Task<AdminRoleWareHouse> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);
    }
}
