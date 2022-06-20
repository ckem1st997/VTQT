using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public partial class CommentService : ICommentService
    {
        #region Fields
        private readonly IRepository<Comment> _commentRepository;
        #endregion

        #region Ctor
        public CommentService()
        {
            _commentRepository = EngineContext.Current.Resolve<IRepository<Comment>>(DataConnectionHelper.ConnectionStringNames.Ticket);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(Comment entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _commentRepository.InsertAsync(entity);

            return result;
        }

        public async Task<long> InsertsAsync(IEnumerable<Comment> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _commentRepository.InsertAsync(entities);

            return result;
        }

        public async Task<int> UpdateAsync(Comment entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _commentRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> UpdatesAsync(IEnumerable<Comment> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var result = await _commentRepository.UpdateAsync(entities);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _commentRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<Comment> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _commentRepository.GetByIdAsync(id);

            return result;
        }
        #endregion

        #region List
        public IList<Comment> GetByTicketIdAsync(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                throw new ArgumentNullException(nameof(ticketId));
            }

            var results = _commentRepository.Table
                .Where(x => x.TicketId == ticketId);

            return results.ToList();
        }

        public IList<Comment> GetByCRIdAsync(string crId)
        {
            if (string.IsNullOrEmpty(crId))
            {
                throw new ArgumentNullException(nameof(crId));
            }

            var results = _commentRepository.Table
                .Where(x => x.CrId == crId);

            return results.ToList();
        }

        public IList<Comment> GetByCRMxIdAsync(string crMxId)
        {
            if (string.IsNullOrEmpty(crMxId))
            {
                throw new ArgumentNullException(nameof(crMxId));
            }

            var results = _commentRepository.Table
                .Where(x => x.CrMxId == crMxId);

            return results.ToList();
        }

        public IList<Comment> GetByCRPartnerIdAsync(string crPartnerId)
        {
            if (string.IsNullOrEmpty(crPartnerId))
            {
                throw new ArgumentNullException(nameof(crPartnerId));
            }

            var results = _commentRepository.Table
                .Where(x => x.CrPartnerId == crPartnerId);

            return results.ToList();
        }

        public IList<Comment> GetByFTTHIdAsync(string ftthId)
        {
            if (string.IsNullOrEmpty(ftthId))
            {
                throw new ArgumentNullException(nameof(ftthId));
            }

            var results = _commentRepository.Table
                .Where(x => x.FtthId == ftthId);

            return results.ToList();
        }

        public IList<Comment> GetByWideFtthIdAsync(string wideFtthId)
        {
            if (string.IsNullOrEmpty(wideFtthId))
            {
                throw new ArgumentNullException(nameof(wideFtthId));
            }

            var results = _commentRepository.Table
                .Where(x => x.WideFtthId == wideFtthId);

            return results.ToList();
        }
        #endregion

        #region Utilities

        #endregion
    }
}
