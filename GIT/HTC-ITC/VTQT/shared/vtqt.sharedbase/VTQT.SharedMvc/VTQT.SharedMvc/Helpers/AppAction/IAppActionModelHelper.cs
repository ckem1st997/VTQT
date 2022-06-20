using System.Collections.Generic;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public interface IAppActionModelHelper
    {
        List<AppActionModel> GetAppActions(string appId, bool showHidden = false);

        List<AppActionTreeModel> GetAppActionTree(string appId, int? expandLevel, bool showHidden = false);
    }
}
