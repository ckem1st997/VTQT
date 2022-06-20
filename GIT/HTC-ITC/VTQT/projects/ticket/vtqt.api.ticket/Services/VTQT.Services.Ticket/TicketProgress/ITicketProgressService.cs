using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VTQT.Services.Ticket
{
    public partial interface ITicketProgressService
    {
        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}
