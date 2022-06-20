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
    public class ApprovalCRService : IApprovalCRService
    {
        #region Fields

        private readonly IRepository<ApprovalCR> _approvalCRRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;

        #endregion Fields

        #region Ctor

        public ApprovalCRService()
        {
            _approvalCRRepository = EngineContext.Current.Resolve<IRepository<ApprovalCR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(ApprovalCR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _approvalCRRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<ApprovalCR> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _approvalCRRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(ApprovalCR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _approvalCRRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<ApprovalCR> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _approvalCRRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _approvalCRRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ApprovalCR> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _approvalCRRepository.GetByIdAsync(id);

            return result;
        }

        #endregion Methods

        #region List

        public IPagedList<ApprovalCR> Get(ApprovalCRSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _approvalCRRepository.Table
                        where i.CrId == ctx.CrId
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

            return new PagedList<ApprovalCR>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ApprovalCR> GetByCRIdAsync(string crId)
        {
            if (string.IsNullOrEmpty(crId))
            {
                throw new ArgumentNullException(nameof(crId));
            }

            var results = _approvalCRRepository.Table
                .Where(x => x.CrId == crId);

            return results.ToList();
        }

        public IList<ApprovalCR> GetByApprovalCRId(ApprovalCRSearchContext ctx)
        {
                if (string.IsNullOrWhiteSpace(ctx.CrId))
                    throw new ArgumentNullException(nameof(ctx.CrId));

                var query =
                    from x in _approvalCRRepository.Table
                    where x.CrId == ctx.CrId
                    select x;

                return query.ToList();
        }

        #endregion List
    }
}