using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.FbmCrm;

namespace VTQT.Services.Master
{
    public partial interface ICustomerService
    {
        IList<ApplicationUser> GetAll(bool showHidden = false);

        IPagedList<ApplicationUser> Get(CustomerSearchContext ctx);

        Task<ApplicationUser> GetByIdAsync(int id);

        IList<ApplicationUser> GetAvailable();

        Task<ApplicationUser> GetByCodeAsync(string code);
    }
}
