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
    public partial class UnitService : IUnitService
    {
        #region Fields
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouseItem> _itemRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public UnitService(IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _unitRepository = EngineContext.Current.Resolve<IRepository<Unit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _itemRepository = EngineContext.Current.Resolve<IRepository<WareHouseItem>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(Unit entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _unitRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Unit.Prefix);

            return result;
        }

        public async Task<int> UpdateAsync(Unit entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _unitRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Unit.Prefix);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _unitRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Unit.Prefix);

            return result;
        }

        public IList<Unit> GetAll(bool showHidden = false)
        {
            var key = WarehouseCacheKeys.Unit.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _unitRepository.Table select p;
                if (!showHidden)
                {
                    query = from p in query where !p.Inactive select p;
                }

                query = from p in query orderby p.UnitName select p;
                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }



        public IList<Unit> GetSelect()
        {
            var key = WarehouseCacheKeys.Unit.AllCacheKey.FormatWith(true);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _unitRepository.Table select p;
                query = from p in query orderby p.UnitName select p;
                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }




        public virtual IList<Unit> GetComboBox(string name)
        {
            var query = from p in _unitRepository.Table select p;
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.UnitName.Contains(name)).Select(x => x);
            query =
                (from p in query
                 where !p.Inactive
                 select new Unit { Id = p.Id, UnitName = p.UnitName }).Take(10);
            return query.ToList();
        }
        public IPagedList<Unit> Get(UnitSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _unitRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                            el.e.UnitName.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Unit) &&
                             el.l.LocaleKey == nameof(Unit.UnitName) &&
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
                orderby p.UnitName
                select p;

            return new PagedList<Unit>(query, ctx.PageIndex, ctx.PageSize);
        }

        public async Task<Unit> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _unitRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _unitRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Unit.Prefix);

            return result;
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _unitRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.UnitName)
                && name != null
                && x.UnitName.ToLower().Equals(name.ToLower()));
        }

        public async Task<bool> ExistsAsync(string oldName, string newName)
        {
            return await _unitRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.UnitName)
                && oldName != null && newName != null
                && x.UnitName.ToLower().Equals(newName.ToLower())
                && !x.UnitName.ToLower().Equals(oldName.ToLower()));
        }

        public string GetUnitNameByWareHouseItemCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = (from i in _itemRepository.Table.Where(x => x.Code.Equals(code))
                          join u in _unitRepository.Table on i.UnitId equals u.Id
                          select u.UnitName).FirstOrDefault();

            return result;
        } 
        
        
        public Unit GetUnitByWareHouseItemCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = (from i in _itemRepository.Table.Where(x => x.Code.Equals(code))
                          join u in _unitRepository.Table on i.UnitId equals u.Id
                          select u).FirstOrDefault();

            return result;
        }
        #endregion
    }
}
