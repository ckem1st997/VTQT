using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class WareHouseItemService : IWareHouseItemService
    {
        #region Fields
        private readonly IRepository<WareHouseItem> _itemRepository;
        private readonly IRepository<WareHouseItemCategory> _wareHouseItemCategoryRepository;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<WareHouseItemCategory> _categoryRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public WareHouseItemService(IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _wareHouseItemCategoryRepository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _itemRepository = EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _unitRepository = EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _categoryRepository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods      
        public async Task<int> InsertAsync(WareHouseItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _itemRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItem.Prefix);

            return result;
        }

        public async Task<long> InsertAsync(IEnumerable<WareHouseItem> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _itemRepository.InsertAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItem.Prefix);

            return result;
        }

        public async Task<int> UpdateAsync(WareHouseItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _itemRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItem.Prefix);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _itemRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItem.Prefix);

            return result;
        }

        public IList<WareHouseItem> GetAll(bool showHidden = false)
        {
            var key = WarehouseCacheKeys.WareHouseItem.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _itemRepository.Table select p;
                if (!showHidden)
                {
                    query = from p in query where !p.Inactive select p;
                }
                query = from p in query orderby p.Code select p;
                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        public IPagedList<WareHouseItem> Get(WareHouseItemSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _itemRepository.Table
                        join u in _unitRepository.Table on i.UnitId equals u.Id into ui
                        join v in _vendorRepository.Table on i.VendorID equals v.Id into vi
                        join c in _categoryRepository.Table on i.CategoryID equals c.Id into ci
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        from ic in ci.DefaultIfEmpty()
                        select new WareHouseItem
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            CategoryID = i.CategoryID,
                            Description = i.Description,
                            VendorID = i.VendorID,
                            VendorName = i.VendorName,
                            Country = i.Country,
                            UnitId = i.UnitId,
                            Inactive = i.Inactive,
                            Unit = iu == null ? null : new Unit
                            {
                                Id = iu.Id,
                                UnitName = iu.UnitName
                            },
                            Vendor = iv == null ? null : new Vendor
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name
                            },
                            WareHouseItemCategory = ic == null ? null : new WareHouseItemCategory
                            {
                                Id = ic.Id,
                                Code = ic.Code,
                                Name = ic.Name
                            }
                        };

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                            el.e.Code.Contains(ctx.Keywords) ||
                            el.e.Name.Contains(ctx.Keywords) ||
                            el.e.Country.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(WareHouseItem) &&
                             el.l.LocaleKey == nameof(WareHouseItem.Name) &&
                             el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }

            if (ctx.Keywords.HasValue())
            {

                //searchItemCategory
                var queryItemCategory = from i in _wareHouseItemCategoryRepository.Table
                                        where i.Name.Contains(ctx.Keywords) ||
                                              i.Code.Contains(ctx.Keywords)
                                        select i.Id;


                if (queryItemCategory?.ToList().Count > 0)
                {
                    var itemCategoryIds = queryItemCategory.ToList();
                    query = query.Where(x => itemCategoryIds.Contains(x.CategoryID));
                }
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

            return new PagedList<WareHouseItem>(query, ctx.PageIndex, ctx.PageSize);
        }

        public async Task<WareHouseItem> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _itemRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _itemRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItem.Prefix);

            return result;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _itemRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.Code)
                && x.Code.Equals(code));
        }

        public async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _itemRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.Code)
                && x.Code.Equals(newCode)
                && !x.Code.Equals(oldCode));
        }

        public async Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            return _itemRepository.Table
                .Where(w => !string.IsNullOrEmpty(w.Code) && codes.Contains(w.Code))
                .Select(s => s.Code);
        }

        public async Task<WareHouseItem> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var entity = await _itemRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return entity;
        }
        #endregion

        #region List

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _itemRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query
                    where !p.Inactive
                    select p;
            }

            query = from p in query
                orderby p.Code
                select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        #endregion
    }
}
