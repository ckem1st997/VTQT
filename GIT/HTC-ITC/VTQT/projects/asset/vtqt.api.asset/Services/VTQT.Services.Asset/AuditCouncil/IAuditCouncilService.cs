using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAuditCouncilService
    {
        Task InsertAsync(AuditCouncil entity);

        Task UpdateAsync(AuditCouncil entity);

        Task DeletesAsync(IEnumerable<string> ids);

        Task DeletesAsync(IEnumerable<AuditCouncil> auditCouncils);

        IList<AuditCouncil> GetByAuditCouncilId(AuditCouncilSearchContext ctx);

        Task<AuditCouncil> GetByIdAsync(string id);

        IQueryable<AuditCouncil> GetListById(string id);

        IQueryable<AuditCouncil> GetListShowNameById(string id);

        Task<bool> ExistsAsync(string auditId, string employeeId);
    }
}