using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public interface IAuditDetailService
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

        Task<long> InsertRangeAsync(IEnumerable<AuditDetail> entities);

        Task<IList<AuditDetail>> GetAuditDetailByAssetItemId(string idOrganization, string idAssetItem);

        IList<Core.Domain.Asset.Asset> GetAuditDetailByOrganizationUnitId(string idOrganizationUnit, DateTime dateTime, int assetType);

        IList<Core.Domain.Asset.Asset> GetAuditDetailByStationCode(string idStationCode, DateTime dateTime, int assetType);

        IList<Core.Domain.Asset.Asset> GetAuditDetailByProjectCode(string idProjectCode, DateTime dateTime, int assetType);
    }
}