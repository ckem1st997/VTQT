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
    public partial class DeviceTicketService : IDeviceTicketService
    {
        #region Fields
        private readonly IRepository<VTQT.Core.Domain.Ticket.Phenomenon> _phenomenonRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DeviceTicket> _deviceTicketRepository;
        #endregion

        #region Ctor
        public DeviceTicketService()
        {
            _phenomenonRepository = EngineContext.Current.Resolve<IRepository<Phenomenon>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceRepository = EngineContext.Current.Resolve<IRepository<Device>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceTicketRepository = EngineContext.Current.Resolve<IRepository<DeviceTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(DeviceTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _deviceTicketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<DeviceTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _deviceTicketRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(DeviceTicket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _deviceTicketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<DeviceTicket> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _deviceTicketRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _deviceTicketRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<DeviceTicket> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _deviceTicketRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List
        public IPagedList<DeviceTicket> Get(DeviceTicketSearchContext ctx)
        {
            ctx.Keywords = ctx.Keywords?.Trim();

            var query = from i in _deviceTicketRepository.Table
                        where i.TicketId == ctx.TicketId
                        select i;

            if (ctx.Keywords.HasValue())
            {
                query = from p in query
                        where p.DeviceName.Contains(ctx.Keywords)
                        select p;
            }

            query =
                from p in query
                orderby p.DeviceName
                select p;

            return new PagedList<DeviceTicket>(query, ctx.PageIndex, ctx.PageSize);
        }
        public IList<DeviceTicket> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _deviceTicketRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<DeviceTicket> GetByDeviceTicketId(DeviceTicketSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.TicketId))
                throw new ArgumentNullException(nameof(ctx.TicketId));

            var query =
                from x in _deviceTicketRepository.Table
                where x.TicketId == ctx.TicketId
                select x;

            return query.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
