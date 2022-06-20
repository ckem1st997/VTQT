using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class VendorService : IVendorService
    {
        #region Fields
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public VendorService(IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods

        public async Task<long> InsertVendorAsync(IEnumerable<Vendor> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _vendorRepository.InsertAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Vendor.Prefix);

            return result;
        }

        public async Task<int> InsertAsync(Vendor entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _vendorRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Vendor.Prefix);

            return result;
        }

        public async Task<int> UpdateAsync(Vendor entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _vendorRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Vendor.Prefix);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _vendorRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Vendor.Prefix);

            return result;
        }

        public IPagedList<Vendor> Get(VendorSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _vendorRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                            el.e.Code.Contains(ctx.Keywords) ||
                            el.e.Name.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Vendor) &&
                             el.l.LocaleKey == nameof(Vendor.Name) &&
                             el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query =
                    from p in query
                    where !p.Inactive
                    select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query =
                    from p in query
                    where p.Inactive
                    select p;
            }

            query =
                from p in query
                orderby p.Code
                select p;

            return new PagedList<Vendor>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Vendor> GetAll(bool showHidden = false)
        {
            var key = WarehouseCacheKeys.Vendor.AllCacheKey.FormatWith(showHidden);
            var result = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _vendorRepository.Table select p;
                if (!showHidden)
                {
                    query = from p in query where !p.Inactive select p;
                }
                query = from p in query orderby p.Code select p;
                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return result;
        }

        public async Task<Vendor> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _vendorRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _vendorRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Vendor.Prefix);

            return result;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _vendorRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.Code)
                && x.Code.Equals(code));
        }

        public async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _vendorRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.Code)
                && x.Code.Equals(newCode)
                && !x.Code.Equals(oldCode));
        }
        public async Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            return _vendorRepository.Table
                .Where(w => !string.IsNullOrEmpty(w.Code) && codes.Contains(w.Code))
                .Select(s => s.Code);
        }
        #endregion
    }
}
