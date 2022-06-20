using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class FtthService : IFtthService
    {
        #region Fields

        private IRepository<Core.Domain.Ticket.Ftth> _ftthRepository;
        private IRepository<Core.Domain.Ticket.ParitcularFtth> _paritcularFtthRepository;
        private IRepository<Core.Domain.Ticket.WideFtth> _wideFtthRepository;
        #endregion Fields

        #region Ctor

        public FtthService()
        {
            _wideFtthRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.WideFtth>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _ftthRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Ftth>>(DataConnectionHelper.ConnectionStringNames.Ticket);
            _paritcularFtthRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.ParitcularFtth>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public async Task<int> InsertAsync(Core.Domain.Ticket.Ftth entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ftthRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Core.Domain.Ticket.Ftth entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ftthRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            Task delParitcularFtth = _paritcularFtthRepository.DeleteAsync(_paritcularFtthRepository.Table.Where(x => ids.Contains(x.FtthId)).Select(x => x.Id));
            Task delWideFtth = _wideFtthRepository.DeleteAsync(_wideFtthRepository.Table.Where(x => ids.Contains(x.FtthId)).Select(x => x.Id));

            await Task.WhenAll(new List<Task> { delParitcularFtth });

            var result = await _ftthRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Core.Domain.Ticket.Ftth> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ftthRepository.GetByIdAsync(id);

            return result;
        }

        public Core.Domain.Ticket.Ftth GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result =  _ftthRepository.GetById(id);

            return result;
        }

        public async Task<Core.Domain.Ticket.Ftth> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _ftthRepository.Table
                .FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _ftthRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        #endregion Methods
    }
}