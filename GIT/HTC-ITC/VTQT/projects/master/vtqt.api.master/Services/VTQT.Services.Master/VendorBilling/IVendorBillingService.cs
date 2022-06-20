using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.FbmContract;

namespace VTQT.Services.Master
{
    public partial interface IVendorBillingService
    {
        IPagedList<Contractor> Get(VendorBillingSearchContext ctx);

        Task<Contractor> GetByIdAsync(int id);

        IList<Contractor> GetAvailable();
    }
}
