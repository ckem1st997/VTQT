using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IOutWardService
    {
        Task InsertAsync(Outward entity, IList<OutwardDetail> details = null);

        Task UpdateAsync(Outward entity, IList<OutwardDetail> details = null, IList<string> deleteDetailIds = null);

        Task DeletesAsync(IEnumerable<string> ids);

        Task<Outward> GetByIdAsync(string id);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);
    }
}
