using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VTQT.Services.Ticket
{
    public interface IApprovalProgressService
    {
        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
