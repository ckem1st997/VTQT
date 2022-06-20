using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("role")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.Role")]
    public class RoleController : AdminApiController
    {
        #region Fields

        private readonly IRoleService _roleService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion

        #region Ctor

        public RoleController(
            IRoleService roleService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService)
        {
            _roleService = roleService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Roles.Index")]
        public async Task<IActionResult> Index()
        {
            var searchModel = new RoleSearchModel();

            return Ok(new XBaseResult
            {
                data = searchModel
            });
        }

        [Route("details")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Roles.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _roleService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Role"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DisplayName = entity.GetLocalized(x => x.DisplayName, languageId, false, false);
                locale.Description = entity.GetLocalized(x => x.Description, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Roles.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new RoleModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Master.AppActions.Roles.Create")]
        public async Task<IActionResult> Create(RoleModel model)
        {
            model.Name = model.Name?.Trim();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _roleService.ExistsAsync(model.Name))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Master.Roles.Fields.Name"))
                });

            var entity = model.ToEntity();
            entity.Name = model.Name;

            await _roleService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Role"))
            });
        }

        [Route("edit")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Roles.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _roleService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Role"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DisplayName = entity.GetLocalized(x => x.DisplayName, languageId, false, false);
                locale.Description = entity.GetLocalized(x => x.Description, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        [AppApiAction("Master.AppActions.Roles.Edit")]
        public async Task<IActionResult> Edit(RoleModel model)
        {
            ModelState.Remove("Name");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _roleService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Role"))
                });

            entity = model.ToEntity(entity);

            await _roleService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Role"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Master.AppActions.Roles.Deletes")]
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

            await _roleService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Role"))
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

            await _roleService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.Role"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.Role"))
            });
        }

        #endregion

        #region Lists

        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] RoleSearchModel searchModel)
        {
            var searchContext = new RoleSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<RoleModel>();
            var entities = _roleService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.DisplayName = await e.GetLocalizedAsync(x => x.DisplayName, searchContext.LanguageId);
                m.Description = await e.GetLocalizedAsync(x => x.Description, searchContext.LanguageId);

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        #endregion

        #region Helpers



        #endregion

        #region Utilities

        private async Task UpdateLocalesAsync(Role entity, RoleModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.DisplayName, localized.DisplayName, localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Description, localized.Description, localized.LanguageId);
            }
        }

        #endregion
    }
}
