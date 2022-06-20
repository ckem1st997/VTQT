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
    public partial class TicketProvinceService : ITicketProvinceService
    {
        #region Fields
        private readonly IRepository<TicketProvince> _ticketProvinceRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        #endregion

        #region Ctor
        public TicketProvinceService()
        {
            _ticketProvinceRepository = EngineContext.Current.Resolve<IRepository<TicketProvince>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(TicketProvince entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketProvinceRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(TicketProvince entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketProvinceRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _ticketProvinceRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<TicketProvince> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ticketProvinceRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<TicketProvince> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _ticketProvinceRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _ticketProvinceRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _ticketProvinceRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }
        #endregion

        #region List
        public Core.IPagedList<TicketProvince> Get(TicketProvinceSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _ticketProvinceRepository.Table select p;

            if (ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el => el.e.Code.Contains(ctx.Keywords) ||
                              el.e.Name.Contains(ctx.Keywords) ||
                             (el.l.LanguageId == ctx.LanguageId &&
                              el.l.LocaleKeyGroup == nameof(TicketProvince) &&
                              el.l.LocaleKey == nameof(TicketProvince.Name) &&
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

            return new PagedList<TicketProvince>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _ticketProvinceRepository.Table select p;
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

        public IList<TicketProvince> GetAll(bool showHidden = false)
        {
            var query = from p in _ticketProvinceRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Code select p;
            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
