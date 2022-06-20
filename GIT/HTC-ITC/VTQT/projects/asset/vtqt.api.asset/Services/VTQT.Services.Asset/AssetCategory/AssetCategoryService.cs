using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using System;
using VTQT.Data;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using System.Linq;
using LinqToDB;
using VTQT.Core.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Services.Asset
{
    public partial class AssetCategoryService : IAssetCategoryService
    {
        #region Fields
        private readonly IRepository<AssetCategory> _assetCategoryRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        //private readonly XBaseCacheManager _cachedManager;
        #endregion

        #region Ctor
        public AssetCategoryService()
        {
            _assetCategoryRepository = EngineContext.Current.Resolve<IRepository<AssetCategory>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            //_cachedManager = cacheManager;
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(AssetCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _assetCategoryRepository.InsertAsync(entity);

            return result;
        }        

        public async Task<int> UpdateAsync(AssetCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _assetCategoryRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _assetCategoryRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<bool> ExistAsync(string code)
        {
            return await _assetCategoryRepository.Table
                .AnyAsync(x => x.Code != null && x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool status)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _assetCategoryRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !status)
                .UpdateAsync();

            return result;
        }

        public async Task<AssetCategory> GetByIdAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _assetCategoryRepository.GetByIdAsync(id);

            return result;
        }

        public async Task UpdatePath(AssetCategory entity)
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
                    await _assetCategoryRepository.UpdateAsync(entity);
                }
                else
                {
                    var query = await _assetCategoryRepository.Table
                        .FirstOrDefaultAsync(x => x.Path.EndsWith(entity.ParentId));

                    if (!string.IsNullOrEmpty(query?.Path))
                    {
                        entity.Path = query.Path + concatStr + entity.Id;
                        await _assetCategoryRepository.UpdateAsync(entity);
                    }
                }
            }
        }

        public void UpdateAllPath()
        {
            var roots = _assetCategoryRepository.Table
                .Where(x => x.ParentId == null).ToList();
            var children = _assetCategoryRepository.Table
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
                        var itemCategory = await _assetCategoryRepository.GetByIdAsync(tree.Data);
                        if (itemCategory != null)
                        {
                            itemCategory.Path = tree.Path;
                            await _assetCategoryRepository.UpdateAsync(itemCategory);
                        }
                    });
                }
            }
        }
        #endregion

        #region List
        public IList<AssetCategory> GetAll(bool showHidden = false, int assetType = 0)
        {
            var query = from p in _assetCategoryRepository.Table 
                        select p;

            if (assetType != 0)
            {
                query = from p in query
                        where p.AssetType == assetType
                        select p;
            }

            if (!showHidden)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }

            query = from p in query orderby p.Code select p;

            return query.ToList();
        }

        public IPagedList<AssetCategory> Get(AssetCategorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _assetCategoryRepository.Table select p;

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
                             el.l.LocaleKeyGroup == nameof(AssetCategory) &&
                             el.l.LocaleKey == nameof(AssetCategory.Name) &&
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

            return new PagedList<AssetCategory>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden = false)
        {
            var query = from p in _assetCategoryRepository.Table select p;
            var result = new List<SelectListItem>();

            if (!showHidden)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }

            if (query?.ToList()?.Count > 0)
            {
                query.ToList().ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    result.Add(m);
                });
            }

            return result;
        }
        #endregion

        #region Utilities
        private void BuildTree(AssetCategory child, TreePath node, List<TreePath> trees, List<AssetCategory> children)
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
        #endregion
    }
}
