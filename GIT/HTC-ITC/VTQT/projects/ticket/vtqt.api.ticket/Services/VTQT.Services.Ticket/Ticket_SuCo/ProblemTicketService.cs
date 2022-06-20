using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.Services.Ticket
{
    public partial class ProblemTicketService : IProblemTicketService
    {
        #region Fields
        private readonly IRepository<TicketCategory> _ticketCategoryRepository;
        private readonly IRepository<OrganizationUnit> _organizationUnitRepository;
        private readonly IRepository<TicketArea> _ticketAreaRepository;
        private readonly IRepository<TicketProvince> _ticketProvinceRepository;
        private readonly IRepository<TicketReason> _ticketReasonRepository;
        private readonly IRepository<Ticket_SuCo> _problemTicketRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<Priority> _priorityRepository;
        private readonly IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;
        private readonly IRepository<Channel> _channelRepository;
        private readonly IRepository<NetworkLink> _networkLinkRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<ChannelTicket> _channelTicketRepository;
        private readonly IRepository<NetworkLinkTicket> _networkLinkTicketRepository;
        private readonly IRepository<DeviceTicket> _deviceTicketRepository;
        private readonly IRepository<CustomerClass> _customerClassRepository;
        private readonly IRepository<SlaLevel> _slaLevelRepository;
        #endregion

        #region Ctor
        public ProblemTicketService()
        {
            _ticketCategoryRepository = EngineContext.Current.Resolve<IRepository<TicketCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _priorityRepository = EngineContext.Current.Resolve<IRepository<Priority>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _problemTicketRepository = EngineContext.Current.Resolve<IRepository<Ticket_SuCo>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ticket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketReasonRepository = EngineContext.Current.Resolve<IRepository<TicketReason>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketProvinceRepository = EngineContext.Current.Resolve<IRepository<TicketProvince>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketAreaRepository = EngineContext.Current.Resolve<IRepository<TicketArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _organizationUnitRepository = EngineContext.Current.Resolve<IRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _areaRepository = EngineContext.Current.Resolve<IRepository<Area>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelRepository = EngineContext.Current.Resolve<IRepository<Channel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _networkLinkRepository = EngineContext.Current.Resolve<IRepository<NetworkLink>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceRepository = EngineContext.Current.Resolve<IRepository<Device>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelTicketRepository = EngineContext.Current.Resolve<IRepository<ChannelTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _networkLinkTicketRepository = EngineContext.Current.Resolve<IRepository<NetworkLinkTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceTicketRepository = EngineContext.Current.Resolve<IRepository<DeviceTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _customerClassRepository = EngineContext.Current.Resolve<IRepository<CustomerClass>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _slaLevelRepository = EngineContext.Current.Resolve<IRepository<SlaLevel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(Ticket_SuCo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _problemTicketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Ticket_SuCo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _problemTicketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _problemTicketRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Ticket_SuCo> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _problemTicketRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Ticket_SuCo> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var result = await _problemTicketRepository.Table
                .FirstOrDefaultAsync(x => x.TicketId == ticketId);

            return result;
        }
        #endregion

        #region List
        public async Task<IPagedList<ProblemTicketGridModel>> Get(ProblemTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _problemTicketRepository.Table
                        join t in _ticketRepository.Table on p.TicketId equals t.Id into pt
                        from tp in pt.DefaultIfEmpty()
                        join a in _ticketReasonRepository.Table on p.KindOfReason equals a.Id into at
                        from ta in at.DefaultIfEmpty()
                        join i in _ticketProvinceRepository.Table on p.TicketProvince equals i.Id into it
                        from ti in it.DefaultIfEmpty()
                        join t in _ticketAreaRepository.Table on p.TicketArea equals t.Id into tn
                        from tt in tn.DefaultIfEmpty()
                        join u in _organizationUnitRepository.Table on p.ProcessingDepartment equals u.Id into un
                        from ut in un.DefaultIfEmpty()
                        join e in _organizationUnitRepository.Table on p.ProcessingUnit equals e.Id into en
                        from et in en.DefaultIfEmpty()
                        join l in _statusRepository.Table on tp.Status equals l.Id into ln
                        from lt in ln.DefaultIfEmpty()
                        join k in _priorityRepository.Table on tp.Priority equals k.Id into kn
                        from kt in kn.DefaultIfEmpty()
                        join j in _ticketCategoryRepository.Table on tp.Category equals j.Id into jn
                        from jt in jn.DefaultIfEmpty()
                        where tp.ProjectId == ctx.ProjectId
                        orderby lt.Az
                        select new ProblemTicketGridModel
                        {                     
                            Id = tp.Id,
                            TicketId = tp.Id,
                            PendingHourTime = tp.PendingHourTime,
                            TicketArea = tt.Name,
                            TicketAreaId=tt.Id,
                            TicketProvince = ti.Name,
                            ChannelCapacity = p.ChannelCapacity,
                            EcalatePosition = p.EcalatePosition,
                            Solution = tp.Solution,
                            ProcessingUnit = et.Name,
                            ProcessingDepartment = ut.Name,
                            DetailReason = p.DetailReason,
                            SlaOver = p.SlaOver,
                            ImportantTicket = p.ImportantTicket,
                            Sla = p.Sla,
                            HourTimeMinus = p.HourTimeMinus,
                            MinuteTimeMinus = p.MinuteTimeMinus,
                            KindOfReason = ta.Description,
                            Note = tp.Note,
                            LastReason = tp.LastReason,
                            FirstReason = tp.FirstReason,
                            Code = tp.Code,
                            Subject = tp.Subject,
                            Detail = tp.Detail,
                            StartDate = tp.StartDate,
                            FinishDate = tp.FinishDate,
                            Deadline = tp.Deadline,
                            Duration = (int)tp.Duration,
                            Assignee = tp.Assignee,
                            Status = lt.Name,
                            StatusId=lt.Id,
                            Priority = kt.Name,
                            PriorityId=kt.Id,
                            ProjectId = tp.ProjectId,
                            Category = jt.Name,
                            Inactive = tp.Inactive,
                            CreatedBy = tp.CreatedBy,
                            CreatedDate = tp.CreatedDate,
                            ModifiedBy = tp.ModifiedBy,
                            ModifiedDate = tp.ModifiedDate,
                            SecondTimeMinus = p.SecondTimeMinus                            
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Subject.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords)||
                              p.Sla.ToString().Contains(ctx.Keywords)||
                              p.TicketId.Contains(ctx.Keywords)
                        select p;
            }
            if (ctx.StartDate.HasValue)
            {
                query = from p in query
                        where p.StartDate >= ctx.StartDate.Value
                        select p;
            }
            if (ctx.FinishDate.HasValue)
            {
                query = from p in query
                        where p.FinishDate <= ctx.FinishDate.Value
                        select p;
            }
            if (!string.IsNullOrEmpty(ctx.CreatedBy))
            {
                query = from p in query
                        where p.CreatedBy == ctx.CreatedBy
                        select p;
            }
            if (!string.IsNullOrEmpty(ctx.Priority))
            {
                query = from p in query
                        where p.PriorityId == ctx.Priority
                        select p;
            }
            if (!string.IsNullOrEmpty(ctx.Trangthai))
            {
                query = from l in query
                        where l.StatusId == ctx.Trangthai
                        select l;
            }
            if (!string.IsNullOrEmpty(ctx.Vungsuco))
            {
                query = from p in query
                        where p.TicketAreaId == ctx.Vungsuco
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<ProblemTicketGridModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<Ticket_SuCo> GetByTicketSucoId(ProblemTicketSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.TicketId))
                throw new ArgumentNullException(nameof(ctx.TicketId));

            var query =
                from x in _problemTicketRepository.Table
                where x.TicketId == ctx.TicketId
                select x;

            return query.ToList();
        }

        public IPagedList<ProblemTicketExcelModel> GetExcelData(ProblemTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from problemTicket in _problemTicketRepository.Table
                        join ticket in _ticketRepository.Table on problemTicket.TicketId equals ticket.Id into a
                        from problemTicketTicket in a.DefaultIfEmpty()
                        join status in _statusRepository.Table on problemTicketTicket.Status equals status.Id into b
                        from problemTicketStatus in b.DefaultIfEmpty()
                        join ticketArea in _ticketAreaRepository.Table on problemTicket.TicketArea equals ticketArea.Id into c
                        from problemTicketArea in c.DefaultIfEmpty()
                        join ticketProvince in _ticketProvinceRepository.Table on problemTicket.TicketProvince equals ticketProvince.Id into d
                        from problemTicketProvince in d.DefaultIfEmpty()
                        join area in _areaRepository.Table on problemTicketProvince.AreaId equals area.Id into e
                        from problemTicketProvinceArea in e.DefaultIfEmpty()                                                
                        join organizationUnit in _organizationUnitRepository.Table on problemTicket.ProcessingUnit equals organizationUnit.Id into i
                        from problemTicketProcessingUnit in i.DefaultIfEmpty()
                        join channelTicket in _channelTicketRepository.Table on problemTicketTicket.Id equals channelTicket.TicketId into j
                        from problemChannelTicket in j.DefaultIfEmpty()
                        join channel in _channelRepository.Table on problemChannelTicket.ChannelId equals channel.Id into k
                        from channel in k.DefaultIfEmpty()
                        join networkTicket in _networkLinkTicketRepository.Table on problemTicketTicket.Id equals networkTicket.TicketId into l
                        from problemNetworkTicket in l.DefaultIfEmpty()
                        join networkLink in _networkLinkRepository.Table on problemNetworkTicket.NetworkLinkId equals networkLink.Id into m
                        from networkLink in m.DefaultIfEmpty()
                        join deviceTicket in _deviceTicketRepository.Table on problemTicketTicket.Id equals deviceTicket.TicketId into n
                        from problemDeviceTicket in n.DefaultIfEmpty()
                        join device in _deviceRepository.Table on problemDeviceTicket.DeviceId equals device.Id into o
                        from device in o.DefaultIfEmpty()
                        join customerClass in _customerClassRepository.Table on channel.CustomerClass equals customerClass.Id into p
                        from problemTicketCustomer in p.DefaultIfEmpty()
                        join slaLevel in _slaLevelRepository.Table on problemTicketCustomer.Id equals slaLevel.CustomerClass into q
                        from problemSlaLevel in q.DefaultIfEmpty()
                        where problemTicketTicket.ProjectId == ctx.ProjectId
                        orderby problemTicketStatus.Az
                        select new ProblemTicketExcelModel
                        {
                            Code = problemTicketTicket.Code,
                            Subject = problemTicketTicket.Subject,
                            ChannelCode = channel.HtcCid,
                            ChannelName = channel.Name,
                            NetworkLinkCode = networkLink.Code,
                            NetworkLinkName = networkLink.Name,
                            DeviceCode = device.Code,
                            DeviceName = device.Name,                            
                            CreatedDate = problemTicketTicket.StartDate.Value.ToString("dd/MM/yyyy"),
                            ProcessingUnit = problemTicketProcessingUnit.Name,
                            StartDate = problemTicketTicket.StartDate.HasValue ? problemTicketTicket.StartDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            FinishDate = problemTicketTicket.FinishDate.HasValue ? problemTicketTicket.FinishDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            DurationTime = (problemTicketTicket.FinishDate.HasValue && problemTicketTicket.StartDate.HasValue) ?
                                TimeSpan.FromSeconds(problemTicketTicket.FinishDate.Value.ToUnixTime() - problemTicketTicket.StartDate.Value.ToUnixTime()).ToString(@"dd") + " ngày - " + TimeSpan.FromSeconds(problemTicketTicket.FinishDate.Value.ToUnixTime() - problemTicketTicket.StartDate.Value.ToUnixTime()).ToString(@"hh\:mm\:ss") : "",
                            TicketAreaName = problemTicketArea.Name,
                            TicketProvinceName = problemTicketProvince.Name,
                            MinusTime = MinusTime(problemTicket),
                            SlaTime = SlaTime(problemTicket, problemTicketTicket),
                            CustomerClass = problemTicketCustomer.Name,
                            LastReason = problemTicketTicket.LastReason,
                            Detail = problemTicketTicket.Detail,
                            Solution = problemTicketTicket.Solution,
                            Note = problemTicketTicket.Note,
                            Status = problemTicketTicket.Status,
                            Priority = problemTicketTicket.Priority,
                            CreatedBy = problemTicketTicket.CreatedBy,
                            TicketArea = problemTicket.TicketArea,
                            StartDateTime = problemTicketTicket.StartDate,
                            FinishDateTime = problemTicketTicket.FinishDate,
                            SlaCable = problemSlaLevel.SlaCable.ToString(),
                            SlaDevice = problemSlaLevel.SlaDevice.ToString(),
                            SlaLogic = problemSlaLevel.SlaLogic.ToString(),
                            SlaPrivate = problemSlaLevel.SlaPrivate.ToString()
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Subject.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords) ||
                              p.Detail.Contains(ctx.Keywords) ||
                              p.LastReason.Contains(ctx.Keywords) ||
                              p.Note.Contains(ctx.Keywords)
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.Trangthai))
            {
                query = from p in query
                        where p.Status == ctx.Trangthai
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.CreatedBy))
            {
                query = from p in query
                        where p.CreatedBy == ctx.CreatedBy
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.Priority))
            {
                query = from p in query
                        where p.Priority == ctx.Priority
                        select p;
            }

            if (!string.IsNullOrEmpty(ctx.Vungsuco))
            {
                query = from p in query
                        where p.TicketArea == ctx.Vungsuco
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

            return new PagedList<ProblemTicketExcelModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        private string SlaTime(Ticket_SuCo problemTicket, Core.Domain.Ticket.Ticket problemTicketTicket)
        {
            var hour = problemTicket.HourTimeMinus;
            var minute = problemTicket.MinuteTimeMinus;
            var second = problemTicket.SecondTimeMinus;
            var sla = "";

            if (problemTicketTicket.FinishDate.HasValue && problemTicketTicket.StartDate.HasValue)
            {
                var duration = TimeSpan.FromSeconds(problemTicketTicket.FinishDate.Value.ToUnixTime() - problemTicketTicket.StartDate.Value.ToUnixTime());
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

        private string MinusTime(Ticket_SuCo problemTicket)
        {
            var hour = problemTicket.HourTimeMinus;
            var minute = problemTicket.MinuteTimeMinus;
            var second = problemTicket.SecondTimeMinus;
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
        #endregion

        #region Utitlities

        #endregion
    }
}
