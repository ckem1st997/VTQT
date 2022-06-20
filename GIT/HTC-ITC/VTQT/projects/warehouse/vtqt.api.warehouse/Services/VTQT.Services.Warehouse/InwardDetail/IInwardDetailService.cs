using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IInwardDetailService
    {
        Task InsertAsync(InwardDetail entity);

        Task UpdateAsync(InwardDetail entity);

        Task DeletesAsync(IEnumerable<string> ids);

        Task DeletesAsync(IEnumerable<InwardDetail> inwardDetails);

        IList<InwardDetail> GetByInwardId(InwardDetailSearchContext ctx);

        Task<InwardDetail> GetByIdAsync(string id);

        IQueryable<InwardDetail> GetListById(string id);

        IQueryable<InwardDetail> GetListShowNameById(string id);

        IList<InwardDetail> GetSelect();

        Task<bool> ExistsAsync(string itemId, string inwardId,
            string departmentId, string employeeId, string stationId,
            string projectId, string customerId);
    }
}
