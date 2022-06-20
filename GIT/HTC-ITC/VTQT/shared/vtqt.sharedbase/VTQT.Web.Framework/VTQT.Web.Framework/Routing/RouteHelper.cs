using VTQT.Web.Framework.Helpers;

namespace VTQT.Web.Framework.Routing
{
    public static class RouteHelper
    {
        public static class AppAssemblyAreas
        {
            public static class AssetWeb
            {
                public static string Web = AppHelper.Apps.AssetWeb.Web.AssemblyName;
                public static string Admin = AppHelper.Apps.AssetWeb.Admin.AssemblyName;
            }

            public static class WarehouseWeb
            {
                public static string Web = AppHelper.Apps.WarehouseWeb.Web.AssemblyName;
                public static string Admin = AppHelper.Apps.WarehouseWeb.Admin.AssemblyName;
            }

            public static class MasterWeb
            {
                public static string Web = AppHelper.Apps.MasterWeb.Web.AssemblyName;
                public static string Admin = AppHelper.Apps.MasterWeb.Admin.AssemblyName;
            }
        }

        public static class DataTokens
        {
            public static string AppAssemblyAreaKey = "AppAssemblyArea";
        }
    }
}
