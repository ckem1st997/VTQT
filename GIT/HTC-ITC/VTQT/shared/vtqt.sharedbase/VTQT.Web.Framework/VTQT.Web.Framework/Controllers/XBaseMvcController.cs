using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Infrastructure;
using VTQT.Core.Localization;
using VTQT.Core.Logging;
using VTQT.Services;
using VTQT.Services.Localization;
using VTQT.Services.Logging;
using VTQT.Web.Framework.Filters;
using VTQT.Web.Framework.Localization;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.UI;

namespace VTQT.Web.Framework.Controllers
{
    [SetWorkingCulture(Order = 2)]
    [MvcNotify(Order = 1000)] // Run last (OnResultExecuting)
    public abstract partial class XBaseMvcController : Controller
    {
        protected XBaseMvcController()
        {
        }

        public ILogger Logger
        {
            get;
            set;
        }

        public Localizer T
        {
            get;
            set;
        } = NullLocalizer.Instance;

        public ICommonServices Services
        {
            get;
            set;
        }

        #region Rendering

        /// <summary>
        /// Render component to string
        /// </summary>
        /// <param name="componentName">Component name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<string> RenderViewComponentToStringAsync(string componentName, object arguments = null)
        {
            //original implementation: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.ViewFeatures/Internal/ViewComponentResultExecutor.cs
            //we customized it to allow running from controllers

            if (string.IsNullOrEmpty(componentName))
                throw new ArgumentNullException(nameof(componentName));

            if (!(EngineContext.Current.Resolve<IActionContextAccessor>() is IActionContextAccessor actionContextAccessor))
                throw new Exception("IActionContextAccessor cannot be resolved");

            var context = actionContextAccessor.ActionContext;

            var viewComponentResult = ViewComponent(componentName, arguments);

            var viewData = ViewData;
            if (viewData == null)
                throw new NotImplementedException();

            var tempData = TempData;
            if (tempData == null)
                throw new NotImplementedException();

            await using var writer = new StringWriter();
            var viewContext = new ViewContext(context, NullView.Instance, viewData, tempData, writer, new HtmlHelperOptions());

            // IViewComponentHelper is stateful, we want to make sure to retrieve it every time we need it.
            var viewComponentHelper = EngineContext.Current.Resolve<IViewComponentHelper>();
            (viewComponentHelper as IViewContextAware)?.Contextualize(viewContext);

            var result = viewComponentResult.ViewComponentType == null ?
                await viewComponentHelper.InvokeAsync(viewComponentResult.ViewComponentName, viewComponentResult.Arguments) :
                await viewComponentHelper.InvokeAsync(viewComponentResult.ViewComponentType, viewComponentResult.Arguments);

            result.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            //get Razor view engine
            var razorViewEngine = EngineContext.Current.Resolve<IRazorViewEngine>();

            //create action context
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);

            //set view name as action name in case if not passed
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            //set model
            ViewData.Model = model;

