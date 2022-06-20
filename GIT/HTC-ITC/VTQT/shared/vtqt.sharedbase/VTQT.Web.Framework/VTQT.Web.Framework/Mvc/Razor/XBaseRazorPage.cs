using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Infrastructure;
using VTQT.Core.Localization;
using VTQT.Core.Logging;
using VTQT.Web.Framework.Extensions;
using VTQT.Web.Framework.Filters;
using VTQT.Web.Framework.Localization;

namespace VTQT.Web.Framework.Mvc.Razor
{
    // TODO-XBase: UserBase, MobileDevice
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class XBaseRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        private IText _text;
        private IWorkContext _workContext;
        private IAppContext _appContext;
        private string _clientFormId;

        private ICollection<MvcNotifyEntry> _internalMvcNotifications;

        protected bool HasMessages => ResolveMvcNotifications(null).Any();

        /// <summary>
        /// Get a localized resources
        /// </summary>
        //public Localizer T
        //{
        //    get
        //    {
        //        if (_localizationService == null)
        //            _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        //        if (_localizer == null)
        //        {
        //            _localizer = (format, args) =>
        //            {
        //                var resFormat = _localizationService.GetResourceAsync(format).Result;
        //                if (string.IsNullOrEmpty(resFormat))
        //                {
        //                    return new LocalizedString(format);
        //                }
        //                return new LocalizedString((args == null || args.Length == 0)
        //                    ? resFormat
        //                    : string.Format(resFormat, args));
        //            };
        //        }
        //        return _localizer;
        //    }
        //}
        public Localizer T => _text.Get;

        public IWorkContext WorkContext => _workContext;

        public IAppContext AppContext => _appContext;

        /// <summary>
        /// Unique client form Id per user request.
        /// </summary>
        public string ClientFormId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_clientFormId))
                    return _clientFormId;

                var clientFormId = Context.Request.GetValue("ClientFormId");
                _clientFormId = !clientFormId.IsEmpty() ? clientFormId : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                return _clientFormId;
            }
            set => _clientFormId = value;
        }

        protected XBaseRazorPage()
        {
            _text = EngineContext.Current.Resolve<IText>();
            _workContext = EngineContext.Current.Resolve<IWorkContext>();
            _appContext = EngineContext.Current.Resolve<IAppContext>();
        }

        protected ICollection<LocalizedString> GetMessages(MvcNotifyType type)
        {
            return ResolveMvcNotifications(type).AsReadOnly();
        }

        private IEnumerable<LocalizedString> ResolveMvcNotifications(MvcNotifyType? type)
        {
            IEnumerable<MvcNotifyEntry> result = Enumerable.Empty<MvcNotifyEntry>();

            if (_internalMvcNotifications == null)
            {
                string key = MvcNotifyAttribute.NotificationsKey;
                ICollection<MvcNotifyEntry> entries;

                var tempData = ViewContext.TempData;
                if (tempData.ContainsKey(key))
                {
                    //entries = tempData[key] as ICollection<MvcNotifyEntry>;
                    entries = tempData.Get<ICollection<MvcNotifyEntry>>(key);
                    if (entries != null)
                    {
                        result = result.Concat(entries);
                    }
                }

                var viewData = ViewContext.ViewData;
                if (viewData.ContainsKey(key))
                {
                    entries = viewData[key] as ICollection<MvcNotifyEntry>;
                    if (entries != null)
                    {
                        result = result.Concat(entries);
                    }
                }

                _internalMvcNotifications = new HashSet<MvcNotifyEntry>(result);
            }

            if (type == null)
            {
                return _internalMvcNotifications.Select(x => x.Message);
            }

            return _internalMvcNotifications.Where(x => x.Type == type.Value).Select(x => x.Message);
        }

        //public HelperResult RenderWrappedSection(string name, object wrapperHtmlAttributes)
        //{
        //    Func<TextWriter, Task> action = async (TextWriter tw) =>
        //    {
        //        var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(wrapperHtmlAttributes);
        //        var tagBuilder = new TagBuilder("div");
        //        tagBuilder.MergeAttributes(htmlAttributes);

        //        var section = await this.RenderSectionAsync(name, false);
        //        if (section != null)
        //        {
        //            tw.Write(tagBuilder.RenderStartTag());
        //            section.WriteTo(tw, HtmlEncoder.Default);
        //            tw.Write(tagBuilder.RenderEndTag());
        //        }
        //    };
        //    return new HelperResult(action);
        //}

        //public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        //{
        //    return this.IsSectionDefined(sectionName) ? this.RenderSection(sectionName) : defaultContent(new object());
        //}

        ///// <summary>
        ///// Return a value indicating whether the working language and theme support RTL (right-to-left)
        ///// </summary>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the 
        ///// </returns>
        //public async Task<bool> ShouldUseRtlThemeAsync()
        //{
        //    var workContext = EngineContext.Current.Resolve<IWorkContext>();
        //    var supportRtl = (await workContext.GetWorkingLanguageAsync()).Rtl;
        //    if (supportRtl)
        //    {
        //        //ensure that the active theme also supports it
        //        var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
        //        var themeContext = EngineContext.Current.Resolve<IThemeContext>();
        //        supportRtl = (await themeProvider.GetThemeBySystemNameAsync(await themeContext.GetWorkingThemeNameAsync()))?.SupportRtl ?? false;
        //    }

        //    return supportRtl;
        //}
    }

    /// <summary>
    /// Admin web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class XBaseAdminRazorPage<TModel> : XBaseRazorPage<TModel>
    {
        private AdminAreaSettings _adminAreaSettings;
        public AdminAreaSettings AdminAreaSettings => _adminAreaSettings;

        protected XBaseAdminRazorPage()
        {
            _adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();
        }
    }

    /// <summary>
    /// Web view page
    /// </summary>
    public abstract class XBaseRazorPage : XBaseRazorPage<dynamic>
    {
    }
}
