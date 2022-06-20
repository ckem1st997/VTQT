using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Services.Apps;
using VTQT.Services.Localization;
using VTQT.Services.Security;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("authorize-role")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.AuthorizeRole")]
    public class AuthorizeRoleController : AdminApiController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;
        private readonly IAppService _appService;
        private readonly IAppActionModelHelper _appActionModelHelper;
        private readonly IAppModelHelper _appModelHelper;

        #endregion

        #region Ctor

        public AuthorizeRoleController(
            IPermissionService permissionService,
            IRoleService roleService,
            IAppService appService,
            IAppActionModelHelper appActionModelHelper,
            IAppModelHelper appModelHelper)
        {
            _permissionService = permissionService;
            _roleService = roleService;
            _appService = appService;
            _appActionModelHelper = appActionModelHelper;
            _appModelHelper = appModelHelper;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.AuthorizeRoles.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("authorize")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Authorize(string roleId)
        {
            var entity = await _roleService.GetByIdAsync(roleId);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Role"))
                });

            var model = entity.ToModel();
            model.DisplayName = await entity.GetLocalizedAsync(x => x.DisplayName);
            model.AvailableApps = _appModelHelper.GetAllApiTypes()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id
                }).ToList();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("authorize")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Authorize(AuthorizeRoleModel model)
        {
            if (model.AppActionIds.Any())
                await _permissionService.UpdateRolePermissionsAsync(model.RoleId, model.AppActionIds, model.AppId);
            else
                await _permissionService.RemoveAllRolePermissionsAsync(model.RoleId, model.AppId);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Authorized"), T("Common.Role"))
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

        [Route("get-app-action-tree")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAppActionTree(string appId, int? expandLevel, string roleId, bool showHidden = true)
        {
            var appActionTreeModelCache = _appActionModelHelper.GetAppActionTree(appId, expandLevel, showHidden);
            var appActionTreeModel = new List<AppActionTreeModel>();
            appActionTreeModelCache.Each(x => appActionTreeModel.Add(x.DeepClone()));

            var roleAppActionIds = (await _permissionService.GetRolePermissionsAsync(roleId))
                .Where(w => w.AppId == appId)
                .Select(s => s.Id)
                .ToList();

            foreach (var actionTreeModel in appActionTreeModel)
            {
                var actionId = actionTreeModel.key;
                if (roleAppActionIds.Any(a => a == actionId))
                {
                    actionTreeModel.selected = true;

                    roleAppActionIds.Remove(actionId);
                }

                var childActionTreeModels = actionTreeModel.children;
                if (childActionTreeModels != null && childActionTreeModels.Any())
                    GetChildAppActionTree(ref childActionTreeModels, ref roleAppActionIds);
            }

            return Ok(new XBaseResult
            {
                data = appActionTreeModel
            });
        }

        #endregion

        #region Helpers

        private void GetChildAppActionTree(ref IEnumerable<AppActionTreeModel> childActionTreeModels, ref List<string> roleActionIds)
        {
            if (childActionTreeModels == null || !childActionTreeModels.Any() || roleActionIds == null || !roleActionIds.Any())
                return;

            foreach (var childActionTreeModel in childActionTreeModels)
            {
                var childActionId = childActionTreeModel.key;
                if (roleActionIds.Any(a => a == childActionId))
                {
                    childActionTreeModel.selected = true;

                    roleActionIds.Remove(childActionId);
                }

                var subChildActionTreeModels = childActionTreeModel.children;
                if (subChildActionTreeModels != null && subChildActionTreeModels.Any())
                    GetChildAppActionTree(ref subChildActionTreeModels, ref roleActionIds);
            }
        }

        #endregion

        #region Utilities



        #endregion
    }
}
