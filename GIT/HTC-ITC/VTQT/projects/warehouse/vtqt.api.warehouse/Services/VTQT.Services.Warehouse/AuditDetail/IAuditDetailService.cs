using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IAuditDetailService
    {
        Task InsertAsync(AuditDetail entity);

        Task UpdateAsync(AuditDetail entity);

        Task DeletesAsync(IEnumerable<string> ids);

        Task DeletesAsync(IEnumerable<AuditDetail> auditDetails);

        IList<AuditDetail> GetByAuditId(AuditDetailSearchContext ctx);

        Task<AuditDetail> GetByIdAsync(string id);

        IQueryable<AuditDetail> GetListById(string id);

        IQueryable<AuditDetail> GetListShowNameById(string id);

        Task<bool> ExistsAsync(string auditId, string itemId);

        Task<IList<AuditDetail>> GetAuditDetail(AuditDetailSearchContext ctx);

        Task<IList<AuditDetail>> GetAuditDetailByWareHouseId(string dateTime,string idWh, string idItem);

        Task<long> InsertRangeAsync(IEnumerable<AuditDetail> entities);
    }
}