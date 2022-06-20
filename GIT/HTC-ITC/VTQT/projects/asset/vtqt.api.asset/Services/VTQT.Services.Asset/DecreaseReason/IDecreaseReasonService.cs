using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;

namespace VTQT.Services.Asset
{
    public partial interface IDecreaseReasonService
    {
        IList<SelectListItem> GetMvcListItem();

        Task<DecreaseReason> GetByIdAsync(string id);
    }
}
