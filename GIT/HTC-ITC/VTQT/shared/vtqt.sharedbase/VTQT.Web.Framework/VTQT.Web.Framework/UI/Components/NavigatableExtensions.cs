using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using VTQT.Core.Infrastructure;

namespace VTQT.Web.Framework.UI
{

    public static class NavigatableExtensions
    {

        public static bool HasValue(this INavigatable navigatable)
        {
            return (((navigatable.ActionName.HasValue() && navigatable.ControllerName.HasValue()) || navigatable.ActionName.HasValue() || navigatable.RouteName.HasValue()) || navigatable.Url.HasValue());
        }

        public static bool IsCurrent(this INavigatable navigatable, ViewContext viewContext)
        {
            var urlHelper = EngineContext.Current.Resolve<IUrlHelper>();

            string pathAndQuery = viewContext.HttpContext.Request.GetEncodedPathAndQuery();
            string url = navigatable.GenerateUrl();
            string comparing = urlHelper.RouteUrl(viewContext.RouteData.Values);
            return (url.IsCaseInsensitiveEqual(pathAndQuery) || url.IsCaseInsensitiveEqual(comparing));
        }

        public static void Action(this INavigatable navigatable, RouteValueDictionary routeValues)
        {
            routeValues.ApplyTo(navigatable, new Action<INavigatable, string, string, RouteValueDictionary>(NavigatableExtensions.SetAction));
        }

        public static void Action(this INavigatable navigatable, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            SetAction(navigatable, actionName, controllerName, routeValues);
        }

        public static void Action(this INavigatable navigatable, string actionName, string controllerName, object routeValues)
        {
            navigatable.ControllerName = controllerName;
            navigatable.ActionName = actionName;
            navigatable.SetRouteValues(routeValues);
        }

        public static void Route(this INavigatable navigatable, string routeName, object routeValues)
        {
            navigatable.RouteName = routeName;
            navigatable.SetRouteValues(routeValues);
        }

        public static void Route(this INavigatable navigatable, string routeName, RouteValueDictionary routeValues)
        {
            navigatable.RouteName = routeName;
            navigatable.SetRouteValues((IDictionary<string, object>)routeValues);
        }

        public static void ModifyParam(this INavigatable navigatable, string paramName, IEnumerable<string> booleanParamNames = null)
        {
            if (navigatable.ModifiedParam != null)
            {
                navigatable.ModifiedParam.Name = paramName;
                if (booleanParamNames != null)
                {
                    navigatable.ModifiedParam.BooleanParamNames.Clear();
                    navigatable.ModifiedParam.BooleanParamNames.AddRange(booleanParamNames);
                }
            }
        }

        public static void ModifyParam(this INavigatable navigatable, object paramValue)
        {
            if (navigatable.ModifiedParam != null)
            {
                navigatable.ModifiedParam.Value = paramValue;
            }
        }

        public static void Url(this INavigatable navigatable, string value)
        {
            navigatable.Url = value;
        }

        public static string GenerateUrl(this INavigatable navigatable, RouteValueDictionary routeValues = null)
        {
            var urlHelper = EngineContext.Current.Resolve<IUrlHelper>();

            string str = null;

            if (routeValues != null)
            {
                if (routeValues.Any())
                {
                    navigatable.RouteValues.Merge((IDictionary<string, object>)routeValues);
                }
            }
            routeValues = navigatable.RouteValues;

            var hasParam = false;
            var param = navigatable.ModifiedParam;
            if (param != null && param.HasValue() && param.Value != null)
            {
                routeValues[param.Name] = param.Value;
                hasParam = true;
            }

            if (!string.IsNullOrEmpty(navigatable.RouteName))
            {
                return urlHelper.RouteUrl(navigatable.RouteName, routeValues);
            }

            if (!string.IsNullOrEmpty(navigatable.ControllerName) && !string.IsNullOrEmpty(navigatable.ActionName))
            {
                return urlHelper.Action(navigatable.ActionName, navigatable.ControllerName, routeValues, null, null);
            }

            if (!string.IsNullOrEmpty(navigatable.ActionName))
            {
                return urlHelper.Action(navigatable.ActionName, routeValues);
            }

            if (!string.IsNullOrEmpty(navigatable.Url))
            {
                return (!navigatable.Url.StartsWith("~/", StringComparison.Ordinal) ? navigatable.Url : urlHelper.Content(navigatable.Url));
            }

            //if (routeValues == null)
            //{
            //    routeValues = new RouteValueDictionary();
            //    if (navigatable.RouteValues.Any())
            //    {
            //        routeValues.Merge((IDictionary<string, object>)navigatable.RouteValues);
            //    }
            //}

            if (hasParam)
            {
                var booleanParamNames = param.BooleanParamNames;

                foreach (var key in urlHelper.ActionContext.HttpContext.Request.Query.Keys.Where(key => key != null))
                {
                    string value = urlHelper.ActionContext.HttpContext.Request.Query[key];
                    if (booleanParamNames.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        // little hack here due to ugly MVC implementation
                        // find more info here: http://www.mindstorminteractive.com/blog/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                        if (!String.IsNullOrEmpty(value) && value.Equals("true,false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            value = "true";
                        }
                    }
                    routeValues[key] = value;
                }

                routeValues[param.Name] = param.Value;

                var requestContext = urlHelper.ActionContext;
                // TODO-XBase: Retry
                //if (requestContext.RouteData != null && requestContext.RouteData.Route != null)
                //{
                //    var virtualPath = requestContext.RouteData.Route.GetVirtualPath(requestContext, routeValues);
                //    if (virtualPath != null)
                //    {
                //        str = VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(requestContext.HttpContext.Request.ApplicationPath), virtualPath.VirtualPath);
                //    }
                //    else
                //    {
                //        str = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, urlHelper.RequestContext, true);
                //    }
                //}

                // Debug info:
                ///Admin/AdjustSeniorityAnnualLeave/_Seniority

                //var virtualPath = requestContext.RouteData.Route.GetVirtualPath(requestContext, routeValues);
                //Admin/AdjustSeniorityAnnualLeave

                //str = VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(requestContext.HttpContext.Request.ApplicationPath), virtualPath.VirtualPath);
                ///Admin/AdjustSeniorityAnnualLeave

                //str = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, urlHelper.RequestContext, true);
                ///Admin/AdjustSeniorityAnnualLeave

                ///Admin/AdjustSeniorityAnnualLeave

                if (requestContext.RouteData != null && requestContext.RouteData.Routers.Any())
                {
                    str = urlHelper.RouteUrl(routeValues);
                }

                return str;
            }

            if (routeValues.Any())
            {
                str = urlHelper.RouteUrl(routeValues);
            }

            return str;
        }

        private static void SetAction(INavigatable navigatable, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            navigatable.ActionName = actionName;
            navigatable.ControllerName = controllerName;
            navigatable.SetRouteValues((IDictionary<string, object>)routeValues);
        }

        private static void SetRouteValues(this INavigatable navigatable, object values)
        {
            if (values != null)
            {
                navigatable.RouteValues.Clear();
                navigatable.RouteValues.Merge(values);
            }
        }

        private static void SetRouteValues(this INavigatable navigatable, IDictionary<string, object> values)
        {
            if (values != null)
            {
                navigatable.RouteValues.Clear();
                navigatable.RouteValues.Merge(values);
            }
        }


    }

}
