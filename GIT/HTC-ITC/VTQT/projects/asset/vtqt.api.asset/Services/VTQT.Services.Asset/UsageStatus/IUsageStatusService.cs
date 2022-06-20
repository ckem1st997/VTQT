using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IUsageStatusService
    {
        IList<SelectListItem> GetMvcListItem();

        Task<UsageStatus> GetByIdAsync(string id);
    }
}
