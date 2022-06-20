using System.Collections.Generic;
using VTQT.Core.Domain.Apps;

namespace VTQT.Core
{
    /// <summary>
    /// App context
    /// </summary>
    public interface IAppContext
    {
        AppSvcEntity CurrentApp { get; set; }

        AppSvcEntity MasterApp { get; set; }

        List<AppSvcEntity> AllApps { get; set; }
    }
}
