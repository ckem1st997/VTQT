using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VTQT.Caching;
using VTQT.Caching.Helpers.CacheKeys;
using VTQT.Core;
using VTQT.Services;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Extensions;
using VTQT.Web.Framework.UI;

namespace VTQT.SharedMvc.MvcAdmin.Controllers
{
    public class AdminMvcCommonController : AdminMvcController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly IAppActionService _appActionService;
        private readonly IPermissionService _permissionService;
        private readonly IAppService _appService;
        private readonly IWorkContext _workContext;
        private readonly IAppContext _appContext;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public AdminMvcCommonController(
            ILanguageService languageService,
            IAppActionService appActionService,
            IPermissionService permissionService,
            IAppService appService,
            IWorkContext workContext,
            IAppContext appContext,
            IXBaseCacheManager cacheManager)
        {
            _languageService = languageService;
            _appActionService = appActionService;
            _permissionService = permissionService;
            _appService = appService;
            _workContext = workContext;
            _appContext = appContext;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Languages

        public IActionResult LanguageSelected(int userlanguage)
        {
            //var language = _languageService.GetLanguageById(userlanguage);
            //if (language != null)
            //{
            //    _services.WorkContext.WorkingLanguage = language;
            //}
            return Content(T("Notifies.DataEditSuccess"));
        }

        #endregion

        #region System

        public async Task<IActionResult> ClearCache(string previousUrl)
        {
            await _cacheManager.HybridProvider.RemoveByPrefixAsync("xbase");
            await _cacheManager.HybridProvider.RemoveByPrefixAsync("Enums");
            return Redirect(previousUrl??nameof(Index));
        }



        public async Task<IActionResult> _HMenu()
        {
            var models = new List<AppActionModel>();
            if (_workContext?.User != null)
            {
                var cacheKey = ModelCacheKeys.AdminMenusModelCacheKey.FormatWith(_workContext.UserId, _appContext.CurrentApp.Id, _workContext.WorkingLanguage.Id);

                models = (await _cacheManager.HybridProvider.GetAsync(cacheKey, async () => await PrepareAdminMenuAsync(),
                    TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime))).Value;
            }

            return PartialView("~/Views/MvcAdminCommon/_HMenu.cshtml", models);
        }

        private async Task<List<AppActionModel>> PrepareAdminMenuAsync()
        {
            var query = (await _permissionService
                .GetUserPermissionsAsync(_workContext.UserId, _appContext.CurrentApp.Id))
                .Where(w => w.ShowOnMenu);

            var models = query
                .Select(s => new AppActionModel
                {
                    Id = s.Id,
                    AppId = s.AppId,
                    ParentId = s.ParentId,
                    Name = s.GetLocalized(x => x.Name),
                    Description = s.GetLocalized(x => x.Description),
                    Controller = !string.IsNullOrEmpty(s.Controller) ? s.Controller.Substring(0, s.Controller.IndexOf("Controller", StringComparison.OrdinalIgnoreCase)) : "",
                    Action = s.Action ?? "",
                    Icon = s.Icon ?? "",
                    Active = s.Active,
                    DisplayOrder = s.DisplayOrder
                }).AsEnumerable();

            var parents = models
                .Where(w => string.IsNullOrWhiteSpace(w.ParentId))
                .OrderBy(o => o.DisplayOrder);

            var result = new List<AppActionModel>();
            foreach (var parent in parents)
            {
                var childs = PrepareChildAdminMenu(ref models, parent.Id);
                parent.AppActionChildren = childs;
                result.Add(parent);
            }

            return result;
        }

        private List<AppActionModel> PrepareChildAdminMenu(ref IEnumerable<AppActionModel> models, string parentId)
        {
            var childs = models
                .Where(w => w.ParentId == parentId)
                .OrderBy(o => o.DisplayOrder);

            var result = new List<AppActionModel>();
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    var subChilds = PrepareChildAdminMenu(ref models, child.Id);
                    child.AppActionChildren = subChilds;
                    result.Add(child);
                }
            }

            return result;
        }

        public IActionResult Breadcrumb(string appType, string appControllerName, string appActionName)
        {
            var key = ModelCacheKeys.AdminBreadcrumbsModelCacheKey.FormatWith(appType, appControllerName, appActionName, _workContext.WorkingLanguage.Id);
            var models = _cacheManager.HybridProvider.Get(key, () =>
            {
                var appApiType = AppHelperBase.GetAppApiTypeMapping(appType);
                var appId = _appService.GetAllApps().FirstOrDefault(w => w.AppType.Equals(appApiType, StringComparison.OrdinalIgnoreCase))?.Id;

                var e = _appActionService
                    .GetFirst(x =>
                        x.AppId == appId
                        && !string.IsNullOrEmpty(x.Controller) && !string.IsNullOrEmpty(x.Action)
                        && x.Controller.ToUpper() == appControllerName.ToUpper()
                        && x.Action.ToUpper() == appActionName.ToUpper()
                    );

                var breadcrumb = new List<AppActionModel>();
                while (e != null)
                {
                    breadcrumb.Add(new AppActionModel
                    {
                        Id = e.Id,
                        Name = e.GetLocalized(x => x.Name),
                        Description = e.GetLocalized(x => x.Description),
                        Controller = e.Controller,
                        Action = e.Action,
                        Icon = e.Icon,
                        ShowOnMenu = e.ShowOnMenu,
                        ActionUrl = !e.Controller.IsEmpty() && !e.Action.IsEmpty()
                            ? Url.Action(e.Action, e.Controller.Substring(0, e.Controller.IndexOf("Controller", StringComparison.OrdinalIgnoreCase)))
                            : null
                    });

                    e = e.Parent;
                }

                return breadcrumb;
            }, TimeSpan.FromMinutes(CachingDefaults.MonthCacheTime)).Value;

            return Ok(models);
        }

        #endregion

        #region UI Components

        [HttpPost]
        public IActionResult SetSelectedTab(string navId, string tabId, string path)
        {
            if (navId.HasValue() && tabId.HasValue() && path.HasValue())
            {
                var info = new SelectedTabInfo { TabId = tabId, Path = path };
                //TempData["SelectedTab." + navId] = info;
                TempData.Put($"SelectedTab.{navId}", info);
            }
            return Ok(new { Success = true });
        }

        #endregion

        #region Lists



        #endregion
    }
}
