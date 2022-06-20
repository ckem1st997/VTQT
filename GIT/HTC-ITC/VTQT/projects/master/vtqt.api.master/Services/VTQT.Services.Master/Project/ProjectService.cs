using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.FbmContract;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Master
{
    public partial class ProjectService : IProjectService
    {
        #region Fields
        private readonly IIntRepository<Project> _projectRepository;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public ProjectService(
            IXBaseCacheManager cacheManager)
        {
            _projectRepository = EngineContext.Current.Resolve<IIntRepository<Project>>(DataConnectionHelper.ConnectionStringNames.FbmContract);
            _cacheManager = cacheManager;
        }
        #endregion

        #region Methods

        public virtual IList<Project> GetAll(bool showHidden = false)
        {
            var key = FbmContractCacheKeys.Projects.AllCacheKey.FormatWith(showHidden);
            var entities = _cacheManager.HybridProvider.Get(key, () =>
            {
                var query = from x in _projectRepository.Table select x;
                if (!showHidden)
                    query =
                        from x in query
                        where x.IsActive
                        select x;

                query =
                    from x in query
                    orderby x.DisplayOrder, x.ProjectCode
                    select x;

                return query.ToList();
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return entities;
        }

        public IPagedList<Project> Get(ProjectSearchContext ctx)
        {
            var query = from p in _projectRepository.Table select p;

            ctx.Keywords = ctx.Keywords?.Trim();

            if (ctx.Keywords != null  && ctx.Keywords.HasValue())
            {
                query = from p in query
                        where (p.ProjectCode != null && p.ProjectCode.Contains(ctx.Keywords)) || 
                              (p.ProjectName != null && p.ProjectName.Contains(ctx.Keywords))
                        select p;
            }

            query = from p in query orderby p.ProjectCode select p;

            return new PagedList<Project>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Project> GetAvailable()
        {
            var query = from p in _projectRepository.Table select p;

            query = from p in query
                    orderby p.ProjectCode
                    select p;

            return query.ToList();
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            var result = await _projectRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Project> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var entity = await _projectRepository.Table.FirstOrDefaultAsync(x => x.ProjectCode == code);

            return entity;
        }
        #endregion
    }
}
