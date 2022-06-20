using System;
using System.Collections.Generic;

namespace VTQT.Core
{
    public class AppHelperBase
    {
        #region Apps

        public static class Apps
        {
            public static readonly List<string> AllTypes = new List<string>
            {
                NotifyApi.AppType,
                FileApi.AppType,
                DashboardApi.AppType,
                DashboardWeb.AppType,
                TicketApi.AppType,
                TicketWeb.AppType,
                AssetApi.AppType,
                AssetWeb.AppType,
                WarehouseApi.AppType,
                WarehouseWeb.AppType,
                MasterApi.AppType,
                MasterWeb.AppType,
                Other.AppType,
            };
            public static readonly List<string> ApiTypes = new List<string>
            {
                NotifyApi.AppType,
                FileApi.AppType,
                DashboardApi.AppType,
                TicketApi.AppType,
                AssetApi.AppType,
                WarehouseApi.AppType,
                MasterApi.AppType,
            };
            public static readonly List<string> WebTypes = new List<string>
            {
                DashboardWeb.AppType,
                TicketWeb.AppType,
                AssetWeb.AppType,
                WarehouseWeb.AppType,
                MasterWeb.AppType,
            };

            public static readonly List<string> WebAssemblyAreas = new List<string>
            {
                DashboardWeb.Web.AssemblyName,
                DashboardWeb.Admin.AssemblyName,

                TicketWeb.Web.AssemblyName,
                TicketWeb.Admin.AssemblyName,

                AssetWeb.Web.AssemblyName,
                AssetWeb.Admin.AssemblyName,

                WarehouseWeb.Web.AssemblyName,
                WarehouseWeb.Admin.AssemblyName,

                MasterWeb.Web.AssemblyName,
                MasterWeb.Admin.AssemblyName,
            };

            public static class NotifyApi
            {
                public static string AppType = "VTQT.Api.Notify";
            }

            public static class FileApi
            {
                public static string AppType = "VTQT.Api.File";
            }

            public static class DashboardApi
            {
                public static string AppType = "VTQT.Api.Dashboard";
            }

            public static class DashboardWeb
            {
                public static string AppType = "VTQT.Web.Dashboard";

                public static class Web
                {
                    public static string AssemblyName = "VTQT.Web.Dashboard";
                }

                public static class Admin
                {
                    public static string AssemblyName = "VTQT.Web.Dashboard.Admin";
                }
            }

            public static class TicketApi
            {
                public static string AppType = "VTQT.Api.Ticket";
            }

            public static class TicketWeb
            {
                public static string AppType = "VTQT.Web.Ticket";

                public static class Web
                {
                    public static string AssemblyName = "VTQT.Web.Ticket";
                }

                public static class Admin
                {
                    public static string AssemblyName = "VTQT.Web.Ticket.Admin";
                }
            }

            // AssetApi => AssetWeb.Admin
            // AssetPublicApi => AssetWeb.Web

            public static class AssetApi
            {
                public static string AppType = "VTQT.Api.Asset";
            }

            public static class AssetWeb
            {
                public static string AppType = "VTQT.Web.Asset";

                public static class Web
                {
                    public static string AssemblyName = "VTQT.Web.Asset";
                }

                public static class Admin
                {
                    public static string AssemblyName = "VTQT.Web.Asset.Admin";
                }
            }

            public static class WarehouseApi
            {
                public static string AppType = "VTQT.Api.Warehouse";
            }

            public static class WarehouseWeb
            {
                public static string AppType = "VTQT.Web.Warehouse";

                public static class Web
                {
                    public static string AssemblyName = "VTQT.Web.Warehouse";
                }

                public static class Admin
                {
                    public static string AssemblyName = "VTQT.Web.Warehouse.Admin";
                }
            }

            public static class MasterApi
            {
                public static string AppType = "VTQT.Api.Master";
            }

            public static class MasterWeb
            {
                public static string AppType = "VTQT.Web.Master";

                public static class Web
                {
                    public static string AssemblyName = "VTQT.Web.Master";
                }

                public static class Admin
                {
                    public static string AssemblyName = "VTQT.Web.Master.Admin";
                }
            }

            public static class Other
            {
                public static string AppType = "Other";
            }
        }

        #endregion

        #region Helpers

        public static string GetAppApiTypeMapping(string appMvcType)
        {
            if (appMvcType.Equals(Apps.DashboardWeb.AppType, StringComparison.OrdinalIgnoreCase))
                return Apps.DashboardApi.AppType;
            if (appMvcType.Equals(Apps.TicketWeb.AppType, StringComparison.OrdinalIgnoreCase))
                return Apps.TicketApi.AppType;
            if (appMvcType.Equals(Apps.AssetWeb.AppType, StringComparison.OrdinalIgnoreCase))
                return Apps.AssetApi.AppType;
            if (appMvcType.Equals(Apps.WarehouseWeb.AppType, StringComparison.OrdinalIgnoreCase))
                return Apps.WarehouseApi.AppType;
            if (appMvcType.Equals(Apps.MasterWeb.AppType, StringComparison.OrdinalIgnoreCase))
                return Apps.MasterApi.AppType;

            return string.Empty;
        }

        #endregion
    }
}
