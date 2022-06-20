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
    public class ApprovalCRMxService : IApprovalCRMxService
    {
        #region Fields

        private readonly IRepository<ApprovalCRMx> _approvalCRMxRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;

        #endregion Fields

        #region Ctor

        public ApprovalCRMxService()
        {
            _approvalCRMxRepository = EngineContext.Current.Resolve<IRepository<ApprovalCRMx>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(ApprovalCRMx entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _approvalCRMxRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<ApprovalCRMx> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _approvalCRMxRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(ApprovalCRMx entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _approvalCRMxRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<ApprovalCRMx> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _approvalCRMxRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _approvalCRMxRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ApprovalCRMx> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _approvalCRMxRepository.GetByIdAsync(id);

            return result;
        }

        #endregion Methods

        #region List

        public IPagedList<ApprovalCRMx> Get(ApprovalCRMxSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _approvalCRMxRepository.Table
                        where i.CrMxId == ctx.CrMxId
                        select i;

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

            return new PagedList<ApprovalCRMx>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ApprovalCRMx> GetByCRMxIdAsync(string crMxId)
        {
            if (string.IsNullOrEmpty(crMxId))
            {
                throw new ArgumentNullException(nameof(crMxId));
            }

            var results = _approvalCRMxRepository.Table
                .Where(x => x.CrMxId == crMxId);

            return results.ToList();
        }

        public IList<ApprovalCRMx> GetByApprovalCRMxId(ApprovalCRMxSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.CrMxId))
                throw new ArgumentNullException(nameof(ctx.CrMxId));

            var query =
                from x in _approvalCRMxRepository.Table
                where x.CrMxId == ctx.CrMxId
                select x;

            return query.ToList();
        }

        #endregion List
    }
}