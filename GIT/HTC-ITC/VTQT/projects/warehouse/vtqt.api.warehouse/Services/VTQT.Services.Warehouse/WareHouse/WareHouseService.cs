using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
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
    public partial class WareHouseService : IWareHouseService
    {
        #region Constants



        #endregion

        #region Fields

        private readonly IRepository<WareHouse> _wareHouseRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public WareHouseService(
            IXBaseCacheManager cacheManager)
        {
            _wareHouseRepository = EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public async Task<long> InsertWHAsync(IEnumerable<WareHouse> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _wareHouseRepository.InsertAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual async Task<int> InsertAsync(WareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _wareHouseRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual async Task<int> UpdateAsync(WareHouse entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _wareHouseRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _wareHouseRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual IList<WareHouse> GetAll(bool showHidden = false)
        {
            var key = WarehouseCacheKeys.Warehouses.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _wareHouseRepository.Table select p;
                if (!showHidden)
                    query =
                        from p in query
                        where !p.Inactive
                        select p;

                query =
                    from p in query
                    orderby p.Code
                    select p;

                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }


        public virtual IPagedList<WareHouse> Get(WareHouseSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _wareHouseRepository.Table select p;

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
                             el.l.LocaleKeyGroup == nameof(WareHouse) &&
                             el.l.LocaleKey == nameof(WareHouse.Name) &&
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

            return new PagedList<WareHouse>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<WareHouse> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _wareHouseRepository.GetByIdAsync(id);
        }

        public virtual async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _wareHouseRepository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public async void UpdatePath(WareHouse entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else
            {
                string concatStr = "/";
                if (string.IsNullOrEmpty(entity.ParentId))
                {
                    entity.Path = entity.Id;
                    await _wareHouseRepository.UpdateAsync(entity);
                }
                else
                {
                    var query = await _wareHouseRepository.Table
                        .FirstOrDefaultAsync(x => x.Path.EndsWith(entity.ParentId));

                    if (!string.IsNullOrEmpty(query?.Path))
                    {
                        entity.Path = query.Path + concatStr + entity.Id;
                        await _wareHouseRepository.UpdateAsync(entity);
                    }
                }
            }
        }

        public void UpdateAllPath()
        {
            var roots = _wareHouseRepository.Table
                .Where(x => x.ParentId == null).ToList();
            var children = _wareHouseRepository.Table
                .Where(x => x.ParentId != null).ToList();
            List<TreePath> trees = new List<TreePath>();
            if (roots?.Count > 0)
            {
                roots.ForEach(root =>
                {
                    TreePath tree = new TreePath()
                    {
                        IsRoot = true,
                        Data = root.Id,
                        Path = root.Id
                    };
                    trees.Add(tree);
                    BuildTree(null, tree, trees, children);
                });

                if (trees.Count > 0)
                {
                    trees.ForEach(async tree =>
                    {
                        var itemCategory = await _wareHouseRepository.GetByIdAsync(tree.Data);
                        if (itemCategory != null)
                        {
                            itemCategory.Path = tree.Path;
                            await _wareHouseRepository.UpdateAsync(itemCategory);
                        }
                    });
                }
            }
        }

        private void BuildTree(WareHouse child, TreePath node, List<TreePath> trees, List<WareHouse> children)
        {
            child = children.FirstOrDefault(x => x.ParentId == node.Data);
            if (node.IsRoot == true && child == null)
            {
                return;
            }
            else
            {
                if (child != null)
                {
                    TreePath nodeChild = new TreePath()
                    {
                        Data = child.Id,
                        Parents = node
                    };
                    nodeChild.Path = node.Path + "/" + nodeChild.Data;
                    node.Children.Add(nodeChild);
                    trees.Add(nodeChild);
                    children.Remove(child);
                    BuildTree(null, nodeChild, trees, children);
                }
                else
                {
                    BuildTree(null, node.Parents, trees, children);
                }
            }
        }

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _wareHouseRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(code));
           
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _wareHouseRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(newCode)
                        && !a.Code.Equals(oldCode));
            
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden = false)
        {
            var results = new List<SelectListItem>();

            var query = from p in _wareHouseRepository.Table select p;
            if (!showHidden)
                query =
                    from p in query
                    where !p.Inactive
                    select p;

            query =
                from p in query
                orderby p.Code
                select p;

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Code,
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public async Task<WareHouse> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(code);
            }

            var entity = await _wareHouseRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return entity;
        }

        public string GetLastSelectedNodeTree(string appId, string userId, string path, string warehouseId)
        {
            var cacheKey = ModelCacheKeys.WarehousesTreeModelCacheKey.FormatWith(appId, userId, path);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                return warehouseId;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }

        public async Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            return _wareHouseRepository.Table
                .Where(w => !string.IsNullOrEmpty(w.Code) && codes.Contains(w.Code))
                .Select(s => s.Code);
        }
        #endregion
    }
}
