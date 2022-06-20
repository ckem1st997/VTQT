using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Asset
{
    public partial class AssetDetailService : IAssetDetailService
    {
        #region Fields
        private readonly IRepository<AssetDetail> _assetDetailRepository;
        private readonly IRepository<AssetAttachment> _assetAttachmentRepository;
        private readonly IRepository<Core.Domain.Asset.Asset> _assetRepository;
        #endregion

        #region Ctor

        public AssetDetailService()
        {
            _assetDetailRepository =
                EngineContext.Current.Resolve<IRepository<AssetDetail>>(
                    DataConnectionHelper.ConnectionStringNames.Asset);
            _assetAttachmentRepository =
                EngineContext.Current.Resolve<IRepository<AssetAttachment>>(DataConnectionHelper.ConnectionStringNames
                    .Asset);
            _assetRepository =
                EngineContext.Current.Resolve<IRepository<Core.Domain.Asset.Asset>>(DataConnectionHelper.ConnectionStringNames.Asset);
        }
        #endregion

        #region Methods
        public async Task<long> InsertAsync(IEnumerable<AssetDetail> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _assetDetailRepository.InsertAsync(entities);

            return result;
        }

        public async Task<long> UpdateAsync(IEnumerable<AssetDetail> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _assetDetailRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _assetDetailRepository.DeleteAsync(ids);

            return result;
        }
        #endregion

        #region List
        public IList<AssetAttachment> GetAssetAttachmentsByAssetId(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentNullException(nameof(assetId));
            }

            var results = new List<AssetAttachment>();

            var assetAttachmentIds = _assetDetailRepository.Table
                .Where(x => x.AssetId == assetId)
                .Select(e => e.AssetAttachmentId);


            if (!(assetAttachmentIds?.ToList()?.Count > 0)) return results;
            var ids = assetAttachmentIds.ToList();
            var query = from p in _assetAttachmentRepository.Table
                where assetAttachmentIds.Contains(p.Id)
                select p;

            if (query?.ToList().Count > 0)
            {
                results.AddRange(query.ToList());
            }

            return results;
        }

        public IList<Core.Domain.Asset.Asset> GetAssetsByAssetAttachmentId(string assetAttachmentId)
        {
            if (string.IsNullOrEmpty(assetAttachmentId))
            {
                throw new ArgumentNullException(nameof(assetAttachmentId));
            }

            var results = new List<Core.Domain.Asset.Asset>();

            var assetIds = _assetDetailRepository.Table
                .Where(x => x.AssetAttachmentId == assetAttachmentId)
                .Select(e => e.AssetId);


            if (!(assetIds?.ToList()?.Count > 0)) return results;
            var ids = assetIds.ToList();
            var query = from p in _assetRepository.Table
                        where assetIds.Contains(p.Id)
                        select p;

            if (query?.ToList().Count > 0)
            {
                results.AddRange(query.ToList());
            }

            return results;
        }

        public IList<AssetDetail> GetAssetDetails(string assetId, string assetAttachmentId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentNullException(nameof(assetId));
            }
            else if (string.IsNullOrEmpty(assetAttachmentId))
            {
                throw new ArgumentNullException(nameof(assetAttachmentId));
            }

            var results = _assetDetailRepository.Table
                .Where(x => x.AssetId == assetId && x.AssetAttachmentId == assetAttachmentId);

            return results?.ToList();
        }

        public IList<AssetDetail> GetAssetDetailsByAssetId(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentNullException(nameof(assetId));
            }

            var results = _assetDetailRepository.Table
                .Where(x => x.AssetId == assetId);

            return results?.ToList();
        }

        public IList<AssetDetail> GetAssetDetailsByAssetAttachmentId(string assetAttachmentId)
        {
            if (string.IsNullOrEmpty(assetAttachmentId))
            {
                throw new ArgumentNullException(nameof(assetAttachmentId));
            }

            var results = _assetDetailRepository.Table
                .Where(x => x.AssetAttachmentId == assetAttachmentId);

            return results?.ToList();
        }
        #endregion
    }
}