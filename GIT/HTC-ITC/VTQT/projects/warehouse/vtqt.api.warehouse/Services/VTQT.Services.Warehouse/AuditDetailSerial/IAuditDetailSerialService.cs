using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface  IAuditDetailSerialService
    {
        Task<int> InsertAsync(AuditDetailSerial entity);

        Task<int> UpdateAsync(AuditDetailSerial entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        Task<int> DeletesAsync(IEnumerable<AuditDetailSerial> auditDetailSerials);

        Task<AuditDetailSerial> GetByIdAsync(string id);

        IQueryable<AuditDetailSerial> GetListById(string id);

        Task<bool> ExistsAsync(string itemId);
    }
}
