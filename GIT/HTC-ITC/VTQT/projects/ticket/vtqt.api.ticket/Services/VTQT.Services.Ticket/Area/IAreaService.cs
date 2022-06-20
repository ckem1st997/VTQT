using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VTQT.Services.Ticket
{
    public interface IAreaService
    {
        IList<SelectListItem> GetMvcListItems(bool showHidden);
    }
}