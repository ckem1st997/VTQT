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
    public class ChannelService : IChannelService
    {
        #region Fields

        private readonly IRepository<Channel> _channelRepository;
        private readonly IRepository<ChannelCategory> _channelCategoryRepository;
        private readonly IRepository<ChannelStatus> _channelStatusRepository;
        private readonly IRepository<ChannelArea> _ChannelAreaRepository;
        private readonly IRepository<CustomerClass> _customerClassRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ChannelService(IWorkContext workContext)
        {
            _channelRepository = EngineContext.Current.Resolve<IRepository<Channel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelCategoryRepository = EngineContext.Current.Resolve<IRepository<ChannelCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelStatusRepository = EngineContext.Current.Resolve<IRepository<ChannelStatus>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ChannelAreaRepository = EngineContext.Current.Resolve<IRepository<ChannelArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _customerClassRepository = EngineContext.Current.Resolve<IRepository<CustomerClass>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _localizedPropertyRepository = EngineContext.Current.Resolve<IRepository<LocalizedProperty>>(DataConnectionHelper.ConnectionStringNames.Master);
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(Channel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _channelRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Channel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var utcNow = DateTime.UtcNow;
            entity.ModifiedBy = _workContext.UserId;
            entity.ModifiedDate = utcNow;
            var result = await _channelRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _channelRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Channel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _channelRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Channel> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _channelRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            return await _channelRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _channelRepository.Table
                         .Where(w => ids.Contains(w.Id))
                         .Set(x => x.Inactive, !active)
                         .UpdateAsync();

            return result;
        }

        #endregion

        #region List

        public IPagedList<Channel> Get(ChannelSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _channelRepository.Table
                        join u in _channelCategoryRepository.Table on i.CategoryId equals u.Id into ui
                        join v in _channelStatusRepository.Table on i.ChannelStatusId equals v.Id into vi
                        join t in _ChannelAreaRepository.Table on i.ChannelAreaId equals t.Id into ti
                        join h in _customerClassRepository.Table on i.CustomerClass equals h.Id into hi
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        from it in ti.DefaultIfEmpty()
                        from ih in hi.DefaultIfEmpty()
                        select new Channel
                        {
                            Id = i.Id,
                            Code = i.Code,
                            Name = i.Name,
                            CustomerName = i.CustomerName,
                            CustomerCode = i.CustomerCode,
                            CustomerChannel = i.CustomerChannel,
                            CategoryId = iu.Name,
                            HtcCid = i.HtcCid,
                            HicCidOld = i.HicCidOld,
                            StartPoint = i.StartPoint,
                            EndPoint = i.EndPoint,
                            DistanceKilometer = i.DistanceKilometer,
                            DistanceTimesBandwidth = i.DistanceTimesBandwidth,
                            ChannelStatusId = iv.Name,
                            TotalBandwidth = i.TotalBandwidth,
                            DomesticBandwidth = i.DomesticBandwidth,
                            OverseasBandwidth = i.OverseasBandwidth,
                            Vlan = i.Vlan,
                            Vcid = i.Vcid,
                            Vrf = i.Vrf,
                            IpAddress = i.IpAddress,
                            StartPointDevicePop = i.StartPointDevicePop,
                            StartPointPortPop = i.StartPointPortPop,
                            EndPointDevicePop = i.EndPointDevicePop,
                            EndPointPortPop = i.EndPointPortPop,
                            ChannelAreaId = it.Name,
                            ContractCode = i.ContractCode,
                            ModifiedBandwidthNote = i.ModifiedBandwidthNote,
                            StartDate = i.StartDate,
                            EndDate = i.EndDate,
                            UpRole = i.UpRole,
                            EndRole = i.EndRole,
                            Note = i.Note,
                            CurrentCoreStatus = i.CurrentCoreStatus,
                            CurrentLastmileStatus = i.CurrentLastmileStatus,
                            CustomerContact = i.CustomerContact,
                            CreatedDate = i.CreatedDate,
                            ModifiedDate = i.ModifiedDate,
                            CreatedBy = i.CreatedBy,
                            ModifiedBy = i.ModifiedBy,
                            AreaA = i.AreaA,
                            AreaB = i.AreaB,
                            CustomerClass = ih.Name,
                            ProvinceA = i.ProvinceA,
                            ProvinceB = i.ProvinceB,
                            LinkId = i.LinkId,
                            Sla = i.Sla,
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
                             el.l.LocaleKeyGroup == nameof(Channel) &&
                             el.l.LocaleKey == nameof(Channel.Name) &&
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

            return new PagedList<Channel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<SelectListItem> GetMvcListItems(bool showHidden)
        {
            var results = new List<SelectListItem>();

            var query = from p in _channelRepository.Table select p;
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