            //try to get a view by the name
            var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
            if (viewResult.View == null)
            {
                //or try to get a view by the path
                viewResult = razorViewEngine.GetView(null, viewName, false);
                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewName} view was not found");
            }
            await using var stringWriter = new StringWriter();
            var viewContext = new ViewContext(actionContext, viewResult.View, ViewData, TempData, stringWriter, new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return stringWriter.GetStringBuilder().ToString();
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Pushes an info message to the notification queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="durable">A value indicating whether the message should be persisted for the next request</param>
        protected virtual void NotifyInfo(string message, bool durable = true)
        {
            Services.MvcNotifier.Information(message, durable);
        }

        /// <summary>
        /// Pushes a warning message to the notification queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="durable">A value indicating whether the message should be persisted for the next request</param>
        protected virtual void NotifyWarning(string message, bool durable = true)
        {
            Services.MvcNotifier.Warning(message, durable);
        }

        /// <summary>
        /// Pushes a success message to the notification queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="durable">A value indicating whether the message should be persisted for the next request</param>
        protected virtual void NotifySuccess(string message, bool durable = true)
        {
            Services.MvcNotifier.Success(message, durable);
        }

        /// <summary>
        /// Pushes an error message to the notification queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="durable">A value indicating whether the message should be persisted for the next request</param>
        protected virtual void NotifyError(string message, bool durable = true)
        {
            if (message.Contains("CONSTRAINT `FK_"))
                message = "Dữ liệu đang được sử dụng, bạn không thể xóa được!";

            Services.MvcNotifier.Error(message, durable);
        }

        /// <summary>
        /// Pushes an error message to the notification queue
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="durable">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether the exception should be logged</param>
        protected virtual void NotifyError(Exception exception, bool durable = true, bool logException = true)
        {
            // TODO-XBase-Log
            //if (logException)
            //{
            //    LogException(exception);
            //}

            Services.MvcNotifier.Error(HttpUtility.HtmlEncode(exception.ToAllMessages()), durable);
        }

        /// <summary>
        /// Pushes an error message to the notification queue that the access to a resource has been denied
        /// </summary>
        /// <param name="durable">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="log">A value indicating whether the message should be logged</param>
        protected virtual void NotifyAccessDenied(bool durable = true, bool log = true)
        {
            var message = T("Admin.AccessDenied.Description");

            // TODO-XBase-Log
            //if (log)
            //{
            //    Logger.Error(message);
            //}

            Services.MvcNotifier.Error(message, durable);
        }

        #endregion

        #region Localization (Sync with XBaseApiController)

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="T">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="localeLabels">Locale labels</param>
        /// <param name="actionLabels">(labels, languageId)</param>
        protected virtual async Task AddApiLocalesAsync<T>(ILanguageService languageService, IList<T> locales, IDictionary<string, string> localeLabels,
            Action<IDictionary<string, string>, string> actionLabels)
            where T : ILocalizedApiLocaleModel
        {
            await AddApiLocalesAsync(languageService, locales, localeLabels, actionLabels, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="T">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="localeLabels">Locale labels</param>
        /// <param name="actionLabels">(labels, languageId)</param>
        /// <param name="configure">Configure action (locale, languageId)</param>
        protected virtual async Task AddApiLocalesAsync<T>(ILanguageService languageService, IList<T> locales, IDictionary<string, string> localeLabels,
            Action<IDictionary<string, string>, string> actionLabels, Action<T, string> configure)
            where T : ILocalizedApiLocaleModel
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var languages = await languageService.GetAllLanguagesAsync(true);
            var curLanguageId = workContext.LanguageId;
            if (string.IsNullOrWhiteSpace(curLanguageId))
                curLanguageId = languages.FirstOrDefault()?.Id;
            foreach (var language in languages)
            {
                var cultureParts = language.LanguageCulture.Split('-');

                var locale = Activator.CreateInstance<T>();
                locale.LanguageId = language.Id;
                locale._LanguageCode = cultureParts[0];
                locale._FlagCode = cultureParts[1];
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }

            actionLabels.Invoke(localeLabels, curLanguageId);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task AddMvcLocalesAsync<TLocalizedMvcModelLocal>(ILanguageService languageService,
            IList<TLocalizedMvcModelLocal> locales) where TLocalizedMvcModelLocal : ILocalizedMvcLocaleModel
        {
            await AddMvcLocalesAsync(languageService, locales, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task AddMvcLocalesAsync<TLocalizedMvcModelLocal>(ILanguageService languageService,
            IList<TLocalizedMvcModelLocal> locales, Action<TLocalizedMvcModelLocal, string> configure) where TLocalizedMvcModelLocal : ILocalizedMvcLocaleModel
        {
            foreach (var language in await languageService.GetAllLanguagesAsync(true))
            {
                var locale = Activator.CreateInstance<TLocalizedMvcModelLocal>();
                locale.LanguageId = language.Id;

                if (configure != null)
                    configure.Invoke(locale, locale.LanguageId);

                locales.Add(locale);
            }
        }

        #endregion

        #region Security

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected virtual IActionResult AccessDeniedView()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            //return Challenge();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = webHelper.GetRawUrl(Request) });
        }

        /// <summary>
        /// Access denied JSON data for DataTables
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the access denied JSON data
        /// </returns>
        protected async Task<JsonResult> AccessDeniedDataTablesJson()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            return ErrorJson(await localizationService.GetResourceAsync("Admin.AccessDenied.Description"));
        }

        #endregion

        #region Redirect

        protected ActionResult RedirectToReferrer()
        {
            return RedirectToReferrer(null, () => RedirectToRoute("HomePage"));
        }

        protected ActionResult RedirectToReferrer(string referrer)
        {
            return RedirectToReferrer(referrer, () => RedirectToRoute("HomePage"));
        }

        protected ActionResult RedirectToReferrer(string referrer, string fallbackUrl)
        {
            // addressing "Open Redirection Vulnerability" (prevent cross-domain redirects / phishing)
            if (fallbackUrl.HasValue() && !Url.IsLocalUrl(fallbackUrl))
            {
                fallbackUrl = null;
            }

            return RedirectToReferrer(
                referrer,
                fallbackUrl.HasValue() ? () => Redirect(fallbackUrl) : (Func<ActionResult>)null);
        }

        protected virtual ActionResult RedirectToReferrer(string referrer, Func<ActionResult> fallbackResult)
        {
            bool skipLocalCheck = false;

            var typedHeaders = Request.GetTypedHeaders();
            if (referrer.IsEmpty() && typedHeaders.Referer != null && typedHeaders.Referer.ToString().HasValue())
            {
                referrer = typedHeaders.Referer.ToString();
                if (referrer.HasValue())
                {
                    var domain1 = (new Uri(referrer)).GetLeftPart(UriPartial.Authority);
                    var domain2 = (new Uri(this.Request.GetDisplayUrl())).GetLeftPart(UriPartial.Authority);
                    if (domain1.IsCaseInsensitiveEqual(domain2))
                    {
                        // always allow fully qualified urls from local host
                        skipLocalCheck = true;
                    }
                    else
                    {
                        referrer = null;
                    }
                }
            }

            // addressing "Open Redirection Vulnerability" (prevent cross-domain redirects / phishing)
            if (referrer.HasValue() && !skipLocalCheck && !Url.IsLocalUrl(referrer))
            {
                referrer = null;
            }

            if (referrer.HasValue())
            {
                return Redirect(referrer);
            }

            if (fallbackResult != null)
            {
                return fallbackResult();
            }

            return NotFound();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Error's JSON data
        /// </summary>
        /// <param name="error">Error text</param>
        /// <returns>Error's JSON data</returns>
        protected JsonResult ErrorJson(string error)
        {
            return Json(new
            {
                error
            });
        }

        /// <summary>
        /// Error's JSON data
        /// </summary>
        /// <param name="errors">Error messages</param>
        /// <returns>Error's JSON data</returns>
        protected JsonResult ErrorJson(object errors)
        {
            return Json(new
            {
                error = errors
            });
        }

        /// <summary>
        /// Display "Edit" (manage) link (in public store)
        /// </summary>
        /// <param name="editPageUrl">Edit page URL</param>
        protected virtual void DisplayEditLink(string editPageUrl)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();

            pageHeadBuilder.AddEditPageUrl(editPageUrl);
        }

        protected virtual IActionResult InvalidModelResult()
        {
            NotifyError(ModelState.GetErrorsToHtml());
            return Ok(new XBaseResult { success = false });
        }

        protected virtual IActionResult HandleApiError<T>(XBaseResult<T> response, bool isAjax = true, object data = null)
            where T : class
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.success)
                throw new Exception("[HandleApiError] Handle \"success=false\" only.");

            if (response.httpStatusCode == (int)HttpStatusCode.InternalServerError)
                Logger.Error(response.message);

            NotifyError(response.GetErrorsToHtml());

            if (isAjax)
            {
                if (data != null)
                    return Ok(data);

                var resData = new XBaseResult<T>(response) { message = null, data = null };
                if (resData.httpStatusCode == 401 || resData.httpStatusCode == 403)
                {
                    var respondedJson = new
                    {
                        status = resData.httpStatusCode,
                        headers = new { location = Request.GetEncodedUrl() }
                    };
                    Response.Headers["X-Responded-JSON"] = JsonConvert.SerializeObject(respondedJson);
                }

                return Ok(resData);
            }

            if (response.isRedirect)
                return Redirect(response.redirectUrl);

            return new StatusCodeResult(response.httpStatusCode);
        }

        protected virtual IActionResult HandleApiResponse<T>(XBaseResult<T> response, bool isAjax = true, object data = null)
            where T : class
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (!response.success)
                return HandleApiError(response: response, isAjax: isAjax, data: data);

            NotifySuccess(response.message);

            if (isAjax)
            {
                var resData = data ?? new XBaseResult<T>(response) { message = null, data = null };
                return Ok(resData);
            }

            if (response.isRedirect)
                return Redirect(response.redirectUrl);

            return View(data ?? response.data);
        }

