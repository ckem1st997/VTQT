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
    public class ConfirmCRService : IConfirmCRService
    {
        #region Fields

        private readonly IRepository<ConfirmCR> _confirmCRRepository;
        private readonly IRepository<Core.Domain.Ticket.CR> _crRepository;

        #endregion Fields

        #region Ctor

        public ConfirmCRService()
        {
            _confirmCRRepository = EngineContext.Current.Resolve<IRepository<ConfirmCR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(ConfirmCR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _confirmCRRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<ConfirmCR> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _confirmCRRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(ConfirmCR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _confirmCRRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<ConfirmCR> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _confirmCRRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _confirmCRRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ConfirmCR> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _confirmCRRepository.GetByIdAsync(id);

            return result;
        }

        #endregion Methods

        #region List

        public IPagedList<ConfirmCR> Get(ConfirmCRSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _confirmCRRepository.Table
                        where i.crId == ctx.CrId
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

            return new PagedList<ConfirmCR>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ConfirmCR> GetByCRIdAsync(string crId)
        {
            if (string.IsNullOrEmpty(crId))
            {
                throw new ArgumentNullException(nameof(crId));
            }

            var results = _confirmCRRepository.Table
                .Where(x => x.crId == crId);

            return results.ToList();
        }

        public IList<ConfirmCR> GetByConfirmCRId(ConfirmCRSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.CrId))
                throw new ArgumentNullException(nameof(ctx.CrId));

            var query =
                from x in _confirmCRRepository.Table
                where x.crId == ctx.CrId
                select x;

            return query.ToList();
        }

        #endregion List
    }
}