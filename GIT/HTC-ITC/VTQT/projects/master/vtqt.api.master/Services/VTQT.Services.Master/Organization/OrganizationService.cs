using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.FbmOrganization;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.Services.Master
{
    public partial class OrganizationService : IOrganizationService
    {
        #region Fields
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public OrganizationService(
            IXBaseCacheManager cacheManager)
        {
            _organizationRepository = EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.FbmOrganization);
            _cacheManager = cacheManager;
        }
        #endregion

        #region Methods
        public virtual IList<OrganizationUnit> GetAll(bool showHidden = false)
        {
            var key = MasterCacheKeys.OrganizationUnits.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.HybridProvider.Get(key, () =>
            {
                var query = from x in _organizationRepository.Table
                            where !x.IsDeleted
                            select x;
                if (!showHidden)
                    query =
                        from x in query
                        where x.IsActive
                        select x;

                query =
                    from x in query
                    orderby x.TreePath
                    select x;

                return query.ToList();
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return entities;
        }

        public IPagedList<OrganizationUnit> Get(OrganizationSearchContext ctx)
        {
            var query = from p in _organizationRepository.Table
                        where !p.IsDeleted
                        select p;

            ctx.Keywords = ctx.Keywords?.Trim();

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = from p in query
                        where (p.Code != null && p.Code.Contains(ctx.Keywords)) ||
                              (p.Name != null && p.Name.Contains(ctx.Keywords))
                        select p;
            }

            query = from p in query orderby p.Code select p;

            return new PagedList<OrganizationUnit>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<OrganizationUnit> GetAvailable()
        {
            var query = from p in _organizationRepository.Table
                        where !p.IsDeleted
                        select p;

            query = from p in query
                    orderby p.Code
                    select p;

            return query.ToList();
        }

        public async Task<OrganizationUnit> GetByIdAsync(int id)
        {
            var result = await _organizationRepository.GetByIdAsync(id);

            return result;
        }

        public IList<OrganizationTreeModel> GetOrganizationTree(int? expandLevel, bool showHidden = false)
        {
            expandLevel = expandLevel ?? 1;
            var cacheKey = ModelCacheKeys.DepartmentsTreeAllModelCacheKey.FormatWith(showHidden);

            var models = _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                var qq = new Queue<OrganizationTreeModel>();
                var lstCheck = new List<OrganizationTreeModel>();
                var result = new List<OrganizationTreeModel>();

                var organizationalUnitModels = GetOrganizationalUnits(showHidden)
                    .Select(s => new OrganizationTreeModel
                    {
                        children = new List<OrganizationTreeModel>(),
                        folder = false,
                        key = s.Id.ToString(),
                        title = s.Name,
                        tooltip = s.Name,
                        Path = s.Path,
                        ParentId = s.ParentId,
                        Code = s.Code,
                        Name = s.Name
                    });
                var roots = organizationalUnitModels
                    .Where(w => !w.ParentId.HasValue)
                    .OrderBy(o => o.Name);

                foreach (var root in roots)
                {
                    root.level = 1;
                    root.expanded = !expandLevel.HasValue || root.level <= expandLevel.Value;
                    root.folder = true;
                    qq.Enqueue(root);
                    lstCheck.Add(root);
                    result.Add(root);
                }

                while (qq.Any())
                {
                    var cur = qq.Dequeue();
                    if (lstCheck.All(a => a.key != cur.key))
                        result.Add(cur);

                    var childs = organizationalUnitModels
                        .Where(w => w.ParentId.HasValue && w.ParentId.ToString() == cur.key)
                        .OrderBy(o => o.Name);

                    if (!childs.Any())
                        continue;

                    var childLevel = cur.level + 1;
                    foreach (var child in childs)
                    {
                        if (lstCheck.Any(a => a.key == child.key))
                            continue;

                        child.level = childLevel;
                        child.expanded = !expandLevel.HasValue || child.level <= expandLevel.Value;

                        qq.Enqueue(child);
                        lstCheck.Add(child);
                        cur.children.Add(child);
                    }
                }

                return result;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return models;
        }

        public string GetLastSelectedNodeTree(string appId, string userId, string path, string departmentId)
        {
            var cacheKey = ModelCacheKeys.DepartmentsTreeModelCacheKey.FormatWith(appId, userId, path);
            return _cacheManager.HybridProvider.Get(cacheKey, () =>
            {
                return departmentId;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;
        }
        #endregion

        #region Utilities
        private IList<OrganizationModel> GetOrganizationalUnits(bool showHidden)
        {
            var cacheKey = MasterCacheKeys.OrganizationUnits.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.GetDb(cacheKey, () =>
            {
                var query = from p in _organizationRepository.Table
                            where !p.IsDeleted
                            select p;

                if (!showHidden)
                {
                    query = from p in query
                            where p.IsActive
                            select p;
                }

                query = from p in query
                        orderby p.TreePath
                        select p;

                return query.ToList();
            });           

            var models = new List<OrganizationModel>();

            if (entities?.Count > 0)
            {
                entities.ForEach(x =>
                {
                    var m = new OrganizationModel
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        Path = x.TreePath
                    };
                    models.Add(m);
                });
            }
            return models;
        }
        #endregion
    }
}
