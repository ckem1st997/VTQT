using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IInwardService
    {
        Task InsertAsync(Inward entity, IList<InwardDetail> details = null);

        Task UpdateAsync(Inward entity, IList<InwardDetail> details = null, IList<string> deleteDetailIds = null);

        Task DeletesAsync(IEnumerable<string> ids);

        Task<Inward> GetByIdAsync(string id);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        bool CheckItemExistAsync(string itemId, string warehouseId);
    }
}
