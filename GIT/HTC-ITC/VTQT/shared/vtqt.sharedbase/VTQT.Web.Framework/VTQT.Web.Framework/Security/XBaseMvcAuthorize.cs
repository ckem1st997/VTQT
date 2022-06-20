using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Services.Apps;
using VTQT.Services.Security;
using VTQT.Web.Framework.Helpers;

namespace VTQT.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class XBaseMvcAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var ctxUser = context.HttpContext.User;
            if (workContext.User == null
                && ctxUser != null && ctxUser.Identity.IsAuthenticated)
            {
                workContext.InitUserCache();
                workContext.InitUserPermissionsCache();
                workContext.InitUserTokenCache();
            }

            if (!HasAppActionAccess(context))
                context.Result = new ForbidResult();
        }

        public virtual bool HasAppActionAccess(AuthorizationFilterContext context)
        {
            var appContext = EngineContext.Current.Resolve<IAppContext>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appService = EngineContext.Current.Resolve<IAppService>();

            var appMvcId = appContext.CurrentApp.Id;
            var appMvc = appService.GetAllApps().FirstOrDefault(w => w.Id == appMvcId);

            var appApiType = AppHelperBase.GetAppApiTypeMapping(appMvc.AppType);
            var appApiId = appService.GetAllApps().FirstOrDefault(w => w.AppType.Equals(appApiType, StringComparison.OrdinalIgnoreCase)).Id;

            return workContext.UserPermissions.Any(a => a.AppId == appApiId);
        }
    }
}
