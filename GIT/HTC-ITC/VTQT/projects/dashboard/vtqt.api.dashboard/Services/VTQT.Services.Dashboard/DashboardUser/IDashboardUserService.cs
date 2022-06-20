using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;

namespace VTQT.Services.Dashboard
{
    public partial interface IDashboardUserService
    {

        Task<long> InsertRangeAsync(IEnumerable<DashBoardUser> entities);

        Task<int> UpdateRangeAsync(IEnumerable<DashBoardUser> entities);
        Task<DashBoardUser> GetByIdAsync(string id);

        Task<int> DeletesAsync(IEnumerable<string> ids);
        Task<int> DeletesAsync(string idUser, string idTypeValue);

        Task<bool> ExistAsync(string idTypeValue, string idUser);
        Task<bool> ExistAsync(string idUser);
        IQueryable<DashBoardUser> GetListRole(string idTypeValue);

        Task<List<TypeValueModel>> GetListByUser(string idUser);
        Task<IPagedList<DashBoardUser>> Get(DashboardUserSearchContext ctx);
    }
}