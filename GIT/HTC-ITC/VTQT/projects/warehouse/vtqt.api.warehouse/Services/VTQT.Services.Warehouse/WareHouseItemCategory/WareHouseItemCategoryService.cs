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
    public partial class WareHouseItemCategoryService : IWareHouseItemCategoryService
    {
        #region Fields
        private readonly IRepository<WareHouseItemCategory> _repository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public WareHouseItemCategoryService(IXBaseCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _repository = EngineContext.Current.Resolve<IRepository<WareHouseItemCategory>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods

        public async Task<long> InsertWHitemCategoryAsync(IEnumerable<WareHouseItemCategory> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _repository.InsertAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItemCategory.Prefix);

            return result;
        }

        public async Task<int> InsertAsync(WareHouseItemCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _repository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItemCategory.Prefix);

            return result;
        }

        public async Task<int> UpdateAsync(WareHouseItemCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _repository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItemCategory.Prefix);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _repository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItemCategory.Prefix);

            return result;
        }

        public IList<WareHouseItemCategory> GetAll(bool showHidden = false)
        {
            var key = WarehouseCacheKeys.WareHouseItemCategory.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _repository.Table select p;
                if (!showHidden)
                {
                    query = from p in query where !p.Inactive select p;
                }
                query = from p in query orderby p.Code select p;
                return query.ToList();
            }, CachingDefaults.MonthCacheTime);

            return entities;
        }

        public IPagedList<WareHouseItemCategory> Get(WareHouseItemCategorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _repository.Table select p;

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
                             el.l.LocaleKeyGroup == nameof(WareHouseItemCategory) &&
                             el.l.LocaleKey == nameof(WareHouseItemCategory.Name) &&
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

            return new PagedList<WareHouseItemCategory>(query, ctx.PageIndex, ctx.PageSize);
        }

        public async Task<WareHouseItemCategory> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _repository.GetByIdAsync(id);

            return result;
        }

        public async void UpdatePath(WareHouseItemCategory entity)
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
                    await _repository.UpdateAsync(entity);
                }
                else
                {
                    var query = await _repository.Table
                        .FirstOrDefaultAsync(x => x.Path.EndsWith(entity.ParentId));

                    if (!string.IsNullOrEmpty(query?.Path))
                    {
                        entity.Path = query.Path + concatStr + entity.Id;
                        await _repository.UpdateAsync(entity);
                    }
                }
            }                    
        }

        public void UpdateAllPath()
        {
            var roots = _repository.Table
                .Where(x => x.ParentId == null).ToList();
            var children = _repository.Table
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
                        var itemCategory = await _repository.GetByIdAsync(tree.Data);
                        if (itemCategory != null)
                        {
                            itemCategory.Path = tree.Path;
                            await _repository.UpdateAsync(itemCategory);
                        }                        
                    });
                }
            }
        }

        private void BuildTree(WareHouseItemCategory child, TreePath node, List<TreePath> trees, List<WareHouseItemCategory> children)
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

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _repository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.WareHouseItemCategory.Prefix);

            return result;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _repository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.Code)
                && x.Code.Equals(code));
        }

        public async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _repository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.Code)
                && x.Code.Equals(newCode)
                && !x.Code.Equals(oldCode));
        }

        public async Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            return _repository.Table
                .Where(w => !string.IsNullOrEmpty(w.Code) && codes.Contains(w.Code))
                .Select(s => s.Code);
        }
        #endregion
    }
}
