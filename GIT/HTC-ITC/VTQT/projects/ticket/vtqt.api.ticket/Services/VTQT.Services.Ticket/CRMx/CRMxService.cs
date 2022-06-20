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
    public class CRMxService : ICRMxService
    {
        #region Fields

        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<CRCategory> _CRCategoryRepository;
        private readonly IRepository<TicketArea> _crAreaRepository;
        private readonly IRepository<TicketProvince> _crProvinceRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;
        private readonly IRepository<Core.Domain.Ticket.CRMx> _crMxRepository;

        #endregion Fields

        #region Ctor

        public CRMxService()
        {
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _CRCategoryRepository = EngineContext.Current.Resolve<IRepository<CRCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crAreaRepository = EngineContext.Current.Resolve<IRepository<TicketArea>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crProvinceRepository = EngineContext.Current.Resolve<IRepository<TicketProvince>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crRepository = EngineContext.Current.Resolve<IRepository<CR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crMxRepository = EngineContext.Current.Resolve<IRepository<CRMx>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(CRMx entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _crMxRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(CRMx entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _crMxRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _crMxRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<CRMx> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _crMxRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<CRMx> GetByCRMxIdAsync(string crId)
        {
            if (string.IsNullOrEmpty(crId))
            {
                throw new ArgumentNullException(nameof(crId));
            }

            var result = await _crMxRepository.Table
                .FirstOrDefaultAsync(x => x.CrId == crId);

            return result;
        }

        #endregion Methods

        #region List

        public async Task<IPagedList<CRMxGridModel>> Get(CRMxSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _crMxRepository.Table
                        join t in _crRepository.Table on p.CrId equals t.Id into pt
                        from tp in pt.DefaultIfEmpty()
                        join t in _crAreaRepository.Table on p.CrArea equals t.Id into tn
                        from tt in tn.DefaultIfEmpty()
                        join u in _crProvinceRepository.Table on p.CrProvince equals u.Id into un
                        from ut in un.DefaultIfEmpty()
                        join j in _CRCategoryRepository.Table on tp.Category equals j.Id into jn
                        from jt in jn.DefaultIfEmpty()
                        join k in _statusRepository.Table on tp.Status equals k.Id into kn
                        from kt in kn.DefaultIfEmpty()
                        where tp.ProjectId == ctx.ProjectId
                        select new CRMxGridModel
                        {
                            Id = tp.Id,
                            CrId = tp.Id,
                            Code = tp.Code,
                            Name = tp.Name,
                            Category = jt.Name,
                            Status = kt.Name,
                            CategoryId = jt.Id,
                            CreatedBy = tp.CreatedBy,
                            CreatedDate = tp.CreatedDate,
                            ModifiedBy = tp.ModifiedBy,
                            ModifiedDate = tp.ModifiedDate,
                            Detail = tp.Detail,
                            Note = tp.Note,
                            Inactive = tp.Inactive,
                            ProjectId = tp.ProjectId,
                            StartDate = tp.StartDate,
                            FinishDate = tp.FinishDate,
                            ImplementationSteps = tp.ImplementationSteps,
                            CrReason = tp.CrReason,
                            FieldHandler = tp.FieldHandler,
                            InfluenceChannel = tp.InfluenceChannel,
                            CrArea = tt.Name,
                            CrProvince = ut.Name,
                            StartTimeAction = p.StartTimeAction,
                            RestoreTimeService = p.RestoreTimeService,
                            HourTimeMinus = p.HourTimeMinus ??0,
                            MinuteTimeMinus = p.MinuteTimeMinus??0,
                            SecondTimeMinus = p.SecondTimeMinus??0,
                            OverTimeRegister = p.OverTimeRegister,
                            Supervisor = p.Supervisor,
                            TicketAreaId = tt.Id,
                            StatusId = kt.Id,
                            Total = tp.TotalTime.ToString()
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Name.Contains(ctx.Keywords) ||
                              p.Code.Contains(ctx.Keywords) ||
                              p.CrId.Contains(ctx.Keywords)
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
            if (!string.IsNullOrEmpty(ctx.CategoryId))
            {
                query = from p in query
                        where p.CategoryId == ctx.CategoryId
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
            if (ctx.StartTimeAction.HasValue)
            {
                query = from p in query
                        where p.StartTimeAction >= ctx.StartTimeAction.Value
                        select p;
            }
            if (ctx.RestoreTimeService.HasValue)
            {
                query = from p in query
                        where p.RestoreTimeService <= ctx.RestoreTimeService.Value
                        select p;
            }

            query = from p in query
                    orderby p.Code
                    select p;

            return new PagedList<CRMxGridModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<CRMx> GetByCRMxId(CRMxSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.CrId))
                throw new ArgumentNullException(nameof(ctx.CrId));

            var query =
                from x in _crMxRepository.Table
                where x.CrId == ctx.CrId
                select x;

            return query.ToList();
        }

        #endregion List
    }
}