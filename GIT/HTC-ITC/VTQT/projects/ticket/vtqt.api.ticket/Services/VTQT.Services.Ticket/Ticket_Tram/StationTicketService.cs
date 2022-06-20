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
    public partial class StationTicketService : IStationTicketService
    {
        #region Fields
        private readonly IRepository<Ticket_Tram> _stationTicketRepository;
        private readonly IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;
        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<TicketArea> _ticketAreaRepository;
        private readonly IRepository<TicketProvince> _ticketProvinceRepository;
        private readonly IRepository<Station> _stationRepository;
        #endregion

        #region Ctor
        public StationTicketService()
        {
            _stationTicketRepository = EngineContext.Current.Resolve<IRepository<Ticket_Tram>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ticket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketAreaRepository = EngineContext.Current.Resolve<IRepository<TicketArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ticketProvinceRepository = EngineContext.Current.Resolve<IRepository<TicketProvince>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _stationRepository = EngineContext.Current.Resolve<IRepository<Station>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods               
        public async Task<int> InsertAsync(Ticket_Tram entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _stationTicketRepository.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync(Ticket_Tram entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _stationTicketRepository.UpdateAsync(entity);
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof (ids));
            }

            return await _stationTicketRepository.DeleteAsync(ids);
        }

        public async Task<Ticket_Tram> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var ticket = await _stationTicketRepository.GetByIdAsync(id);

            return ticket;
        }

        public async Task<Ticket_Tram> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var result = await _stationTicketRepository.Table
                .FirstOrDefaultAsync(x => x.TicketId == ticketId);

            return result;
        }
        #endregion

        #region List
        public IPagedList<StationTicketGridModel> Get(StationTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from s in _stationTicketRepository.Table
                        join t in _ticketRepository.Table on s.TicketId equals t.Id into ts                        
                        from st in ts.DefaultIfEmpty()
                        join tt in _statusRepository.Table on st.Status equals tt.Id into ttst
                        from tstt in ttst.DefaultIfEmpty()
                        where st.ProjectId == ctx.ProjectId
                        orderby tstt.Az
                        select new StationTicketGridModel
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
                            EngineStartTime = s.EngineStartTime,
                            EngineFinishTime = s.EngineFinishTime,
                            StationId = st.StationId
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
                        where p.StartDate >= ctx.StartDate
                        select p;
            }

            if (ctx.FinishDate.HasValue)
            {
                query = from p in query
                        where p.FinishDate <= ctx.FinishDate
                        select p;
            }

            return new PagedList<StationTicketGridModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IPagedList<StationTicketExcelModel> GetExcelData(StationTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from stationTicket in _stationTicketRepository.Table
                        join ticket in _ticketRepository.Table on stationTicket.TicketId equals ticket.Id into a
                        from stationTicketTicket in a.DefaultIfEmpty()
                        join status in _statusRepository.Table on stationTicketTicket.Status equals status.Id into b
                        from stationTicketStatus in b.DefaultIfEmpty()
                        join station in _stationRepository.Table on stationTicketTicket.StationId equals station.Id into c
                        from stationTicketStation in c.DefaultIfEmpty()
                        join ticketArea in _ticketAreaRepository.Table on stationTicket.TicketArea equals ticketArea.Id into d
                        from stationTicketArea in d.DefaultIfEmpty()
                        where stationTicketTicket.ProjectId == ctx.ProjectId
                        orderby stationTicketStatus.Az
                        select new StationTicketExcelModel
                        {
                            Code = stationTicketTicket.Code,
                            Subject = stationTicketTicket.Subject,
                            CreatedDate = stationTicketTicket.StartDate.Value.ToString("dd/MM/yyyy"),
                            StationCode = stationTicketStation.Code,
                            StationName = stationTicketStation.Name,
                            TicketAreaName = stationTicketArea.Name,
                            Detail = stationTicketTicket.Detail,
                            LastReason = stationTicketTicket.LastReason,
                            FirstReason = stationTicketTicket.FirstReason,
                            Note = stationTicketTicket.Note,
                            Status = stationTicketTicket.Status,
                            Assignee = stationTicketTicket.Assignee,
                            CreatedBy = stationTicketTicket.CreatedBy,
                            Priority = stationTicketTicket.Priority,
                            TicketArea = stationTicket.TicketArea,
                            StartDateTime = stationTicketTicket.StartDate,
                            FinishDateTime = stationTicketTicket.FinishDate,
                            TicketProvince = stationTicket.TicketProvince,
                            StartDate = stationTicketTicket.StartDate.HasValue ? stationTicketTicket.StartDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            FinishDate = stationTicketTicket.FinishDate.HasValue ? stationTicketTicket.FinishDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            DurationTime = (stationTicketTicket.FinishDate.HasValue && stationTicketTicket.StartDate.HasValue) ? 
                                TimeSpan.FromSeconds(stationTicketTicket.FinishDate.Value.ToUnixTime() - stationTicketTicket.StartDate.Value.ToUnixTime()).ToString(@"dd") + " ngày - " + TimeSpan.FromSeconds(stationTicketTicket.FinishDate.Value.ToUnixTime() - stationTicketTicket.StartDate.Value.ToUnixTime()).ToString(@"hh\:mm\:ss") : ""
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
                        where p.StartDateTime >= ctx.StartDate
                        select p;
            }

            if (ctx.FinishDate.HasValue)
            {
                query = from p in query
                        where p.FinishDateTime <= ctx.FinishDate
                        select p;
            }

            return new PagedList<StationTicketExcelModel>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion
    }
}
