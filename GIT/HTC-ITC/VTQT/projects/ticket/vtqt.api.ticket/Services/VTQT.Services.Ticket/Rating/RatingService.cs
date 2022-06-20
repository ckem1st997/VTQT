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
    public class RatingService : IRatingService
    {
        #region Fields

        private IRepository<Core.Domain.Ticket.Rating> _ratingRepository;

        #endregion Fields

        #region Ctor

        public RatingService()
        {
            _ratingRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Ticket.Rating>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion Ctor

        #region Methods

        public IList<Rating> GetAll(bool showHidden = false)
        {
            var query = from p in _ratingRepository.Table select p;
            if (!showHidden)
            {
                query = from p in query where !p.Inactive select p;
            }
            query = from p in query orderby p.Name select p;
            return query.ToList();
        }

        public async Task<int> InsertAsync(Core.Domain.Ticket.Rating entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ratingRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Core.Domain.Ticket.Rating entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _ratingRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _ratingRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Core.Domain.Ticket.Rating> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _ratingRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<Core.Domain.Ticket.Rating> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _ratingRepository.Table
                .FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }

        public async Task<bool> ExistedAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _ratingRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<int> ActivatesAsync(IEnumerable<string> ids, bool active)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await _ratingRepository.Table
                .Where(x => ids.Contains(x.Id))
                .Set(x => x.Inactive, !active)
                .UpdateAsync();
        }

        #endregion Methods
    }
}