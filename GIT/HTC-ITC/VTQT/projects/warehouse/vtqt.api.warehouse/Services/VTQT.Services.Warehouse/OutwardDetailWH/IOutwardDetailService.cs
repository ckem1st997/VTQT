using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IOutwardDetailService
    {
        Task InsertAsync(OutwardDetail entity);

        Task UpdateAsync(OutwardDetail entity);

        Task DeletesAsync(IEnumerable<string> ids);

        Task DeletesAsync(IEnumerable<OutwardDetail> outwardDetails);

        IList<OutwardDetail> GetByOutwardId(OutwardDetailSearchContext ctx);

        IList<OutwardDetail> GetSelect();

        Task<OutwardDetail> GetByIdAsync(string id);

        IQueryable<OutwardDetail> GetListById(string id);

        IQueryable<OutwardDetail> GetListShowNameById(string id);

        Task<bool> ExistsAsync(string itemId, string outwardId,
            string departmentId, string employeeId, string stationId,
            string projectId, string customerId);
    }
}