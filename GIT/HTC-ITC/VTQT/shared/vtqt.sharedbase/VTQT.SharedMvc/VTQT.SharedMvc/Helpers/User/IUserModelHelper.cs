using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public interface IUserModelHelper
    {
        List<UserModel> GetAll(bool showHidden = false);

        List<SelectListItem> GetMvcListItems(bool showHidden = false);
    }
}
