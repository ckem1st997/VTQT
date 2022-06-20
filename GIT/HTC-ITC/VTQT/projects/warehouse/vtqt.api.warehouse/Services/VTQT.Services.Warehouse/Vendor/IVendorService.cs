using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IVendorService
    {
        Task<int> InsertAsync(Vendor entity);

        Task<long> InsertVendorAsync(IEnumerable<Vendor> entities);

        Task<int> UpdateAsync(Vendor entity);

        Task<int> DeletesAsync(IEnumerable<string> ids);

        IList<Vendor> GetAll(bool showHidden = false);

        IPagedList<Vendor> Get(VendorSearchContext ctx);

        Task<Vendor> GetByIdAsync(string id);

        Task<int> ActivatesAsync(IEnumerable<string> ids, bool active);

        Task<bool> ExistsAsync(string code);

        Task<bool> ExistsAsync(string oldCode, string newCode);

        Task<IEnumerable<string>> ExistCodesAsync(IEnumerable<string> codes);
    }
}
