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
    public class CRService : ICRService
    {
        #region Fields

        private IRepository<Core.Domain.Ticket.CR> _cRRepository;
        private IRepository<Core.Domain.Ticket.ApprovalCR> _approvalCRRepository;
        private IRepository<Core.Domain.Ticket.ConfirmCR> _confirmCRRepository;
        private IRepository<Core.Domain.Ticket.CR_HTC> _crHtcRepository;
        private IRepository<Core.Domain.Ticket.Comment> _commentRepository;
        private IRepository<Core.Domain.Ticket.InfrastructorFeeCR> _infrastructorFeeCRRepository;
        private IRepository<Core.Domain.Ticket.ApprovalCRMx> _approvalCRMxRepository;
        private IRepository<Core.Domain.Ticket.ConfirmCRMx> _confirmCRMxRepository;
        private IRepository<Core.Domain.Ticket.CRMx> _crMxRepository;
        private IRepository<Core.Domain.Ticket.CRPartner> _crPartnerRepository;
        #endregion Fields

        #region Ctor

        public CRService()
        {
            _crPartnerRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CRPartner>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crMxRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CRMx>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _confirmCRMxRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.ConfirmCRMx>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _approvalCRMxRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.ApprovalCRMx>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _cRRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _approvalCRRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.ApprovalCR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _confirmCRRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.ConfirmCR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _crHtcRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CR_HTC>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _commentRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Comment>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _infrastructorFeeCRRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.InfrastructorFeeCR>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(Core.Domain.Ticket.CR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _cRRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Core.Domain.Ticket.CR entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _cRRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            Task delApprovalCR = _approvalCRRepository.DeleteAsync(_approvalCRRepository.Table.Where(x => ids.Contains(x.CrId)).Select(x => x.Id));
            Task delFee = _infrastructorFeeCRRepository.DeleteAsync(_infrastructorFeeCRRepository.Table.Where(x => ids.Contains(x.CrId)).Select(x => x.Id));
            Task delComment = _commentRepository.DeleteAsync(_commentRepository.Table.Where(x => ids.Contains(x.CrId)|| ids.Contains(x.CrMxId)).Select(x => x.Id));
            Task delCRHTC = _crHtcRepository.DeleteAsync(_crHtcRepository.Table.Where(x => ids.Contains(x.CrId)).Select(x => x.Id));
            Task delconfirmCR = _confirmCRRepository.DeleteAsync(_confirmCRRepository.Table.Where(x => ids.Contains(x.crId)).Select(x => x.Id));
            Task delApprovalCRMx = _approvalCRMxRepository.DeleteAsync(_approvalCRMxRepository.Table.Where(x => ids.Contains(x.CrMxId)).Select(x => x.Id));
            Task delconfirmCRMx = _confirmCRMxRepository.DeleteAsync(_confirmCRMxRepository.Table.Where(x => ids.Contains(x.crMxId)).Select(x => x.Id));
            Task delCRMx = _crMxRepository.DeleteAsync(_crMxRepository.Table.Where(x => ids.Contains(x.CrId)).Select(x => x.Id));
            Task delCRPartner = _crPartnerRepository.DeleteAsync(_crPartnerRepository.Table.Where(x => ids.Contains(x.CrId)).Select(x => x.Id));

            await Task.WhenAll(new List<Task> { delApprovalCR, delComment, delFee, delCRHTC, delconfirmCR, delApprovalCRMx, delconfirmCRMx, delCRMx, delCRPartner});

            var result = await _cRRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Core.Domain.Ticket.CR> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _cRRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Core.Domain.Ticket.CR> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _cRRepository.Table
                .FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _cRRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await _cRRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();
        }

        public async Task<CR> GetById(IEnumerable<string> ids)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _cRRepository.GetByIdAsync(ids);

            return result;
        }

        #endregion Methods
    }
}