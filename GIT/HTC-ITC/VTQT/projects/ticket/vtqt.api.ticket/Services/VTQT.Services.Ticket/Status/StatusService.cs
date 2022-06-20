﻿using LinqToDB;
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
    public partial class StatusService : IStatusSerive
    {
        #region Fields
        private readonly IRepository<StatusCategory> _statusCategoryRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion

        #region Ctor

        public StatusService()
        {
            _statusCategoryRepository = EngineContext.Current.Resolve<IRepository<StatusCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _projectRepository = EngineContext.Current.Resolve<IRepository<Project>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(Status entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _statusRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Status entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _statusRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _statusRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Status> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _statusRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Status> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _statusRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _statusRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _statusRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion

        #region List

        public IList<Status> GetAll(bool showHidden, string projectId)
        {
            var query = from p in _statusRepository.Table where p.ProjectId == projectId select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }

        public IPagedList<Status> Get(StatusSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _statusRepository.Table
                        join u in _projectRepository.Table on i.ProjectId equals u.Id into ui
                        join v in _statusCategoryRepository.Table on i.StatusCategoryId equals v.Id into vi
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        select new Status
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            StatusCategoryId = i.StatusCategoryId,
                            ProjectId = i.ProjectId,
                            Inactive = i.Inactive,

                            Project = iu == null ? null : new Project
                            {
                                Id = iu.Id,
                                Name = iu.Name
                            },
                            StatusCategory = iv == null ? null : new StatusCategory
                            {
                                Id = iv.Id,
                                Code=iv.Code,
                                Name=iv.Name,
                            },
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
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Status) &&
                             el.l.LocaleKey == nameof(Status.Name) &&
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

            return new PagedList<Status>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId)
        {
            var results = new List<SelectListItem>();

            var query = from p in _statusRepository.Table 
                        where p.ProjectId == projectId
                        select p;
            if (!showHidden)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }

            query = from p in query
                    orderby p.Az
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