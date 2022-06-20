using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public partial class TroubleTicketService : ITroubleTicketService
    {
        #region Fields
        private readonly IRepository<Ticket_VanDe> _troubleTicketRepository;
        private readonly IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;
        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<TicketArea> _ticketAreaRepository;
        private readonly IRepository<TicketProvince> _ticketProvinceRepository;
        private readonly IRepository<TicketProgress> _ticketProgressRepository;
        private readonly IRepository<TicketCategory> _ticketCategoryRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<OrganizationUnit> _organizationUnitRepository;
        private readonly IRepository<Channel> _channelRepository;
        private readonly IRepository<NetworkLink> _networkLinkRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<ChannelTicket> _channelTicketRepository;
        private readonly IRepository<NetworkLinkTicket> _networkLinkTicketRepository;
        private readonly IRepository<DeviceTicket> _deviceTicketRepository;
        private readonly IRepository<CustomerClass> _customerClassRepository;
        private readonly IRepository<TicketReason> _ticketReasonRepository;
        #endregion

        #region Ctor
        public TroubleTicketService()
        {
            _troubleTicketRepository = EngineContext.Current.Resolve<IRepository<Ticket_VanDe>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ticket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketAreaRepository = EngineContext.Current.Resolve<IRepository<TicketArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketProvinceRepository = EngineContext.Current.Resolve<IRepository<TicketProvince>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketProgressRepository = EngineContext.Current.Resolve<IRepository<TicketProgress>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketCategoryRepository = EngineContext.Current.Resolve<IRepository<TicketCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _areaRepository = EngineContext.Current.Resolve<IRepository<Area>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _organizationUnitRepository = EngineContext.Current.Resolve<IRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelRepository = EngineContext.Current.Resolve<IRepository<Channel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _networkLinkRepository = EngineContext.Current.Resolve<IRepository<NetworkLink>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceRepository = EngineContext.Current.Resolve<IRepository<Device>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelTicketRepository = EngineContext.Current.Resolve<IRepository<ChannelTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _networkLinkTicketRepository = EngineContext.Current.Resolve<IRepository<NetworkLinkTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceTicketRepository = EngineContext.Current.Resolve<IRepository<DeviceTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _customerClassRepository = EngineContext.Current.Resolve<IRepository<CustomerClass>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketReasonRepository = EngineContext.Current.Resolve<IRepository<TicketReason>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods               
        public async Task<int> InsertAsync(Ticket_VanDe entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _troubleTicketRepository.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync(Ticket_VanDe entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _troubleTicketRepository.UpdateAsync(entity);
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await _troubleTicketRepository.DeleteAsync(ids);
        }

        public async Task<Ticket_VanDe> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var ticket = await _troubleTicketRepository.GetByIdAsync(id);

            return ticket;
        }

        public async Task<Ticket_VanDe> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var result = await _troubleTicketRepository.Table
                .FirstOrDefaultAsync(x => x.TicketId == ticketId);

            return result;
        }
        #endregion

        #region List
        public IPagedList<TroubleTicketGridModel> Get(TroubleTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from s in _troubleTicketRepository.Table
                        join t in _ticketRepository.Table on s.TicketId equals t.Id into ts
                        from st in ts.DefaultIfEmpty()
                        join tt in _statusRepository.Table on st.Status equals tt.Id into ttst
                        from tstt in ttst.DefaultIfEmpty()
                        where st.ProjectId == ctx.ProjectId
                        orderby tstt.Az
                        select new TroubleTicketGridModel
                        {
                            Id = st.Id,
                            Subject = st.Subject,
                            Code = st.Code,
                            Detail = st.Detail,
                            StartDate = st.StartDate,
                            FinishDate = st.FinishDate,
                            Deadline = st.Deadline,
                            Duration = st.Duration ?? 0,
                            Assignee = st.Assignee,
                            Status = tstt.Name,
                            Priority = st.Priority,
                            ProjectId = st.ProjectId,
                            Category = st.Category,
                            Inactive = st.Inactive,
                            CreatedBy = st.CreatedBy,
                            CreatedDate = st.CreatedDate,
                            ModifiedBy = st.ModifiedBy,
                            ModifiedDate = st.ModifiedDate,
                            FirstReason = st.FirstReason,
                            LastReason = st.LastReason,
                            Note = st.Note,
                            Solution = st.Solution,
                            PendingHourTime = st.PendingHourTime,
                            TicketId = s.TicketId,
                            TicketArea = s.TicketArea,
                            TicketProvince = s.TicketProvince,
                            HourTimeMinus = s.HourTimeMinus,
                            MinuteTimeMinus = s.MinuteTimeMinus,
                            SecondTimeMinus = s.SecondTimeMinus,
                            ProcessingUnit = s.ProcessingUnit,
                            ManagementUnit = s.ManagementUnit,
                            Sla = s.Sla,
                            SlaOver = s.SlaOver,
                            ImportantTicket = s.ImportantTicket
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Subject.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords) ||
                              p.Detail.Contains(ctx.Keywords) ||
                              p.LastReason.Contains(ctx.Keywords) ||
                              p.FirstReason.Contains(ctx.Keywords) ||
                              p.Note.Contains(ctx.Keywords)
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.StatusId))
            {
                query = from p in query
                        where p.Status == ctx.StatusId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.Assignee))
            {
                query = from p in query
                        where p.Assignee == ctx.Assignee
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CreatedBy))
            {
                query = from p in query
                        where p.CreatedBy == ctx.CreatedBy
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.PriorityId))
            {
                query = from p in query
                        where p.Priority == ctx.PriorityId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.TicketAreaId))
            {
                query = from p in query
                        where p.TicketArea == ctx.TicketAreaId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.TicketProvinceId))
            {
                query = from p in query
                        where p.TicketProvince == ctx.TicketProvinceId
                        select p;
            }

            if (ctx.StartDate.HasValue)
            {
                query = from p in query
                        where p.StartDate.Value >= ctx.StartDate.Value
                        select p;
            }

            if (ctx.FinishDate.HasValue)
            {
                query = from p in query
                        where p.FinishDate.Value <= ctx.FinishDate.Value
                        select p;
            }

            return new PagedList<TroubleTicketGridModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IPagedList<TroubleTicketExcelModel> GetExcelData(TroubleTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from troubleTicket in _troubleTicketRepository.Table
                        join ticket in _ticketRepository.Table on troubleTicket.TicketId equals ticket.Id into a
                        from troubleTicketTicket in a.DefaultIfEmpty()
                        join status in _statusRepository.Table on troubleTicketTicket.Status equals status.Id into b
                        from troubleTicketStatus in b.DefaultIfEmpty()
                        join ticketArea in _ticketAreaRepository.Table on troubleTicket.TicketArea equals ticketArea.Id into c
                        from troubleTicketArea in c.DefaultIfEmpty()
                        join ticketProvince in _ticketProvinceRepository.Table on troubleTicket.TicketProvince equals ticketProvince.Id into d
                        from troubleTicketProvince in d.DefaultIfEmpty()
                        join area in _areaRepository.Table on troubleTicketProvince.AreaId equals area.Id into e
                        from troubleTicketProvinceArea in e.DefaultIfEmpty()
                        join ticketProgress in _ticketProgressRepository.Table on troubleTicket.TicketProgressId equals ticketProgress.Id into f
                        from troubleTicketProgress in f.DefaultIfEmpty()
                        join ticketCategory in _ticketCategoryRepository.Table on troubleTicket.TicketCategoryId equals ticketCategory.Id into g
                        from troubleTicketCategory in g.DefaultIfEmpty()
                        join organizationUnit in _organizationUnitRepository.Table on troubleTicket.ManagementUnit equals organizationUnit.Id into h
                        from troubleTicketManagementUnit in h.DefaultIfEmpty()
                        join organizationUnit in _organizationUnitRepository.Table on troubleTicket.ProcessingUnit equals organizationUnit.Id into i
                        from troubleTicketProcessingUnit in i.DefaultIfEmpty()
                        join channelTicket in _channelTicketRepository.Table on troubleTicketTicket.Id equals channelTicket.TicketId into j
                        from troubleChannelTicket in j.DefaultIfEmpty()
                        join channel in _channelRepository.Table on troubleChannelTicket.ChannelId equals channel.Id into k
                        from channel in k.DefaultIfEmpty()
                        join networkTicket in _networkLinkTicketRepository.Table on troubleTicketTicket.Id equals networkTicket.TicketId into l
                        from troubleNetworkTicket in l.DefaultIfEmpty()
                        join networkLink in _networkLinkRepository.Table on troubleNetworkTicket.NetworkLinkId equals networkLink.Id into m
                        from networkLink in m.DefaultIfEmpty()
                        join deviceTicket in _deviceTicketRepository.Table on troubleTicketTicket.Id equals deviceTicket.TicketId into n
                        from troubleDeviceTicket in n.DefaultIfEmpty()
                        join device in _deviceRepository.Table on troubleDeviceTicket.DeviceId equals device.Id into o
                        from device in o.DefaultIfEmpty()
                        join customerClass in _customerClassRepository.Table on channel.CustomerClass equals customerClass.Id into p
                        from troubleTicketCustomer in p.DefaultIfEmpty()
                        where troubleTicketTicket.ProjectId == ctx.ProjectId
                        orderby troubleTicketStatus.Az
                        select new TroubleTicketExcelModel
                        {
                            Code = troubleTicketTicket.Code,
                            Subject = troubleTicketTicket.Subject,
                            ChannelCode = channel.HtcCid,
                            ChannelName = channel.Name,
                            NetworkLinkCode = networkLink.Code,
                            NetworkLinkName = networkLink.Name,
                            DeviceCode = device.Code,
                            DeviceName = device.Name,
                            CreatedBy = troubleTicketTicket.CreatedBy,
                            CreatedDate = troubleTicketTicket.StartDate.Value.ToString("dd/MM/yyyy"),
                            ProcessingUnit = troubleTicketProcessingUnit.Name,
                            ManagementUnit = troubleTicketManagementUnit.Name,
                            Category = troubleTicketCategory.Name,
                            StartDate = troubleTicketTicket.StartDate.HasValue ? troubleTicketTicket.StartDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            FinishDate = troubleTicketTicket.FinishDate.HasValue ? troubleTicketTicket.FinishDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            DurationTime = (troubleTicketTicket.FinishDate.HasValue && troubleTicketTicket.StartDate.HasValue) ?
                                TimeSpan.FromSeconds(troubleTicketTicket.FinishDate.Value.ToUnixTime() - troubleTicketTicket.StartDate.Value.ToUnixTime()).ToString(@"dd") + " ngày - " + TimeSpan.FromSeconds(troubleTicketTicket.FinishDate.Value.ToUnixTime() - troubleTicketTicket.StartDate.Value.ToUnixTime()).ToString(@"hh\:mm\:ss") : "",
                            Deadline = troubleTicketTicket.Deadline.HasValue ? troubleTicketTicket.Deadline.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            TicketAreaName = troubleTicketArea.Name,
                            TicketProvinceName = troubleTicketProvince.Name,
                            MinusTime = MinusTime(troubleTicket),
                            SlaTime = SlaTime(troubleTicket, troubleTicketTicket),
                            CustomerClass = troubleTicketCustomer.Name,
                            FirstReason = troubleTicketTicket.FirstReason,
                            LastReason = troubleTicketTicket.LastReason,
                            Detail = troubleTicketTicket.Detail,
                            Solution = troubleTicketTicket.Solution,
                            Note = troubleTicketTicket.Note,
                            Month = troubleTicketTicket.StartDate.Value.ToString("MM"),
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(troubleTicketTicket.StartDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                            TicketProgress = troubleTicketProgress.Name,
                            AreaName = troubleTicketProvinceArea.Name,
                            Status = troubleTicketTicket.Status,
                            Assignee = troubleTicketTicket.Assignee,
                            Priority = troubleTicketTicket.Priority,
                            TicketArea = troubleTicket.TicketArea,
                            TicketProvince = troubleTicket.TicketProvince,
                            StartDateTime = troubleTicketTicket.StartDate,
                            FinishDateTime = troubleTicketTicket.FinishDate
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Subject.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords) ||
                              p.Detail.Contains(ctx.Keywords) ||
                              p.LastReason.Contains(ctx.Keywords) ||
                              p.FirstReason.Contains(ctx.Keywords) ||
                              p.Note.Contains(ctx.Keywords)
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.StatusId))
            {
                query = from p in query
                        where p.Status == ctx.StatusId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.Assignee))
            {
                query = from p in query
                        where p.Assignee == ctx.Assignee
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CreatedBy))
            {
                query = from p in query
                        where p.CreatedBy == ctx.CreatedBy
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.PriorityId))
            {
                query = from p in query
                        where p.Priority == ctx.PriorityId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.TicketAreaId))
            {
                query = from p in query
                        where p.TicketArea == ctx.TicketAreaId
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.TicketProvinceId))
            {
                query = from p in query
                        where p.TicketProvince == ctx.TicketProvinceId
                        select p;
            }

            if (ctx.StartDate.HasValue)
            {
                query = from p in query
                        where p.StartDateTime.Value >= ctx.StartDate.Value
                        select p;
            }

            if (ctx.FinishDate.HasValue)
            {
                query = from p in query
                        where p.FinishDateTime.Value <= ctx.FinishDate.Value
                        select p;
            }

            return new PagedList<TroubleTicketExcelModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        private string MinusTime(Ticket_VanDe troubleTicket)
        {
            var hour = troubleTicket.HourTimeMinus;
            var minute = troubleTicket.MinuteTimeMinus;
            var second = troubleTicket.SecondTimeMinus;
            var totalSecondMinus = 0;
            var timeMinus = "";

            if (hour.HasValue)
            {
                totalSecondMinus += hour.Value * 3600;
            }

            if (minute.HasValue)
            {
                totalSecondMinus += minute.Value * 60;
            }           

            if (second.HasValue)
            {
                totalSecondMinus += second.Value;
            }
            
            if (totalSecondMinus > 0)
            {
                timeMinus = TimeSpan.FromSeconds(totalSecondMinus).ToString(@"dd") + " ngày - " +
                            TimeSpan.FromSeconds(totalSecondMinus).ToString(@"hh\:mm\:ss");
            }

            return timeMinus;
        }

        private string SlaTime(Ticket_VanDe troubleTicket, Core.Domain.Ticket.Ticket troubleTicketTicket)
        {
            var hour = troubleTicket.HourTimeMinus;
            var minute = troubleTicket.MinuteTimeMinus;
            var second = troubleTicket.SecondTimeMinus;
            var sla = "";            

            if (troubleTicketTicket.FinishDate.HasValue && troubleTicketTicket.StartDate.HasValue)
            {
                var duration = TimeSpan.FromSeconds(troubleTicketTicket.FinishDate.Value.ToUnixTime() - troubleTicketTicket.StartDate.Value.ToUnixTime());
                var totalSecondMinus = 0;
                if (hour.HasValue)
                {
                    totalSecondMinus += hour.Value * 3600;
                }
                if (minute.HasValue)
                {
                    totalSecondMinus += minute.Value * 60;
                }                
                if (second.HasValue)
                {
                    totalSecondMinus += second.Value;
                }
                sla = TimeSpan.FromSeconds(duration.TotalSeconds - totalSecondMinus).ToString(@"dd") + " ngày - " +
                      TimeSpan.FromSeconds(duration.TotalSeconds - totalSecondMinus).ToString(@"hh\:mm\:ss");
            }

            return sla;
        }
        #endregion
    }
}
