using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class TicketService : ITicketService
    {
        #region Fields
        private IRepository<Core.Domain.Ticket.Ticket> _ticketRepository;
        private IRepository<NetworkLinkTicket> _networkLinkTicketRepository;
        private IRepository<ChannelTicket> _channelTicketRepository;
        private IRepository<ApprovalTicket> _approvalTicketRepository;
        private IRepository<InfrastructorFee> _infrastructorFeeRepository;
        private IRepository<File> _fileRepository;
        private IRepository<Comment> _commentRepository;
        private IRepository<Ticket_SuCo> _problemTicketRepository;
        private IRepository<DeviceTicket> _deviceTicket;
        private IRepository<Ticket_Tram> _stationTicketRepository;
        private IRepository<Ticket_VanDe> _troubleTicketRepository;
        #endregion

        #region Ctor
        public TicketService()
        {
            _ticketRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ticket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _networkLinkTicketRepository = EngineContext.Current.Resolve<IRepository<NetworkLinkTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _channelTicketRepository = EngineContext.Current.Resolve<IRepository<ChannelTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _approvalTicketRepository = EngineContext.Current.Resolve<IRepository<ApprovalTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _infrastructorFeeRepository = EngineContext.Current.Resolve<IRepository<InfrastructorFee>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _fileRepository = EngineContext.Current.Resolve<IRepository<File>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _commentRepository = EngineContext.Current.Resolve<IRepository<Comment>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _problemTicketRepository = EngineContext.Current.Resolve<IRepository<Ticket_SuCo>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _deviceTicket = EngineContext.Current.Resolve<IRepository<DeviceTicket>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _stationTicketRepository = EngineContext.Current.Resolve<IRepository<Ticket_Tram>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _troubleTicketRepository = EngineContext.Current.Resolve<IRepository<Ticket_VanDe>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(Core.Domain.Ticket.Ticket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Core.Domain.Ticket.Ticket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ticketRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            Task delNetworkLinkTicket = _networkLinkTicketRepository.DeleteAsync(_networkLinkTicketRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delChannelTicket = _channelTicketRepository.DeleteAsync(_channelTicketRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delApprovalTicket = _approvalTicketRepository.DeleteAsync(_approvalTicketRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delFee = _infrastructorFeeRepository.DeleteAsync(_infrastructorFeeRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delFile = _fileRepository.DeleteAsync(_fileRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delComment = _commentRepository.DeleteAsync(_commentRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delProblemTicket = _problemTicketRepository.DeleteAsync(_problemTicketRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delDeviceTicket = _deviceTicket.DeleteAsync(_deviceTicket.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delStationTicket = _stationTicketRepository.DeleteAsync(_stationTicketRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));
            Task delTroubleTicket = _troubleTicketRepository.DeleteAsync(_stationTicketRepository.Table.Where(x => ids.Contains(x.TicketId)).Select(x => x.Id));

            await Task.WhenAll(new List<Task> { delApprovalTicket, delChannelTicket, delComment, delDeviceTicket, delFee, delFile, delNetworkLinkTicket, delProblemTicket, delStationTicket, delTroubleTicket });

            var result = await _ticketRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Core.Domain.Ticket.Ticket> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ticketRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Core.Domain.Ticket.Ticket> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _ticketRepository.Table
                .FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _ticketRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await _ticketRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
