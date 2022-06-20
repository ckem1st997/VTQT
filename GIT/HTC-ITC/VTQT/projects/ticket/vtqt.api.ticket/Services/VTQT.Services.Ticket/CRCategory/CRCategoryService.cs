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
    public class CRCategoryService : ICRCategoryService
    {
        #region Fields

        private readonly IRepository<CRCategory> _crCategoryRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion Fields

        #region Ctor

        public CRCategoryService()
        {
            _crCategoryRepository = EngineContext.Current.Resolve<IRepository<CRCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(CRCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _crCategoryRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(CRCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _crCategoryRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _crCategoryRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<CRCategory> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _crCategoryRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<CRCategory> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _crCategoryRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _crCategoryRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _crCategoryRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion Methods

        #region List

        public IList<CRCategory> GetAll(bool showHidden, string projectId)
        {
            var query = from p in _crCategoryRepository.Table where p.ProjectId == projectId select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }

        public Core.IPagedList<CRCategory> Get(CRCategorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _crCategoryRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el => el.e.Code.Contains(ctx.Keywords) ||
                              el.e.Name.Contains(ctx.Keywords) ||
                             (el.l.LanguageId == ctx.LanguageId &&
                              el.l.LocaleKeyGroup == nameof(CRCategory) &&
                              el.l.LocaleKey == nameof(CRCategory.Name) &&
                              el.l.LocaleValue.Contains(ctx.Keywords)))
                    .Select(el => el.e).Distinct();
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query = from p in query
                        where !p.Inactive
                        select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query = from p in query
                        where p.Inactive
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<CRCategory>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden, string projectId)
        {
            var results = new List<SelectListItem>();

            var query = from p in _crCategoryRepository.Table where p.ProjectId == projectId select p;
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
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        #endregion List
    }
}