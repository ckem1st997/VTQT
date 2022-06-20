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
    public class ConfirmCRMxService : IConfirmCRMxService
    {
        #region Fields

        private readonly IRepository<ConfirmCRMx> _confirmCRMxRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;

        #endregion Fields

        #region Ctor

        public ConfirmCRMxService()
        {
            _confirmCRMxRepository = EngineContext.Current.Resolve<IRepository<ConfirmCRMx>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(ConfirmCRMx entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _confirmCRMxRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<ConfirmCRMx> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _confirmCRMxRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(ConfirmCRMx entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _confirmCRMxRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<ConfirmCRMx> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _confirmCRMxRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _confirmCRMxRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ConfirmCRMx> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _confirmCRMxRepository.GetByIdAsync(id);

            return result;
        }

        #endregion Methods

        #region List

        public IPagedList<ConfirmCRMx> Get(ConfirmCRMxSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _confirmCRMxRepository.Table
                        where i.crMxId == ctx.CrMxId
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

            return new PagedList<ConfirmCRMx>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ConfirmCRMx> GetByCRMxIdAsync(string crMxId)
        {
            if (string.IsNullOrEmpty(crMxId))
            {
                throw new ArgumentNullException(nameof(crMxId));
            }

            var results = _confirmCRMxRepository.Table
                .Where(x => x.crMxId == crMxId);

            return results.ToList();
        }

        public IList<ConfirmCRMx> GetByConfirmCRMxId(ConfirmCRMxSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.CrMxId))
                throw new ArgumentNullException(nameof(ctx.CrMxId));

            var query =
                from x in _confirmCRMxRepository.Table
                where x.crMxId == ctx.CrMxId
                select x;

            return query.ToList();
        }

        #endregion List
    }
}