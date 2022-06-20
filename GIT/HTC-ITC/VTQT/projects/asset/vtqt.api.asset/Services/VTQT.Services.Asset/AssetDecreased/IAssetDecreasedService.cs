using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IAssetDecreasedService
    {
        Task<int> InsertAsync(AssetDecreased entity);
    }
}
