using System.Collections.Generic;

namespace VTQT.Web.Framework.Settings
{
    public class AppDependingSettingData
    {
        public AppDependingSettingData()
        {
            OverrideSettingKeys = new List<string>();
        }

        public string ActiveAppScopeConfiguration { get; set; }
        public List<string> OverrideSettingKeys { get; set; }
        public string RootSettingClass { get; set; }
    }
}
