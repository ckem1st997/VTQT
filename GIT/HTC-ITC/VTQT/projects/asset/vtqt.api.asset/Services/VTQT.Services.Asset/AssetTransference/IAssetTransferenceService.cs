using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAssetTransferenceService
    {
        Task<int> InsertAsync(AssetTransference entity);
    }
}
