using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class ApprovalTicketService : IApprovalTicketService
    {
        #region Fields
        private readonly IRepository<ApprovalTicket> _approvalTicketRepository;
        private readonly IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;
        private readonly IRepository<ApprovalProgress> _approvalProgressRepository;
        #endregion

        #region Ctor
        public ApprovalTicketService()
        {
            _approvalTicketRepository = EngineContext.Current.Resolve<IRepository<ApprovalTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _approvalProgressRepository = EngineContext.Current.Resolve<IRepository<ApprovalProgress>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(ApprovalTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _approvalTicketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<ApprovalTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _approvalTicketRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(ApprovalTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _approvalTicketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<ApprovalTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _approvalTicketRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _approvalTicketRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ApprovalTicket> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _approvalTicketRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List

        public IPagedList<ApprovalTicket> Get(ApprovalTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _approvalTicketRepository.Table
                        join t in _approvalProgressRepository.Table on i.Progress equals t.Id into pt
                        from tp in pt.DefaultIfEmpty()
                        where i.TicketId == ctx.TicketId
                        select new ApprovalTicket
                        {
                            Id = i.Id,
                            Approver = i.Approver,
                            Confirm = i.Confirm,
                            Progress = i.Progress,
                            ReasonDetail = i.ReasonDetail,
                            Ticket = i.Ticket,
                            TicketId = i.TicketId
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.ReasonDetail.Contains(ctx.Keywords) ||
                              p.Approver.Contains(ctx.Keywords)
                        select p;
            }

            query =
                from p in query
                orderby p.ReasonDetail
                select p;

            return new PagedList<ApprovalTicket>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ApprovalTicket> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _approvalTicketRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<ApprovalTicket> GetByApprovalTicketId(ApprovalTicketSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.TicketId))
                throw new ArgumentNullException(nameof(ctx.TicketId));

            var query =
                from x in _approvalTicketRepository.Table
                where x.TicketId == ctx.TicketId
                select x;

            return query.ToList();
        }

        public IPagedList<ApprovalTicket> GetDetail(ApprovalTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _approvalTicketRepository.Table
                        join t in _approvalProgressRepository.Table on i.Progress equals t.Id into pt
                        from tp in pt.DefaultIfEmpty()
                        where i.TicketId == ctx.TicketId
                        select new ApprovalTicket
                        {
                            Id = i.Id,
                            Approver = i.Approver,
                            Confirm = i.Confirm,
                            Progress = $"[{tp.Code}] {tp.Name}",
                            ReasonDetail = i.ReasonDetail,
                            Ticket = i.Ticket,
                            TicketId = i.TicketId
                        };

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.ReasonDetail.Contains(ctx.Keywords) ||
                              p.Approver.Contains(ctx.Keywords)
                        select p;
            }

            query =
                from p in query
                orderby p.ReasonDetail
                select p;

            return new PagedList<ApprovalTicket>(query, ctx.PageIndex, ctx.PageSize);
        }
        #endregion

        #region Utilities

        #endregion
    }
}
