using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Services.Apps;
using VTQT.Services.Configuration;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("app")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.App")]
    public class AppController : AdminApiController
    {
        #region Fields

        private readonly IAppService _appService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public AppController(
            IAppService appService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ISettingService settingService)
        {
            _appService = appService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Apps.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("details")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Apps.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _appService.GetAppByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.App"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            await PrepareAppModel(model, entity);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Apps.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new AppModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            await PrepareAppModel(model, null);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Master.AppActions.Apps.Create")]
        public async Task<IActionResult> Create(AppModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.Url = entity.Url.EnsureEndsWith("/");

            await _appService.InsertAppAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.App"))
            });
        }

        [Route("edit")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Apps.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _appService.GetAppByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.App"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            await PrepareAppModel(model, entity);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        [AppApiAction("Master.AppActions.Apps.Edit")]
        public async Task<IActionResult> Edit(AppModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _appService.GetAppByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.App"))
                });

            entity = model.ToEntity(entity);
            entity.Url = entity.Url.EnsureEndsWith("/");

            await _appService.UpdateAppAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.App"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Master.AppActions.Apps.Deletes")]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var apps = _appService.GetAllApps().Where(w => ids.Contains(w.Id));
            foreach (var app in apps)
            {
                await _appService.DeleteAppAsync(app);

                //when we delete a app we should also ensure that all "per app" settings will also be deleted
                var settingsToDelete = (await _settingService
                        .GetAllSettingsAsync())
                    .Where(s => s.AppId == app.Id)
                    .ToList();

                settingsToDelete.ForEach(x => _settingService.DeleteSettingAsync(x));

                //when we had two apps and now have only one app, we also should delete all "per app" settings
                var allApps = _appService.GetAllApps();

                if (allApps.Count == 1)
                {
                    settingsToDelete = (await _settingService
                            .GetAllSettingsAsync())
                        .Where(s => s.AppId == allApps[0].Id)
                        .ToList();

                    settingsToDelete.ForEach(x => _settingService.DeleteSettingAsync(x));
                }
            }

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.App"))
            });
        }

        #endregion

        #region Lists

        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] AppSearchModel searchModel)
        {
            var models = new List<AppModel>();
            var query = _appService.GetAllApps().AsQueryable();
            var entities = new PagedList<App>(query, searchModel.PageIndex - 1, searchModel.PageSize);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchModel.LanguageId);

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _appService.GetAppByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.App"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        #endregion

        #region Utilities

        private async Task UpdateLocalesAsync(App entity, AppModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        private async Task PrepareAppModel(AppModel model, App app)
        {
            model.AvailableLanguages = (await _languageService.GetAllLanguagesAsync(false, app == null ? "0" : app.Id))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            model.AvailableAppTypes = AppHelperBase.Apps.AllTypes
                .Select(s => new SelectListItem
                {
                    Value = s,
                    Text = s
                }).ToList();
        }

        #endregion
    }
}
