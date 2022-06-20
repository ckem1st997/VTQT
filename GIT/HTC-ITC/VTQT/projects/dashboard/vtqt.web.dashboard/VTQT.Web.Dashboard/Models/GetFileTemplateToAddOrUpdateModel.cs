using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VTQT.Web.Dashboard.Models
{
    public class GetFileTemplateToAddOrUpdateModel
    {
        public string id { get; set; }
        public string keySearch { get; set; }
        public int numberPage { get; set; }
        public IEnumerable<SelectListItem> listItem { get; set; }
    }
}