        #endregion

        #region Helpers

        public string CurrentArea => RouteData.GetAreaName();

        public string CurrentAppAssemblyArea => RouteData.GetAppAssemblyAreaName();

        public string CurrentController => RouteData.GetControllerName();

        public string CurrentAction => RouteData.GetActionName();

        protected string GetLogLevelIcon(LogLevel level)
        {
            var iconClass = "";
            switch (level)
            {
                case LogLevel.Debug:
                    iconClass = "fa fa-play alert-success";
                    break;
                case LogLevel.Information:
                    iconClass = "fa fa-info alert-info";
                    break;
                case LogLevel.Warning:
                    iconClass = "fa fa-exclamation-circle alert-warning";
                    break;
                case LogLevel.Error:
                    iconClass = "fa fa-warning alert-error";
                    break;
                case LogLevel.Fatal:
                    iconClass = "fa fa-times-circle alert-error";
                    break;
            }
            return iconClass;
        }

        protected string GetCrudTypeIcon(CrudType type)
        {
            var iconClass = "";
            switch (type)
            {
                case CrudType.Create:
                    iconClass = "fa fa-plus green";
                    break;
                case CrudType.Read:
                    iconClass = "fa fa-search alert-info";
                    break;
                case CrudType.Update:
                    iconClass = "fa fa-edit yellow";
                    break;
                case CrudType.Delete:
                    iconClass = "fa fa-trash red";
                    break;
            }
            return iconClass;
        }

        #endregion
    }
}
