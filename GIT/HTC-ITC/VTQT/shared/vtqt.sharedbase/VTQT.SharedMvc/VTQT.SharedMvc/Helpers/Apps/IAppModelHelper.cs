using System.Collections.Generic;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public interface IAppModelHelper
    {
        List<AppModel> GetAllApiTypes();

        List<AppModel> GetAllWebTypes();
    }
}
