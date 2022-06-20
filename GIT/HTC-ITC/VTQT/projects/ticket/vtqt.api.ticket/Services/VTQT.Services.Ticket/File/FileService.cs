using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class FileService : IFileService
    {
        #region Fields
        private readonly IRepository<File> _fileRepository;
        #endregion

        #region Ctor
        public FileService()
        {
            _fileRepository = EngineContext.Current.Resolve<IRepository<File>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(File entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _fileRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<File> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _fileRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(File entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _fileRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<File> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _fileRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _fileRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<File> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _fileRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List
        public IList<File> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _fileRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<File> GetByCRIdAsync(string crId)
        {
            if (string.IsNullOrEmpty(crId))
            {
                throw new ArgumentNullException(nameof(crId));
            }

            var results = _fileRepository.Table
                .Where(x => x.CrId == crId);

            return results.ToList();
        }

        public IList<File> GetByFTTHIdAsync(string ftthId)
        {
            if (string.IsNullOrEmpty(ftthId))
            {
                throw new ArgumentNullException(nameof(ftthId));
            }

            var results = _fileRepository.Table
                .Where(x => x.FtthId == ftthId);

            return results.ToList();
        }

        #endregion

        #region Utilities

        #endregion  
    }
}