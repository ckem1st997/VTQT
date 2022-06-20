using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Services;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Master.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class LanguageController : AdminMvcController
    {
        #region Fields

        private readonly ICommonServices _services;
        private readonly ILanguageService _languageService;
        private readonly IAppMappingService _appMappingService;

        #endregion

        #region Constructors

        public LanguageController(
            ICommonServices services,
            ILanguageService languageService,
            IAppMappingService appMappingService)
        {
            _services = services;
            _languageService = languageService;
            _appMappingService = appMappingService;
        }

        #endregion

        #region Methods

        #region Languages

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            var entity = _languageService.GetLanguageById(id);
            if (entity == null)
            {
                NotifyWarning(string.Format(T("Notifies.DoesNotExist"), T("Common.Language")));
                return Ok(new XBaseResult { success = false });
            }

            var model = entity.ToModel();

            PrepareLanguageModel(model, entity, false);

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new LanguageModel();

            PrepareLanguageModel(model, null, false);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageModel model)
        {
            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                await _languageService.InsertLanguageAsync(language);

                //Apps
                //await _appMappingService.InsertAppMappingAsync((language, model.SelectedAppIds));

                NotifySuccess(string.Format(T("Notifies.Added"), T("Common.Language")));
                return Ok(new XBaseResult());
            }

            return InvalidModelResult();
        }

        public ActionResult Edit(string id)
        {
            var entity = _languageService.GetLanguageById(id);
            if (entity == null)
            {
                NotifyWarning(string.Format(T("Notifies.DoesNotExist"), T("Common.Language")));
                return Ok(new XBaseResult { success = false });
            }

            var model = entity.ToModel();

            PrepareLanguageModel(model, entity, false);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LanguageModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _languageService.GetLanguageById(model.Id);
                if (entity == null)
                {
                    NotifyWarning(string.Format(T("Notifies.DoesNotExist"), T("Common.Language")));
                    return Ok(new XBaseResult { success = false });
                }

                //ensure we have at least one published language
                var allLanguages = await _languageService.GetAllLanguagesAsync();
                if (allLanguages.Count == 1 && allLanguages[0].Id == entity.Id &&
                    !model.Published)
                {
                    NotifyWarning(string.Format(T("Notifies.AtLeastOneRequired"), T("Common.Language")));
                    return Ok(new XBaseResult { success = false });
                }

                //update
                entity = model.ToEntity(entity);
                await _languageService.UpdateLanguageAsync(entity);

                //Apps
                //await _appMappingService.InsertAppMappingAsync(entity, model.SelectedAppIds);

                //notification
                NotifySuccess(string.Format(T("Notifies.Updated"), T("Common.Language")));
                return Ok(new XBaseResult());
            }

            return InvalidModelResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            try
            {
                // Ensure we have at least one published language
                var allLanguages = await _languageService.GetAllLanguagesAsync();
                if (allLanguages.Count == 1 && ids.Any(a => a == allLanguages[0].Id))
                {
                    NotifyWarning(string.Format(T("Notifies.AtLeastOneRequired"), T("Common.Language")));
                    return Ok(new XBaseResult { success = false });
                }

                var languages = await _languageService.GetAllLanguagesAsync(true);
                var deleteLanguages = languages.Where(w => ids.Contains(w.Id));
                var deletePublishedLanguages = deleteLanguages.Where(w => w.Published);
                if (allLanguages.Count <= deletePublishedLanguages.Count())
                {
                    NotifyWarning(string.Format(T("Notifies.AtLeastOneRequired"), T("Common.Language")));
                    return Ok(new XBaseResult { success = false });
                }

                foreach (var deleteLanguage in deleteLanguages)
                {
                    await _languageService.DeleteLanguageAsync(deleteLanguage);
                }

                NotifySuccess(string.Format(T("Notifies.Deleted"), T("Common.Language")));
                return Ok(new XBaseResult());
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return Ok(new XBaseResult { success = false });
            }
        }

        #endregion

        #region Resources

        public async Task<ActionResult> Resources(string languageId)
        {
            ViewBag.AllLanguages = (await _languageService.GetAllLanguagesAsync(true))
               .Select(x => new SelectListItem
               {
                   Selected = (x.Id.Equals(languageId)),
                   Text = x.Name,
                   Value = x.Id.ToString()
               }).ToList();

            var language = _languageService.GetLanguageById(languageId);
            ViewBag.LanguageId = languageId;
            ViewBag.LanguageName = language.Name;
            return View();
        }

        #endregion

        #endregion

        #region Action Mapping

        #region Languages

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Languages_Read([DataSourceRequest] DataSourceRequest request)
        {
            var query = await _languageService.GetAllLanguagesAsync(true);

            var result = query.ToDataSourceResult(request,
                s => s.ToModel());

            return Json(result);
        }

        #endregion

        #region Resources

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Resources_Read([DataSourceRequest] DataSourceRequest request, string languageId)
        {
            var language = _languageService.GetLanguageById(languageId);
            var resources = (await _services.Localization.GetAllResourceValuesAsync(languageId))
                .OrderBy(x => x.Key)
                .Where(x => x.Key != "!!___EOF___!!" && x.Value.Value != null)
                .Select(x => new LocaleStringResourceModel
                {
                    LanguageId = languageId,
                    Id = x.Value.Key,
                    ResourceName = x.Key,
                    ResourceValue = x.Value.Value.EmptyNull(),
                });

            var result = resources.ToDataSourceResult(request);

            return Json(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Resources_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<LocaleStringResourceModel> models)
        {
            var results = new List<LocaleStringResourceModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    if (model.ResourceName != null)
                        model.ResourceName = model.ResourceName.Trim();
                    if (model.ResourceValue != null)
                        model.ResourceValue = model.ResourceValue.Trim();

                    var res = await _services.Localization.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId);
                    if (res == null)
                    {
                        var entity = new LocaleStringResource
                        {
                            LanguageId = model.LanguageId,
                            ResourceName = model.ResourceName,
                            ResourceValue = model.ResourceValue
                        };

                        await _services.Localization.InsertLocaleStringResourceAsync(entity);

                        model.Id = entity.Id;
                        results.Add(model);
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format(T("Notifies.AlreadyExist"), T("Common.Fields.ResourceName")));
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Resources_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LocaleStringResourceModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    if (model.ResourceName != null)
                        model.ResourceName = model.ResourceName.Trim();
                    if (model.ResourceValue != null)
                        model.ResourceValue = model.ResourceValue.Trim();

                    var entity = await _services.Localization.GetLocaleStringResourceByIdAsync(model.Id);
                    // if the resourceName changed, ensure it isn't being used by another resource
                    if (!entity.ResourceName.Equals(model.ResourceName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var res = await _services.Localization.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId);
                        if (res != null && res.Id != model.Id)
                        {
                            ModelState.AddModelError("", string.Format(T("Notifies.AlreadyExist"), T("Common.Fields.ResourceName")));
                            break;
                        }
                    }

                    entity.ResourceName = model.ResourceName;
                    entity.ResourceValue = model.ResourceValue;

                    await _services.Localization.UpdateLocaleStringResourceAsync(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Resources_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LocaleStringResourceModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = await _services.Localization.GetLocaleStringResourceByIdAsync(model.Id);
                    if (entity == null)
                    {
                        ModelState.AddModelError("", string.Format(T("Notifies.DoesNotExist"), T("Common.Language")));
                        break;
                    }

                    await _services.Localization.DeleteLocaleStringResourceAsync(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #endregion

        #region Utilities

        private void PrepareLanguageModel(LanguageModel model, Language language, bool excludeProperties)
        {
            var languageId = _services.WorkContext.WorkingLanguage.Id;

            var allCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .OrderBy(x => x.DisplayName)
                .ToList();

            model.AvailableCultures = allCultures
                .Select(x => new SelectListItem { Text = "{0} [{1}]".FormatInvariant(x.DisplayName, x.IetfLanguageTag), Value = x.IetfLanguageTag })
                .ToList();

            model.AvailableTwoLetterLanguageCodes = new List<SelectListItem>();
            model.AvailableFlags = new List<SelectListItem>();

            foreach (var item in allCultures)
            {
                if (!model.AvailableTwoLetterLanguageCodes.Any(x => x.Value.IsCaseInsensitiveEqual(item.TwoLetterISOLanguageName)))
                {
                    // display language name is not provided by net framework
                    var index = item.DisplayName.EmptyNull().IndexOf(" (");

                    if (index == -1)
                        index = item.DisplayName.EmptyNull().IndexOf(" [");

                    var displayName = "{0} [{1}]".FormatInvariant(
                        index == -1 ? item.DisplayName : item.DisplayName.Substring(0, index),
                        item.TwoLetterISOLanguageName);

                    if (item.TwoLetterISOLanguageName.Length == 2)
                    {
                        model.AvailableTwoLetterLanguageCodes.Add(new SelectListItem { Text = displayName, Value = item.TwoLetterISOLanguageName });
                    }
                }
            }
            //foreach (var path in Directory.EnumerateFiles(_services.WebHelper.MapPath("~/Content/global/img/flags/"), "*.svg", SearchOption.TopDirectoryOnly))
            //{
            //    var name = Path.GetFileNameWithoutExtension(path).EmptyNull().ToLower();
            //    string countryDescription = null;

            //    if (countryDescription.IsEmpty())
            //        countryDescription = name;

            //    model.AvailableFlags.Add(new SelectListItem { Text = countryDescription, Value = Path.GetFileName(path) });
            //}

            //model.AvailableFlags = model.AvailableFlags.OrderBy(x => x.Text).ToList();

            //model.AvailableApps = _appModelHelper.GetAllWebTypes();

            //if (!excludeProperties)
            //{
            //    model.SelectedAppIds = _appMappingService.GetAppsIdsWithAccess(language);
            //}
        }

        #endregion
    }
}