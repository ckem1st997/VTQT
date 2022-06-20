using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class CsReportService : ICsReportService
    {
        #region Fields

        private IRepository<Core.Domain.Ticket.CsReport> _csReportRepository;

        #endregion Fields

        #region Ctor

        public CsReportService()
        {
            _csReportRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.CsReport>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(CsReport entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _csReportRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Core.Domain.Ticket.CsReport entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _csReportRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _csReportRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Core.Domain.Ticket.CsReport> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _csReportRepository.GetByIdAsync(id);

            return result;
        }

        #endregion Methods
    }
}