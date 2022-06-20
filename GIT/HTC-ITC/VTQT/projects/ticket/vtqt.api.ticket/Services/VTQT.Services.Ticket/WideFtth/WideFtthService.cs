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
    public class WideFtthService : IWideFtthService
    {
        #region Fields

        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<TicketReason> _TicketReasonRepository;
        private readonly IRepository<Phenomenon> _phenomenonRepository;
        private readonly IRepository<Core.Domain.Ticket.Ftth> _ftthRepository;
        private readonly IRepository<Core.Domain.Ticket.WideFtth> _wideFtthRepository;

        #endregion Fields

        #region Ctor

        public WideFtthService()
        {
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _TicketReasonRepository = EngineContext.Current.Resolve<IRepository<TicketReason>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _phenomenonRepository = EngineContext.Current.Resolve<IRepository<Phenomenon>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ftthRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ftth>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _wideFtthRepository = EngineContext.Current.Resolve<IRepository<WideFtth>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(WideFtth entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _wideFtthRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(WideFtth entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _wideFtthRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _wideFtthRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<WideFtth> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _wideFtthRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<WideFtth> GetByWideFtthIdAsync(string ftthId)
        {
            if (string.IsNullOrEmpty(ftthId))
            {
                throw new ArgumentNullException(nameof(ftthId));
            }

            var result = await _wideFtthRepository.Table
                .FirstOrDefaultAsync(x => x.FtthId == ftthId);

            return result;
        }

        #endregion Methods

        #region List

        public async Task<IPagedList<WideFtthGridModel>> Get(WideFtthSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _wideFtthRepository.Table
                        join t in _TicketReasonRepository.Table on p.ReasonId equals t.Id into pt
                        from tp in pt.DefaultIfEmpty()
                        join t in _phenomenonRepository.Table on p.PhenomenaId equals t.Id into tn
                        from tt in tn.DefaultIfEmpty()
                        join u in _ftthRepository.Table on p.FtthId equals u.Id into un
                        from ut in un.DefaultIfEmpty()
                        join k in _statusRepository.Table on p.StatusId equals k.Id into kn
                        from kt in kn.DefaultIfEmpty()
                        where ut.ProjectId == ctx.ProjectId
                        select new WideFtthGridModel
                        {
                            Id = ut.Id,
                            FtthId = ut.Id,
                            Code = ut.Code,
                            Subject = ut.Subject,
                            CreatedBy = ut.CreatedBy,
                            CreatedDate = ut.CreatedDate,
                            ModifiedBy = ut.ModifiedBy,
                            ModifiedDate = ut.ModifiedDate,
                            ProjectId = ut.ProjectId,
                            StartDate = ut.StartDate,
                            FinishDate = ut.FinishDate,
                            CID = ut.CID,
                            ProjectCode = ut.ProjectCode,
                            ComplaintContent = ut.ComplaintContent,
                            SlaStartTime = ut.SlaStartTime,
                            ReasonId = tp.Description,
                            PhenomenaId = tt.Name,
                            DetailReason = p.DetailReason,
                            Treatment = p.Treatment,
                            StatusId = kt.Name,
                            Status = kt.Id,
                            Reason = tp.Id,
                            Phenomena = tt.Id,
                            Total = ut.TotalTime.ToString()
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Subject.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords) ||
                              p.FtthId.Contains(ctx.Keywords)
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
            if (!string.IsNullOrEmpty(ctx.Trangthai))
            {
                query = from l in query
                        where l.Status == ctx.Trangthai
                        select l;
            }
            if (!string.IsNullOrEmpty(ctx.Reason))
            {
                query = from l in query
                        where l.Reason == ctx.Reason
                        select l;
            }
            if (!string.IsNullOrEmpty(ctx.Phenomena))
            {
                query = from l in query
                        where l.Phenomena == ctx.Phenomena
                        select l;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<WideFtthGridModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<WideFtth> GetByWideFtthId(WideFtthSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.FtthId))
                throw new ArgumentNullException(nameof(ctx.FtthId));

            var query =
                from x in _wideFtthRepository.Table
                where x.FtthId == ctx.FtthId
                select x;

            return query.ToList();
        }

        #endregion List
    }
}