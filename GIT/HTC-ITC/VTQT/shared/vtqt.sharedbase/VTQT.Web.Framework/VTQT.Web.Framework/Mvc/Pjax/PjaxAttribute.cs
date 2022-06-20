using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VTQT.Web.Framework.Mvc.Pjax
{
    public class PjaxAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var pjaxController = filterContext.Controller as IPjax;
            if (pjaxController == null)
            {
                return;
            }

            var controller = (Controller)filterContext.Controller;

            var pjax = filterContext.HttpContext.Request.Headers[Constants.PjaxHeader];
            pjaxController.IsPjaxRequest = bool.TryParse(pjax, out var pjaxBool) && pjaxBool;
            controller.ViewBag.IsPjaxRequest = pjaxBool;

            var version = filterContext.HttpContext.Session.GetString(Constants.PjaxVersion) ?? Guid.NewGuid().ToString("N");
            filterContext.HttpContext.Session.SetString(Constants.PjaxVersion, version);
            controller.ViewBag.PjaxVersion = version;
            pjaxController.PjaxVersion = version;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var pjaxController = filterContext.Controller as IPjax;
            if (pjaxController == null || !pjaxController.IsPjaxRequest)
            {
                return;
            }

            if (filterContext.HttpContext.Response.Headers.Keys.All(k => k.ToUpper() != Constants.PjaxUrl))
            {
                var url = filterContext.HttpContext.Request.GetEncodedUrl();
                filterContext.HttpContext.Response.Headers.Add(Constants.PjaxUrl, url);
            }

            if (filterContext.HttpContext.Response.Headers.Keys.All(k => k.ToUpper() != Constants.PjaxVersion))
            {
                filterContext.HttpContext.Response.Headers.Add(Constants.PjaxVersion, pjaxController.PjaxVersion);
            }
        }
    }
}
