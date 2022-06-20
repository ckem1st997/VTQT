using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Services.Apps;
using VTQT.Services.Security;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("authorize-user")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.AuthorizeUser")]
    public class AuthorizeUserController : AdminApiController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IAppService _appService;
        private readonly IKeycloakService _keycloakService;
        private readonly IAppActionModelHelper _appActionModelHelper;
        private readonly IAppModelHelper _appModelHelper;

        #endregion

        #region Ctor

        public AuthorizeUserController(
            IPermissionService permissionService,
            IAppService appService,
            IKeycloakService keycloakService,
            IAppActionModelHelper appActionModelHelper,
            IAppModelHelper appModelHelper)
        {
            _permissionService = permissionService;
            _appService = appService;
            _keycloakService = keycloakService;
            _appActionModelHelper = appActionModelHelper;
            _appModelHelper = appModelHelper;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.AuthorizeUsers.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("authorize")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Authorize(string userId)
        {
            var entity = _keycloakService.GetUserById(userId);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.User"))
                });

            var model = entity.ToModel();
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
        public async Task<IActionResult> Authorize(AuthorizeUserModel model)
        {
            if (model.AppActionIds.Any())
                await _permissionService.UpdateUserPermissionsAsync(model.UserId, model.AppActionIds, model.AppId);
            else
                await _permissionService.RemoveAllUserPermissionsAsync(model.UserId, model.AppId);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Authorized"), T("Common.User"))
            });
        }

        #endregion

        #region Lists

        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] UserSearchModel searchModel)
        {
            var searchContext = new UserSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<UserModel>();
            var entities = _keycloakService.GetUsers(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();

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
        public async Task<IActionResult> GetAppActionTree(string appId, int? expandLevel, string userId, bool showHidden = true)
        {
            var appActionTreeModelCache = _appActionModelHelper.GetAppActionTree(appId, expandLevel, showHidden);
            var appActionTreeModel = new List<AppActionTreeModel>();
            appActionTreeModelCache.Each(x => appActionTreeModel.Add(x.DeepClone()));

            var userAppActionIds = (await _permissionService.GetUserPermissionsAsync(userId, appId))
                .Select(s => s.Id)
                .ToList();

            foreach (var actionTreeModel in appActionTreeModel)
            {
                var actionId = actionTreeModel.key;
                if (userAppActionIds.Any(a => a == actionId))
                {
                    actionTreeModel.selected = true;

                    userAppActionIds.Remove(actionId);
                }

                var childActionTreeModels = actionTreeModel.children;
                if (childActionTreeModels != null && childActionTreeModels.Any())
                    GetChildAppActionTree(ref childActionTreeModels, ref userAppActionIds);
            }

            return Ok(new XBaseResult
            {
                data = appActionTreeModel
            });
        }

        #endregion

        #region Helpers

        private void GetChildAppActionTree(ref IEnumerable<AppActionTreeModel> childActionTreeModels, ref List<string> userActionIds)
        {
            if (childActionTreeModels == null || !childActionTreeModels.Any() || userActionIds == null || !userActionIds.Any())
                return;

            foreach (var childActionTreeModel in childActionTreeModels)
            {
                var childActionId = childActionTreeModel.key;
                if (userActionIds.Any(a => a == childActionId))
                {
                    childActionTreeModel.selected = true;

                    userActionIds.Remove(childActionId);
                }

                var subChildActionTreeModels = childActionTreeModel.children;
                if (subChildActionTreeModels != null && subChildActionTreeModels.Any())
                    GetChildAppActionTree(ref subChildActionTreeModels, ref userActionIds);
            }
        }

        #endregion

        #region Utilities



        #endregion
    }
}
