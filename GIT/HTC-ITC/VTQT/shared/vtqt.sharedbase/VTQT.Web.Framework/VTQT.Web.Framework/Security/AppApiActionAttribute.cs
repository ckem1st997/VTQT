using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Security;

namespace VTQT.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AppApiActionAttribute : ActionFilterAttribute
    {
        public string ResourceKey { get; set; }

        public AppApiActionAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public AppApiActionAttribute(string resourceKey, int order)
        {
            ResourceKey = resourceKey;
            Order = order;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException(nameof(filterContext));

            if (!HasAppActionAccess(filterContext))
                filterContext.Result = new ForbidResult();
        }

        public virtual bool HasAppActionAccess(ActionExecutingContext filterContext)
        {
            var appContext = EngineContext.Current.Resolve<IAppContext>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();

            var appId = appContext.CurrentApp.Id;
            var controller = filterContext.RouteData.Values["controller"] + "Controller";
            var action = filterContext.RouteData.Values["action"].ToString();
            var appAction = new AppAction
            {
                AppId = appId,
                Controller = controller,
                Action = action
            };

            return permissionService.Authorize(appAction, workContext.UserPermissions);
        }
    }
}
