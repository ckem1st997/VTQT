using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using VTQT.Core;
using VTQT.Core.Infrastructure;

namespace VTQT.Web.Framework.Localization
{
    /// <summary>
    /// Attribute which determines and sets the working culture
    /// </summary>
    public class SetWorkingCultureAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public Lazy<IWorkContext> WorkContext { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null || context.HttpContext == null)
                return;

            HttpRequest request = context.HttpContext.Request;
            if (request == null)
                return;

            WorkContext = new Lazy<IWorkContext>(() => EngineContext.Current.Resolve<IWorkContext>());
            var workContext = WorkContext.Value;
            var workingLanguage = workContext.WorkingLanguage;

            CultureInfo culture;
            // TODO-XBase: UserBase
            //if (workContext.UserBase != null && workingLanguage != null)
            if (workingLanguage != null)
            {
                culture = new CultureInfo(workingLanguage.LanguageCulture);
                workContext.IsEnglishCulture = workContext.CheckEnglishCulture(workingLanguage.LanguageCulture);
            }
            else
            {
                culture = new CultureInfo("en-US");
                workContext.IsEnglishCulture = true;
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

    }
}
