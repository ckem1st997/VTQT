using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class ProjectService : IProjectService
    {
        #region Fields

        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion Fields

        #region Ctor

        public ProjectService()
        {
            _projectRepository = EngineContext.Current.Resolve<IRepository<Project>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(Project entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _projectRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Project entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _projectRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _projectRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Project> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _projectRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Project> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _projectRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _projectRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _projectRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion Methods

        #region List

        public Core.IPagedList<Project> Get(ProjectSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _projectRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(x => x.Name.Contains(ctx.Keywords) || x.Code.Contains(ctx.Keywords));
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query = from p in query
                        where p.Inactive == false
                        select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query = from p in query
                        where p.Inactive == true
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<Project>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Project> GetAll(bool showHidden = false)
        {

                var query = from p in _projectRepository.Table select p;
                if (!showHidden)
                {
                    query = from p in query where !p.Inactive select p;
                }
                query = from p in query orderby p.Code select p;
                return query.ToList();
        }
        #endregion
    }
}