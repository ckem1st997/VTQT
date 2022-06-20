using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System;

namespace VTQT.Services.Asset
{
    public partial class AssetTransferenceService : IAssetTransferenceService
    {
        #region Fields
        private readonly IRepository<AssetTransference> _assetTransferenceRepository;
        #endregion

        #region Ctor
        public AssetTransferenceService()
        {
            _assetTransferenceRepository = EngineContext.Current.Resolve<IRepository<AssetTransference>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(AssetTransference entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Guid.NewGuid().ToString("D");
            var result = await _assetTransferenceRepository.InsertAsync(entity);

            return result;
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
