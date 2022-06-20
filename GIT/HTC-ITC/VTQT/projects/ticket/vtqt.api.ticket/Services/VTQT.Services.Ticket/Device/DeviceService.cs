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
    public class DeviceService : IDeviceService
    {
        #region Fields

        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Station> _stationRepository;
        private readonly IRepository<DeviceCategory> _deviceCategoryRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;

        #endregion Fields

        #region Ctor

        public DeviceService()
        {
            _deviceRepository = EngineContext.Current.Resolve<IRepository<Device>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _stationRepository = EngineContext.Current.Resolve<IRepository<Station>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceCategoryRepository = EngineContext.Current.Resolve<IRepository<DeviceCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(Device entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var utcNow = DateTime.UtcNow;
            var result = await _deviceRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Device entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var result = await _deviceRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _deviceRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Device> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _deviceRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _deviceRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion Methods

        #region List

        public IPagedList<Device> Get(DeviceSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _deviceRepository.Table
                        join u in _stationRepository.Table on i.StationId equals u.Id into ui
                        join v in _deviceCategoryRepository.Table on i.CategoryId equals v.Id into vi
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        select new Device
                        {
                            Id = i.Id,
                            Name = i.Name,
                            StationId = iu.Name,
                            Address = i.Address,
                            CategoryId = iv.Name,
                            Inactive = i.Inactive,
                            Code= i.Code
                        };

            if (ctx.Keywords != null && ctx.Keywords.HasValue())
            {
                query = query.LeftJoin(_localizedPropertyRepository.DataConnection.GetTable<LocalizedProperty>().DatabaseName(DbHelper.DbNames.Master),
                        (e, l) => e.Id == l.EntityId,
                        (e, l) => new { e, l })
                    .Where(
                        el =>
                            el.e.Name.Contains(ctx.Keywords) ||
                            (el.l.LanguageId == ctx.LanguageId &&
                             el.l.LocaleKeyGroup == nameof(Device) &&
                             el.l.LocaleKey == nameof(Device.Name) &&
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
                orderby p.Name
                select p;

            return new PagedList<Device>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _deviceRepository.Table select p;
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