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
    public partial class CategoryService : ICategoryService
    {
        #region Constants



        #endregion

        #region Fields

        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public CategoryService(
            IXBaseCacheManager cacheManager)
        {
            _categoryRepository = EngineContext.Current.Resolve<IRepository<Category>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods
        
        public async Task<long> InsertWHAsync(IEnumerable<Category> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _categoryRepository.InsertAsync(entities);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual async Task<int> InsertAsync(Category entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _categoryRepository.InsertAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual async Task<int> UpdateAsync(Category entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _categoryRepository.UpdateAsync(entity);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _categoryRepository.DeleteAsync(ids);

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        public virtual IList<Category> GetAll(bool showHidden = false)
        {
            var key = WarehouseCacheKeys.Warehouses.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(key, () =>
            {
                var query = from p in _categoryRepository.Table select p;
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


        public virtual IPagedList<Category> Get(CategorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _categoryRepository.Table select p;

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
                             el.l.LocaleKeyGroup == nameof(Category) &&
                             el.l.LocaleKey == nameof(Category.Name) &&
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

            return new PagedList<Category>(query, ctx.PageIndex, ctx.PageSize);
        }

        public virtual async Task<Category> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                throw new ArgumentException(nameof(id));

            return await _categoryRepository.GetByIdAsync(id);
        }

        public virtual async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _categoryRepository.Table
                .Where(w => ids.Contains(w.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();

            await _cacheManager.HybridProvider.RemoveByPrefixAsync(WarehouseCacheKeys.Warehouses.Prefix);

            return result;
        }

        private void BuildTree(Category child, TreePath node, List<TreePath> trees, List<Category> children)
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
            return await _categoryRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(code));

        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _categoryRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(newCode)
                        && !a.Code.Equals(oldCode));

        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden = false)
        {
            var results = new List<SelectListItem>();

            var query = from p in _categoryRepository.Table select p;
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

        public async Task<Category> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(code);
            }

            var entity = await _categoryRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return entity;
        }

        public string GetLastSelectedNodeTree(string appId, string userId, string path, string categoryId)
        {
            var cacheKey = ModelCacheKeys.WarehousesTreeModelCacheKey.FormatWith(appId, userId, path);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                return categoryId;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }

        public async Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            return _categoryRepository.Table
                .Where(w => !string.IsNullOrEmpty(w.Code) && codes.Contains(w.Code))
                .Select(s => s.Code);
        }
        #endregion
    }
}
