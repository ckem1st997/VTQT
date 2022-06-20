using Microsoft.AspNetCore.Routing;
using VTQT.Web.Framework.Routing;

namespace VTQT
{
    public static class RouteExtensions
    {
        public static string GetAreaName(this RouteData routeData)
        {
            if (routeData.DataTokens.TryGetValue("area", out var value))
            {
                return value as string;
            }
            return null;
        }

        public static string GetAreaName(this RouteBase route)
        {
            return route?.DataTokens?["area"] as string;
        }

        /// <summary>
        /// Generates an identifier for the given route in the form "[{area}.]{controller}.{action}"
        /// </summary>
        public static string GenerateRouteIdentifier(this RouteData routeData)
        {
            string area = routeData.GetAreaName();
            string controller = routeData.Values["controller"].ToString();
            string action = routeData.Values["action"].ToString();

            return "{0}{1}.{2}".FormatInvariant(
                area.HasValue() ? area + "." : "",
                controller,
                action);
        }

        public static bool IsRouteEqual(this RouteData routeData, string controller, string action)
        {
            if (routeData == null)
                return false;

            return routeData.Values["controller"].ToString().IsCaseInsensitiveEqual(controller)
                   && routeData.Values["action"].ToString().IsCaseInsensitiveEqual(action);
        }

        #region Custom: XBase

        public static string GetAppAssemblyAreaName(this RouteData routeData)
        {
            if (routeData.DataTokens.TryGetValue(RouteHelper.DataTokens.AppAssemblyAreaKey, out var value))
            {
                return value as string;
            }
            return null;
        }

        public static string GetAppAssemblyAreaName(this RouteBase route)
        {
            return route?.DataTokens?[RouteHelper.DataTokens.AppAssemblyAreaKey] as string;
        }

        public static string GetControllerName(this RouteData routeData)
        {
            return routeData.Values["controller"].ToString();
        }

        public static string GetActionName(this RouteData routeData)
        {
            return routeData.Values["action"].ToString();
        }

        /// <summary>
        /// Generates an identifier for the given route in the form "[{area}.]{controller}.{action}"
        /// </summary>
        public static string GenerateAppAssemblyRouteIdentifier(this RouteData routeData)
        {
            var appAssemblyaArea = routeData.GetAppAssemblyAreaName();
            var controller = routeData.Values["controller"].ToString();
            var action = routeData.Values["action"].ToString();

            return "{0}{1}.{2}".FormatInvariant(
                appAssemblyaArea.HasValue() ? appAssemblyaArea + "." : "",
                controller,
                action);
        }

        public static bool IsAppAssemblyRouteEqual(this RouteData routeData, string appAssemblyAreaName, string controller, string action)
        {
            if (routeData == null)
                return false;

            return routeData.Values[RouteHelper.DataTokens.AppAssemblyAreaKey].ToString().IsCaseInsensitiveEqual(appAssemblyAreaName)
                   && routeData.Values["controller"].ToString().IsCaseInsensitiveEqual(controller)
                   && routeData.Values["action"].ToString().IsCaseInsensitiveEqual(action);
        }

        #endregion

    }
}
