using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("app-action")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.AppAction")]
    public class AppActionController : AdminApiController
    {
        #region Fields

        private readonly IAppActionService _appActionService;
        private readonly IAppService _appService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAppActionModelHelper _appActionModelHelper;
        private readonly IAppModelHelper _appModelHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AppActionController(
            IAppActionService appActionService,
            IAppService appService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAppActionModelHelper appActionModelHelper,
            IAppModelHelper appModelHelper,
            IWorkContext workContext)
        {
            _appActionService = appActionService;
            _appService = appService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _appActionModelHelper = appActionModelHelper;
            _appModelHelper = appModelHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.AppActions.Index")]
        public async Task<IActionResult> Index()
        {
            var availableApps = _appModelHelper.GetAllApiTypes()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id
                }).ToList();

            var model = new AppActionModel
            {
                AvailableApps = availableApps
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("details")]
        [HttpGet]
        [AppApiAction("Master.AppActions.AppActions.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _appActionService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AppAction"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = entity.GetLocalized(x => x.Description, languageId, false, false);
            });

            PrepareModelForEdit(model, entity);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpGet]
        [AppApiAction("Master.AppActions.AppActions.Create")]
        public async Task<IActionResult> Create(string appId, string parentId)
        {
            var model = new AppActionModel { AppId = appId, ParentId = parentId };

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            PrepareModelForCreate(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Master.AppActions.AppActions.Create")]
        public async Task<IActionResult> Create(AppActionModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.AppId = model.AppId;

            if (!string.IsNullOrWhiteSpace(model.Icon))
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                entity.Icon = sanitizer.Sanitize(model.Icon);
            }

            await _appActionService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.AppAction"))
            });
        }

        [Route("edit")]
        [HttpGet]
        [AppApiAction("Master.AppActions.AppActions.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _appActionService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AppAction"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = entity.GetLocalized(x => x.Description, languageId, false, false);
            });

            PrepareModelForEdit(model, entity);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        [AppApiAction("Master.AppActions.AppActions.Edit")]
        public async Task<IActionResult> Edit(AppActionModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _appActionService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AppAction"))
                });

            //var childs = entity.AppActionChildren;
            var childs = _appActionService.GetChildrenByParentId(entity.Id);
            var parentValid = ValidateParentAppAction(model.Id, model.ParentId, childs);
            if (!parentValid)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.RecursiveParent"), T("Common.AppAction"))
                });
            }

            entity = model.ToEntity(entity);

            AppAction parentAppAction = null;
            if (!string.IsNullOrWhiteSpace(model.ParentId))
                parentAppAction = await _appActionService.GetByIdAsync(model.ParentId);
            if (parentAppAction != null)
            {
                var parentChilds = _appActionService.GetChildrenByParentId(parentAppAction.Id);
                //entity.DisplayOrder = !parentAppAction.AppActionChildren.Any() ? 1 : model.DisplayOrder;
                entity.DisplayOrder = !parentChilds.Any() ? 1 : model.DisplayOrder;
            }
            else
                entity.DisplayOrder = model.DisplayOrder;

            if (!string.IsNullOrWhiteSpace(model.Icon))
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                entity.Icon = sanitizer.Sanitize(model.Icon);
            }

            await _appActionService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.AppAction"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Master.AppActions.AppActions.Deletes")]
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

            await _appActionService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.AppAction"))
            });
        }

        [Route("activates")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _appActionService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.AppAction"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.AppAction"))
            });
        }

        [Route("show-on-menu")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> ShowOnMenu(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _appActionService.ShowOnMenuAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.AppAction"))
            });
        }

        [Route("moves")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> Moves(string id, string parentId, int? displayOrder)
        {
            var entity = await _appActionService.GetByIdAsync(id);

            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AppAction"))
                });

            //var childs = entity.AppActionChildren;
            var childs = _appActionService.GetChildrenByParentId(entity.Id);
            var parentValid = ValidateParentAppAction(id, parentId, childs);
            if (!parentValid)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.RecursiveParent"), T("Common.AppAction"))
                });
            }

            await _appActionService.MoveActionAsync(entity, parentId, displayOrder);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.AppAction"))
            });
        }

        [Route("get-app-action-tree")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public ActionResult GetAppActionTree(string appId, int? expandLevel, bool showHidden = true)
        {
            var result = _appActionModelHelper.GetAppActionTree(appId, expandLevel, showHidden);

            return Ok(new XBaseResult
            {
                data = result
            });
        }

        [Route("get-app-actions")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public ActionResult GetAppActions(string appId, string appApiControllerName)
        {
            var app = _appService.GetAppById(appId);
            var appApiAssemblyFilePath = _workContext.GetAppApiAssemblyFilePath(app.AppType);

            var appActions = _workContext.GetAppApiActions(appApiAssemblyFilePath, appApiControllerName)
                .Select(s => new
                {
                    text = !string.IsNullOrEmpty(s.Description) ? $"{s.Description} ({s.Name})" : s.Name,
                    id = s.Name
                });

            return Ok(new XBaseResult
            {
                data = appActions
            });
        }

        #endregion

        #region Lists



        #endregion

        #region Helpers

        private List<AppActionModel> GetAppActionTreeModel(IEnumerable<AppActionModel> models)
        {
            var parents = models
                .Where(w => string.IsNullOrWhiteSpace(w.ParentId))
                .OrderBy(o => o.DisplayOrder);

            var result = new List<AppActionModel>();
            var level = 0;
            foreach (var parent in parents)
            {
                result.Add(new AppActionModel
                {
                    Id = parent.Id,
                    ParentId = parent.ParentId,
                    Name = parent.Name,
                    DisplayOrder = parent.DisplayOrder
                });
                GetChildAppActionTreeModel(ref models, parent.Id, ref result, level);
            }

            return result;
        }

        private void GetChildAppActionTreeModel(ref IEnumerable<AppActionModel> models, string parentId, ref List<AppActionModel> result, int level)
        {
            level++;
            var childs = models
                .Where(w => w.ParentId == parentId)
                .OrderBy(o => o.DisplayOrder);

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    result.Add(new AppActionModel
                    {
                        Id = child.Id,
                        ParentId = child.ParentId,
                        Name = level.ToTreeLevelString() + child.Name,
                        DisplayOrder = child.DisplayOrder
                    });
                    GetChildAppActionTreeModel(ref models, child.Id, ref result, level);
                }
            }
        }

        private List<AppActionModel> GetAppActionTreeModel(IEnumerable<AppActionModel> models, string excludeId)
        {
            var excludeActions = new List<AppActionModel>();
            var curAction = models.SingleOrDefault(s => s.Id == excludeId);
            if (curAction != null)
            {
                excludeActions.Add(curAction);
                var curActionChilds = models.Where(w => w.ParentId == curAction.Id);
                foreach (var child in curActionChilds)
                {
                    excludeActions.Add(child);
                    GetChildAppActionTreeModel(ref models, child.Id, ref excludeActions);
                }
            }
            var availableModels = models.Except(excludeActions, new AppActionModelIdEqualityComparer());

            var parents = availableModels
                .Where(w => string.IsNullOrWhiteSpace(w.ParentId))
                .OrderBy(o => o.DisplayOrder);
            var result = new List<AppActionModel>();
            var level = 0;
            foreach (var parent in parents)
            {
                result.Add(new AppActionModel
                {
                    Id = parent.Id,
                    ParentId = parent.ParentId,
                    Name = parent.Name,
                    DisplayOrder = parent.DisplayOrder
                });
                GetChildAppActionTreeModel(ref availableModels, parent.Id, ref result, level);
            }

            return result;
        }

        private void GetChildAppActionTreeModel(ref IEnumerable<AppActionModel> models, string parentId, ref List<AppActionModel> result)
        {
            var childs = models
                .Where(w => w.ParentId == parentId)
                .OrderBy(o => o.DisplayOrder);
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    result.Add(new AppActionModel
                    {
                        Id = child.Id,
                        ParentId = child.ParentId,
                        Name = child.Name,
                        DisplayOrder = child.DisplayOrder
                    });
                    GetChildAppActionTreeModel(ref models, child.Id, ref result);
                }
            }
        }

        private bool ValidateParentAppAction(string id, string parentId, IEnumerable<AppAction> childs)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            if (childs == null || !childs.Any())
                return true;

            if (string.IsNullOrEmpty(parentId))
                return true;

            if (parentId == id)
                return false;

            var valid = true;
            foreach (var child in childs)
            {
                if (valid)
                {
                    if (parentId == child.Id)
                    {
                        valid = false;
                        break;
                    }

                    SubValidateParentAppAction(parentId, childs, ref valid);
                }
            }

            return valid;
        }

        private void SubValidateParentAppAction(string parentId, IEnumerable<AppAction> childs, ref bool valid)
        {
            foreach (var child in childs)
            {
                if (valid)
                {
                    if (parentId == child.Id)
                    {
                        valid = false;
                        break;
                    }

                    var childChildren = _appActionService.GetChildrenByParentId(child.Id);
                    //SubValidateParentAppAction(parentId, child.AppActionChildren, ref valid);
                    SubValidateParentAppAction(parentId, childChildren, ref valid);
                }
            }
        }

        #endregion

        #region Utilities

        private async Task UpdateLocalesAsync(AppAction entity, AppActionModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Description, localized.Description, localized.LanguageId);
            }
        }

        private void PrepareModelForCreate(AppActionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var app = _appService.GetAppById(model.AppId);

            model.App = new AppModel
            {
                Id = model.AppId,
                Name = app.GetLocalized(x => x.Name)
            };

            var appActionModels = _appActionModelHelper.GetAppActions(model.AppId, true);

            // AvailableParents
            var appActionTreeModel = GetAppActionTreeModel(appActionModels);
            model.AvailableParents = appActionTreeModel
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                    Selected = !string.IsNullOrWhiteSpace(model.ParentId) && s.Id == model.ParentId
                }).ToList();

            // AvailableControllers
            var appAssemblyAreaFilePath = _workContext.GetAppApiAssemblyFilePath(app.AppType);
            model.AvailableControllers = _workContext.GetAppApiControllers(appAssemblyAreaFilePath)
                .Select(s => new SelectListItem
                {
                    Text = !string.IsNullOrEmpty(s.Description) ? $"{s.Description} ({s.Name})" : s.Name,
                    Value = s.Name
                }).ToList();
        }

        private void PrepareModelForEdit(AppActionModel model, AppAction entity)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            model.App = new AppModel
            {
                Id = entity.AppId,
                Name = entity.App.GetLocalized(x => x.Name)
            };

            var appActionModels = _appActionModelHelper.GetAppActions(model.AppId, true);

            // AvailableParents
            var appActionTreeModel = GetAppActionTreeModel(appActionModels, model.Id);
            model.AvailableParents = appActionTreeModel
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                    Selected = !string.IsNullOrWhiteSpace(entity.ParentId) && s.Id == entity.ParentId
                }).ToList();

            // AvailableControllers
            var appAssemblyAreaFilePath = _workContext.GetAppApiAssemblyFilePath(entity.App.AppType);
            model.AvailableControllers = _workContext.GetAppApiControllers(appAssemblyAreaFilePath)
                .Select(s => new SelectListItem
                {
                    Text = !string.IsNullOrEmpty(s.Description) ? $"{s.Description} ({s.Name})" : s.Name,
                    Value = s.Name,
                    Selected = !string.IsNullOrEmpty(entity.Controller) && entity.Controller.Equals(s.Name, StringComparison.OrdinalIgnoreCase)
                }).ToList();

            // AvailableActions
            model.AvailableActions = !string.IsNullOrEmpty(entity.Controller)
                ? _workContext.GetAppApiActions(appAssemblyAreaFilePath, entity.Controller)
                    .Select(s => new SelectListItem
                    {
                        Text = !string.IsNullOrEmpty(s.Description) ? $"{s.Description} ({s.Name})" : s.Name,
                        Value = s.Name,
                        Selected = !string.IsNullOrEmpty(entity.Action) && entity.Action.Equals(s.Name, StringComparison.OrdinalIgnoreCase)
                    }).ToList()
                : Enumerable.Empty<SelectListItem>().ToList();
        }

        #endregion
    }
}
