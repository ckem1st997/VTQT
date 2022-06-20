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
    public class StationService : IStationService
    {
        #region Fields

        private readonly IRepository<Station> _stationServiceRepository;
        private readonly IRepository<StationCategory> _stationCategoryServiceRepository;
        private readonly IRepository<StationLevel> _stationLevelServiceRepository;
        private readonly IRepository<Area> _areaServiceRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion

        #region Ctor

        public StationService()
        {
            _stationServiceRepository = EngineContext.Current.Resolve<IRepository<Station>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _stationCategoryServiceRepository = EngineContext.Current.Resolve<IRepository<StationCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _stationLevelServiceRepository = EngineContext.Current.Resolve<IRepository<StationLevel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _areaServiceRepository = EngineContext.Current.Resolve<IRepository<Area>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(Station entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var utcNow = DateTime.UtcNow;
            var result = await _stationServiceRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Station entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var utcNow = DateTime.UtcNow;
            var result = await _stationServiceRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _stationServiceRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Station> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _stationServiceRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Station> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _stationServiceRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _stationServiceRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _stationServiceRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion

        #region List

        public IPagedList<Station> Get(StationSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _stationServiceRepository.Table
                        join u in _areaServiceRepository.Table on i.AreaId equals u.Id into ui
                        join v in _stationLevelServiceRepository.Table on i.StationLevel equals v.Id into vi
                        join t in _stationCategoryServiceRepository.Table on i.StationCategory equals t.Id into ti
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        from it in ti.DefaultIfEmpty()
                        select new Station
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            CodeVnm = i.CodeVnm,
                            AreaId = iu.Name,
                            ProvinceName = i.ProvinceName,
                            StationLevel = iv.Name,
                            Address = i.Address,
                            Longitude = i.Longitude,
                            Latitude = i.Latitude,
                            Note = i.Note,
                            StationCategory = it.Name,
                            Inactive = i.Inactive,
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
                             el.l.LocaleKeyGroup == nameof(Station) &&
                             el.l.LocaleKey == nameof(Station.Name) &&
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

            return new PagedList<Station>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Station> GetAll(bool showHidden = false)
        {
            var query = from p in _stationServiceRepository.Table select p;
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

            var query = from p in _stationServiceRepository.Table select p;
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