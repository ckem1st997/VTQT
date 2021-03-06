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
    public class CRPartnerService : ICRPartnerService
    {
        #region Fields

        private readonly IRepository<Status> _statusRepository;
        private readonly IRepository<CRCategory> _CRCategoryRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;
        private readonly IRepository<Core.Domain.Ticket.CRPartner> _crPartnerRepository;

        #endregion Fields

        #region Ctor

        public CRPartnerService()
        {
            _statusRepository = EngineContext.Current.Resolve<IRepository<Status>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _CRCategoryRepository = EngineContext.Current.Resolve<IRepository<CRCategory>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crRepository = EngineContext.Current.Resolve<IRepository<CR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crPartnerRepository = EngineContext.Current.Resolve<IRepository<CRPartner>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(CRPartner entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _crPartnerRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(CRPartner entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _crPartnerRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _crPartnerRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<CRPartner> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _crPartnerRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<CRPartner> GetByCRPartnerIdAsync(string crPartnerId)
        {
            if (string.IsNullOrEmpty(crPartnerId))
            {
                throw new ArgumentNullException(nameof(crPartnerId));
            }

            var result = await _crPartnerRepository.Table
                .FirstOrDefaultAsync(x => x.CrId == crPartnerId);

            return result;
        }

        #endregion Methods

        #region List

        public async Task<IPagedList<CRPartnerGridModel>> Get(CRPartnerSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from p in _crPartnerRepository.Table
                        join t in _crRepository.Table on p.CrId equals t.Id into pt
                        from tp in pt.DefaultIfEmpty()
                        join j in _CRCategoryRepository.Table on tp.Category equals j.Id into jn
                        from jt in jn.DefaultIfEmpty()
                        join k in _statusRepository.Table on tp.Status equals k.Id into kn
                        from kt in kn.DefaultIfEmpty()
                        where tp.ProjectId == ctx.ProjectId
                        select new CRPartnerGridModel
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
                            StartTimeAction = p.StartTimeAction,
                            RestoreTimeService = p.RestoreTimeService,
                            HourTimeMinus = p.HourTimeMinus ?? 0,
                            MinuteTimeMinus = p.MinuteTimeMinus ?? 0,
                            SecondTimeMinus = p.SecondTimeMinus ?? 0,
                            OverTimeRegister = p.OverTimeRegister,
                            Supervisor = p.Supervisor,
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

            return new PagedList<CRPartnerGridModel>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<CRPartner> GetByCRPartnerId(CRPartnerSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.CrId))
                throw new ArgumentNullException(nameof(ctx.CrId));

            var query =
                from x in _crPartnerRepository.Table
                where x.CrId == ctx.CrId
                select x;

            return query.ToList();
        }

        #endregion List
    }
}