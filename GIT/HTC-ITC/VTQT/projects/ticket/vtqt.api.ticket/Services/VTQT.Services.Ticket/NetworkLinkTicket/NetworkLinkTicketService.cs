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
    public partial class NetworkLinkTicketService : INetworkLinkTicketService
    {
        #region Fields

        private readonly IRepository<NetworkLink> _networkLinkRepository;
        private readonly IRepository<NetworkLinkTicket> _networkLinkTicketRepository;

        #endregion

        #region Ctor

        public NetworkLinkTicketService()
        {
            _networkLinkRepository =
                EngineContext.Current.Resolve<IRepository<NetworkLink>>(DataConnectionHelper.ConnectionStringNames
                    .Ticket);
            _networkLinkTicketRepository =
                EngineContext.Current.Resolve<IRepository<NetworkLinkTicket>>(DataConnectionHelper.ConnectionStringNames
                    .Ticket);
        }

        #endregion

        #region Methods

        public async Task<int> InsertAsync(NetworkLinkTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.StartDate = entity.StartDate.ToUniversalTime();
            // entity.NetworkLinkName =entity.StartDate.ToString();
            var result = await _networkLinkTicketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<NetworkLinkTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _networkLinkTicketRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(NetworkLinkTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _networkLinkTicketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<NetworkLinkTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _networkLinkTicketRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _networkLinkTicketRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<NetworkLinkTicket> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _networkLinkTicketRepository.GetByIdAsync(id);

            return result;
        }

        #endregion

        #region List

        public IPagedList<NetworkLinkTicket> Get(NetworkLinkTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _networkLinkTicketRepository.Table
                where i.TicketId == ctx.TicketId
                select i;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                    where p.NetworkLinkName.Contains(ctx.Keywords)
                    select p;
            }

            query =
                from p in query
                orderby p.NetworkLinkName
                select p;

            return new PagedList<NetworkLinkTicket>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<NetworkLinkTicket> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _networkLinkTicketRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<NetworkLinkTicket> GetByNetworkLinkTicketId(NetworkLinkTicketSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.TicketId))
                throw new ArgumentNullException(nameof(ctx.TicketId));

            var query =
                from x in _networkLinkTicketRepository.Table
                where x.TicketId == ctx.TicketId
                select x;

            return query.ToList();
        }

        #endregion

        #region Utilities

        #endregion
    }
}