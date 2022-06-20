using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Ticket
{
    public class AssignmentService : IAssignmentService
    {
        #region Fields

        private readonly IRepository<Assignment> _assignmentRepository;

        #endregion

        #region Ctor

        public AssignmentService()
        {
            _assignmentRepository =
                EngineContext.Current.Resolve<IRepository<Assignment>>(
                    DataConnectionHelper.ConnectionStringNames.Ticket);
        }

        #endregion


        #region Methods
        public async Task<int> InsertAsync(Assignment entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _assignmentRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(Assignment entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _assignmentRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _assignmentRepository.DeleteAsync(ids);

            return result;
        }

        #endregion
    }
}
