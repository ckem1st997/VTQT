using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class CableService : ICableService
    {
        #region Fields

        private readonly IRepository<Cable> _cableRepository;

        #endregion Fields

        #region Ctor

        public CableService()
        {
            _cableRepository = EngineContext.Current.Resolve<IRepository<Cable>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(Cable entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _cableRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Cable entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _cableRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _cableRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Cable> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _cableRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _cableRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion Methods

        #region List

        public Core.IPagedList<Cable> Get(CableSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _cableRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.Where(x => x.Name.Contains(ctx.Keywords));
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
                    orderby p.Name
                    select p;

            return new PagedList<Cable>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Cable> GetAll(bool showHidden = false)
        {

            var query = from p in _cableRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Name select p;
            return query.ToList();
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _cableRepository.Table select p;
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
        #endregion
    }
}