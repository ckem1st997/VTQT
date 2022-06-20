using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.FbmOrganization;

namespace VTQT.Services.Asset
{
    public partial interface IAuditService
    {
        Task InsertAsync(Audit entity, IList<AuditDetail> details = null, IList<AuditCouncil> auditCouncils = null);

        Task UpdateAsync(Audit entity, IList<AuditDetail> details = null, IList<string> deleteDetailIds = null, IList<AuditCouncil> auditCouncils = null, IList<string> deleteAuditCouncilIds = null);

        void DeletesAsync(IEnumerable<string> ids);

        Task<Audit> GetByIdAsync(string id);

        IPagedList<Audit> Get(AuditSearchContext ctx);

        IPagedList<Audit> GetListShowName(AuditSearchContext ctx);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        IList<OrganizationUnit> GetAll();
    }
}