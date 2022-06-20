using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.Warehouse
{
    public partial class AuditDetailSerialService : IAuditDetailSerialService
    {
        #region Fields
        private readonly IRepository<AuditDetailSerial> _auditDSRepository;
        #endregion

        #region Ctor
        public AuditDetailSerialService()
        {
            _auditDSRepository = EngineContext.Current.Resolve<IRepository<AuditDetailSerial>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }
        #endregion

        #region Methods
        public async Task<int> InsertAsync(AuditDetailSerial entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _auditDSRepository.InsertAsync(entity);

            return result;
        }

        public async Task<int> UpdateAsync(AuditDetailSerial entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _auditDSRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = await _auditDSRepository.DeleteAsync(ids);

            return result;
        }

        public async Task<AuditDetailSerial> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _auditDSRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<bool> ExistsAsync(string itemId)
        {
            return await _auditDSRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(itemId)
                && x.ItemId.Equals(itemId));
        }

        public IQueryable<AuditDetailSerial> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = from p in _auditDSRepository.Table where p.AuditDetailId.Equals(id.Trim()) select p;

            return result;
        }

        public async Task<int> DeletesAsync(IEnumerable<AuditDetailSerial> auditCouncils)
        {
            if (auditCouncils == null)
            {
                throw new ArgumentNullException(nameof(auditCouncils));
            }

            var result = await _auditDSRepository.DeleteAsync(auditCouncils);

            return result;
        }
        #endregion
    }
}
