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
    public partial class ChannelTicketService : IChannelTicketService
    {
        #region Fields
        private readonly IRepository<VTQT.Core.Domain.Ticket.Phenomenon> _phenomenonRepository;
        private readonly IRepository<Channel> _channelRepository;
        private readonly IRepository<Cable> _cableRepository;
        private readonly IRepository<ChannelTicket> _channelTicketRepository;
        #endregion

        #region Ctor
        public ChannelTicketService()
        {
            _phenomenonRepository = EngineContext.Current.Resolve<IRepository<Phenomenon>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelRepository = EngineContext.Current.Resolve<IRepository<Channel>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _cableRepository = EngineContext.Current.Resolve<IRepository<Cable>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelTicketRepository = EngineContext.Current.Resolve<IRepository<ChannelTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(ChannelTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _channelTicketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<ChannelTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _channelTicketRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(ChannelTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _channelTicketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<ChannelTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _channelTicketRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _channelTicketRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<ChannelTicket> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _channelTicketRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List

        public IPagedList<ChannelTicket> Get(ChannelTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _channelTicketRepository.Table                        
                        where i.TicketId == ctx.TicketId
                        select i;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.ChannelName.Contains(ctx.Keywords)
                        select p;
            }

            query =
                from p in query
                orderby p.ChannelName
                select p;

            return new PagedList<ChannelTicket>(query, ctx.PageIndex, ctx.PageSize);
        }

        public IList<ChannelTicket> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _channelTicketRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<ChannelTicket> GetByChannelTicketId(ChannelTicketSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.TicketId))
                throw new ArgumentNullException(nameof(ctx.TicketId));

            var query =
                from x in _channelTicketRepository.Table
                where x.TicketId == ctx.TicketId
                select x;

            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
