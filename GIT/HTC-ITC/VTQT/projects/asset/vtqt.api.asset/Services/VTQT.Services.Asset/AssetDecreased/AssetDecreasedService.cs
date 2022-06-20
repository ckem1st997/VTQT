using System;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Asset
{
    public partial class AssetDecreasedService : IAssetDecreasedService
    {
        #region Fields
        private readonly IRepository<AssetDecreased> _assetDecreasedRepository;
        #endregion

        #region Ctor
        public AssetDecreasedService()
        {
            _assetDecreasedRepository = EngineContext.Current.Resolve<IRepository<AssetDecreased>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(AssetDecreased entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _assetDecreasedRepository.InsertAsync(entity);

            return result;
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
