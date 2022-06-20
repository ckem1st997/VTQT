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
    public partial class InfrastructorFeeService : IInfrastructorFeeService
    {
        #region Fields
        private readonly IRepository<InfrastructorFee> _infrastructorFeeRepository;
        #endregion

        #region Ctor
        public InfrastructorFeeService()
        {
            _infrastructorFeeRepository = EngineContext.Current.Resolve<IRepository<InfrastructorFee>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(InfrastructorFee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _infrastructorFeeRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<InfrastructorFee> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _infrastructorFeeRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(InfrastructorFee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _infrastructorFeeRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<InfrastructorFee> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _infrastructorFeeRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _infrastructorFeeRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<InfrastructorFee> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _infrastructorFeeRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List

        public IPagedList<InfrastructorFee> Get(InfrastructorFeeSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _infrastructorFeeRepository.Table
                        where i.TicketId == ctx.TicketId
                        select i;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.Name.Contains(ctx.Keywords)
                        select p;
            }

            query =
                from p in query
                orderby p.Name
                select p;

            return new PagedList<InfrastructorFee>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<InfrastructorFee> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _infrastructorFeeRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<InfrastructorFee> GetByInfrastructorFeeId(InfrastructorFeeSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.TicketId))
                throw new ArgumentNullException(nameof(ctx.TicketId));

            var query =
                from x in _infrastructorFeeRepository.Table
                where x.TicketId == ctx.TicketId
                select x;

            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
