using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAssetDetailService
    {
        Task<long> InsertAsync(IEnumerable<AssetDetail> entities);

        Task<long> UpdateAsync(IEnumerable<AssetDetail> entities);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<AssetAttachment> GetAssetAttachmentsByAssetId(string assetId);

        IList<Core.Domain.Asset.Asset> GetAssetsByAssetAttachmentId(string assetAttachmentId);

        IList<AssetDetail> GetAssetDetailsByAssetId(string assetId);

        IList<AssetDetail> GetAssetDetailsByAssetAttachmentId(string assetAttachmentId);

        IList<AssetDetail> GetAssetDetails(string assetId, string assetAttachmentId);
    }
}