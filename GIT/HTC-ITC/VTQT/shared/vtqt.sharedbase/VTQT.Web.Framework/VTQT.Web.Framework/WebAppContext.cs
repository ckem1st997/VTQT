using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Domain.Apps;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Helpers;

namespace VTQT.Web.Framework
{
    // TODO-XBase-WebAppContext: appContext by Api/Mvc
    /// <summary>
    /// App context for web application
    /// </summary>
    public partial class WebAppContext : IAppContext
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppService _appService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        /// <param name="appService">App service</param>
        public WebAppContext(
            IHttpContextAccessor httpContextAccessor,
            IAppService appService)
        {
            _httpContextAccessor = httpContextAccessor;
            _appService = appService;
        }

        #endregion

        #region Properties

        private AppSvcEntity _currentAppApp;
        // TODO-XBase: API Get from appsettings AppType
        /// <summary>
        /// Gets the current app
        /// </summary>
        public AppSvcEntity CurrentApp
        {
            get
            {
                if (_currentAppApp != null)
                    return _currentAppApp;

                var appProjectType = AppHelper.GetAppProjectType();
                if (appProjectType == AppProjectType.API)
                {
                    var allApps = _appService.GetAllApps();
                    var app = allApps.FirstOrDefault(x => x.AppType == CommonHelper.XBaseConfig.AppType);
                    if (app == null)
                        throw new XBaseException($"Current app [{CommonHelper.XBaseConfig.AppType}] is null.");

                    _currentAppApp = app.ToSvcEntity();

                    return _currentAppApp;
                }
                else if (appProjectType == AppProjectType.MVC)
                {
                    ////try to determine the current app by HOST header
                    //string host = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Host];

                    ////we cannot call async methods here. otherwise, an application can hang. so it's a workaround to avoid that
                    //var allApps = _appService.GetAllApps();

                    //var app = allApps.FirstOrDefault(s => _appService.ContainsHostValue(s, host));

                    ////if (app == null)
                    ////    //load the first found app
                    ////    app = allApps.FirstOrDefault();
                    //if (app == null)
                    //    throw new XBaseException($"Current app [{CommonHelper.XBaseConfig.AppType}] is null.");

                    //_currentAppApp = app.ToSvcEntity() ?? throw new Exception("No app could be loaded");
                    var allApps = _appService.GetAllApps();
                    var app = allApps.FirstOrDefault(x => x.AppType == CommonHelper.XBaseConfig.AppType);
                    if (app == null)
                        throw new XBaseException($"Current app [{CommonHelper.XBaseConfig.AppType}] is null.");

                    _currentAppApp = app.ToSvcEntity();

                    return _currentAppApp;
                }

                return _currentAppApp;
            }
            set
            {
                _currentAppApp = value;
            }
        }

        private AppSvcEntity _masterApp;
        public AppSvcEntity MasterApp
        {
            get
            {
                if (_masterApp != null)
                    return _masterApp;

                var allApps = _appService.GetAllApps();
                var masterApp = allApps.FirstOrDefault(s => s.AppType.Equals(AppHelper.Apps.MasterWeb.AppType, StringComparison.OrdinalIgnoreCase));

                if (masterApp == null)
                {
                    throw new Exception("No master app could be loaded");
                }

                _masterApp = masterApp.ToSvcEntity();

                return _masterApp;
            }
            set
            {
                _masterApp = value;
            }
        }

        private List<AppSvcEntity> _allApps;
        public List<AppSvcEntity> AllApps
        {
            get
            {
                if (_allApps != null)
                    return _allApps;

                var allApps = _appService.GetAllApps();
                var allSvcApps = new List<AppSvcEntity>();
                foreach (var app in allApps)
                {
                    var svcApp = app.ToSvcEntity();
                    svcApp.Name = app.GetLocalized(x => x.Name);
                    svcApp.Description = app.GetLocalized(x => x.Description);

                    allSvcApps.Add(svcApp);
                }

                _allApps = allSvcApps;

                return _allApps;
            }
            set
            {
                _allApps = value;
            }
        }

        #endregion
    }
}
