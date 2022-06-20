using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;

namespace VTQT.Services.Warehouse
{
    public partial interface IAccObjectService
    {
      
        IList<SelectListItem> GetMvcListItems(bool showHidden = false);

    }
}
