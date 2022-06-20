using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class PriorityService : IPriorityService
    {
        #region Fields

        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Priority> _priorityRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion

        #region Ctor
        public PriorityService()
        {
            _projectRepository = EngineContext.Current.Resolve<IRepository<Project>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _priorityRepository = EngineContext.Current.Resolve<IRepository<Priority>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(Priority entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _priorityRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Priority entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _priorityRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _priorityRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Priority> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _priorityRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Priority> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _priorityRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _priorityRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _priorityRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion

        #region List
        public IList<Priority> GetAll(bool showHidden, string projectId)
        {
            var query = from p in _priorityRepository.Table where p.ProjectId == projectId select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }

        public IPagedList<Priority> Get(PrioritySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _priorityRepository.Table
                        join u in _projectRepository.Table on i.ProjectId equals u.Id into ui
                        from iu in ui.DefaultIfEmpty()
                        select new Priority
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            ProjectId = i.ProjectId,
                            Inactive = i.Inactive,

                            Project = iu == null ? null : new Project
                            {
                                Id = iu.Id,
                                Name = iu.Name
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
                            (el.l.LocaleKeyGroup == nameof(Priority) &&
                             el.l.LocaleKey == nameof(Priority.Name) &&
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

            return new PagedList<Priority>(query, ctx.PageIndex, ctx.PageSize);
        }

        public Priority GetByIdName(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var query = (from a in _priorityRepository.Table
                         join b in _projectRepository.Table on a.ProjectId equals b.Id
                         select new Priority
                         {
                             Id = a.Id,
                             Code = a.Code,
                             ProjectId = "[" + b.Code + "] " + b.Name + "",
                             Name = a.Name,
                             Inactive = a.Inactive
                         }).Take(1);
            return query.FirstOrDefault();
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId)
        {
            var results = new List<SelectListItem>();

            var query = from p in _priorityRepository.Table
                        where p.ProjectId == projectId
                        select p;
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
                        Text = $"{x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        #endregion
    }
}