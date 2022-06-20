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
    public class TicketAreaService : ITicketAreaService
    {
        #region Fields

        private readonly IRepository<TicketArea> _ticketAreaRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion

        #region Ctor

        public TicketAreaService()
        {
            _ticketAreaRepository = EngineContext.Current.Resolve<IRepository<TicketArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion

        #region Methods
        public async Task<int> InsertAsync(TicketArea entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketAreaRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(TicketArea entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketAreaRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _ticketAreaRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<TicketArea> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ticketAreaRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<TicketArea> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _ticketAreaRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _ticketAreaRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _ticketAreaRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }        

        public virtual async Task<bool> ExistsAsync(string code)
        {
            return await _ticketAreaRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(code));
        }

        public virtual async Task<bool> ExistsAsync(string oldCode, string newCode)
        {
            return await _ticketAreaRepository.Table
                .AnyAsync(
                    a =>
                        !string.IsNullOrEmpty(a.Code)
                        && a.Code.Equals(newCode)
                        && !a.Code.Equals(oldCode));
        }
        #endregion

        #region List
        public virtual IPagedList<TicketArea> Get(TicketAreaSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _ticketAreaRepository.Table select p;

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
                             el.l.LocaleKeyGroup == nameof(TicketArea) &&
                             el.l.LocaleKey == nameof(TicketArea.Name) &&
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

            return new PagedList<TicketArea>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<TicketArea> GetAll(bool showHidden = false)
        {
            var query = from p in _ticketAreaRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _ticketAreaRepository.Table 
                select new TicketAreaEntity
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Inactive = p.Inactive
                };
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
                var ticketAreas = query.ToList();
                ticketAreas.Sort();

                ticketAreas.ForEach(x =>
                {
                    var m = new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Name
                    };
                    results.Add(m);
                });
            }

            return results;
        }
        #endregion
    }
}
