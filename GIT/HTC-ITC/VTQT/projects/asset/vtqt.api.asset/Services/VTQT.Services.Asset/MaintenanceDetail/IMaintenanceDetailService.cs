using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IMaintenanceDetailService
    {
        Task<long> InsertAsync(List<Core.Domain.Asset.MaintenanceDetail> entities);

        IPagedList<Core.Domain.Asset.Asset> Get(MaintenanceDetailSearchContext ctx);

        IPagedList<AssetAttachment> GetAssetAttachments(MaintenanceDetailSearchContext ctx);
    }
}
