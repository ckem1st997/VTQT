using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class TechnologyService : ITechnologyService
    {
        #region Fields

        private IRepository<Core.Domain.Ticket.Technology> _technologyRepository;

        #endregion Fields

        #region Ctor

        public TechnologyService()
        {
            _technologyRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Technology>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> UpdateAsync(Core.Domain.Ticket.Technology entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _technologyRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _technologyRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Core.Domain.Ticket.Technology> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _technologyRepository.Table
                .FirstOrDefaultAsync(x => x.FtthId == id);

            return result;
        }

        public async Task<int> InsertAsync(Technology entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _technologyRepository.InsertAsync(entity);

            return result;
        }

        public async Task<Core.Domain.Ticket.Technology> GetByIdFtthAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _technologyRepository.GetByIdAsync(id);

            return result;
        }

        #endregion Methods
    }
}