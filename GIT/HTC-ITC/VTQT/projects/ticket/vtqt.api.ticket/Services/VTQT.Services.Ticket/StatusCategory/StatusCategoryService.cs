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
    public partial class StatusCategoryService : IStatusCategoryService
    {
        #region Fields
        private readonly IRepository<StatusCategory> _statusCategoryRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        #endregion

        #region Ctor
        public StatusCategoryService()
        {
            _statusCategoryRepository = EngineContext.Current.Resolve<IRepository<StatusCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(StatusCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _statusCategoryRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(StatusCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _statusCategoryRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _statusCategoryRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<StatusCategory> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _statusCategoryRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<StatusCategory> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _statusCategoryRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _statusCategoryRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _statusCategoryRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }
        #endregion

        #region List
        public Core.IPagedList<StatusCategory> Get(StatusCategorySearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _statusCategoryRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(x => x.Name.Contains(ctx.Keywords) || x.Code.Contains(ctx.Keywords));
            }
            if (ctx.Status == (int)ActiveStatus.Activated)
            {
                query = from p in query
                        where p.Inactive==false
                        select p;
            }
            if (ctx.Status == (int)ActiveStatus.Deactivated)
            {
                query = from p in query
                        where p.Inactive==true
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<StatusCategory>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListAsync(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _statusCategoryRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query
                        where p.Inactive==false
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
                        Value = x.Code,
                        Text = $"[{x.Code}] {x.Name}"
                    };
                    results.Add(m);
                });
            }

            return results;
        }

        public IList<StatusCategory> GetAll(bool showHidden = false)
        {

            var query = from p in _statusCategoryRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive==false select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
