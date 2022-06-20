using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Security;

namespace VTQT.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MapAppApiActionAttribute : ActionFilterAttribute
    {
        private readonly string _includeActionNames;
        private readonly string[] _includeActions;

        /// <summary>
        /// Authorize action by mapping with "Include action names".
        /// </summary>
        /// <param name="includeActionNames">Include action name or names separate by comma (if more than one).</param>
        public MapAppApiActionAttribute(string includeActionNames)
        {
            _includeActionNames = includeActionNames;
        }

        /// <summary>
        /// Authorize action by mapping with "Include actions".
        /// </summary>
        /// <param name="includeActions">
        /// Include actions: area name, controller name and action name per action is required.
        /// <para>Each action has format: "{Area}/{Controller}/{Action}".</para>
        /// </param>
        public MapAppApiActionAttribute(string[] includeActions)
        {
            _includeActions = includeActions;
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

            if (!_includeActionNames.IsEmpty())
            {
                var controller = filterContext.RouteData.Values["controller"] + "Controller";
                var actions = _includeActionNames.Split(',');
                var lstAppAction = new List<AppAction>();
                foreach (var action in actions)
                {
                    lstAppAction.Add(new AppAction
                    {
                        AppId = appId,
                        Controller = controller,
                        Action = action
                    });
                }
                return lstAppAction.Any(a => permissionService.Authorize(a, workContext.UserPermissions));
            }
            if (_includeActions != null && _includeActions.Any())
            {
                var lstAppAction = new List<AppAction>();
                foreach (var item in _includeActions)
                {
                    var arr = item.Split('/');
                    var controller = arr[0] + "Controller";
                    var action = arr[1];
                    lstAppAction.Add(new AppAction
                    {
                        AppId = appId,
                        Controller = controller,
                        Action = action
                    });
                }
                return lstAppAction.Any(a => permissionService.Authorize(a, workContext.UserPermissions));
            }

            return false;
        }
    }
}
