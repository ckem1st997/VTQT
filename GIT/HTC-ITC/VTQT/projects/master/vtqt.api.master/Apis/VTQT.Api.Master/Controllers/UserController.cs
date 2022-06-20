using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Services.Master.UserRole;
using VTQT.Services.Security;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("user")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.User")]
    public class UserController : AdminApiController
    {
        #region Fields

        private readonly IKeycloakService _keycloakService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public UserController(
            IKeycloakService keycloakService,
            IUserModelHelper userModelHelper, IUserRoleService userRoleService, IRoleService roleService,
            ISendDiscordHelper sendDiscordHelper, IWorkContext workContext)
        {
            _keycloakService = keycloakService;
            _userModelHelper = userModelHelper;
            _userRoleService = userRoleService;
            _roleService = roleService;
            _sendDiscordHelper = sendDiscordHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Users.Index")]
        public async Task<IActionResult> Index()
        {
            var searchModel = new UserSearchModel();

            return Ok(new XBaseResult
            {
                data = searchModel
            });
        }

        /// <summary>
        /// Lấy dữ liệu người dùng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetUserById(string id)
        {
            var entity = _keycloakService.GetUserById(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.User"))
                });
            }

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }


        [Route("get-user-role")]
        [HttpGet]
        public IActionResult GetUserByUserId(string userId)
        {
            var entity = _userRoleService.GetListUserByUserId(userId);
            var listRole = "";
            foreach (var item in entity)
            {
                listRole += item.RoleId + ",";
            }

            return Ok(new XBaseResult
            {
                data = listRole.Substring(0, listRole.Length - 1)
            });
        }


        [Route("set-user-role")]
        [HttpPost]
        public async Task<IActionResult> SetUserRole(UserRolesModel model)
        {
            // await _sendDiscordHelper.SendMessage(_workContext.UserName + " đã thay đổi quyền cho người dùng " +
            //                                      _keycloakService.GetUserById(model.UserId).UserName);
            var listEntity = _userRoleService.GetListUserByUserId(model.UserId);
            var resDelete = 0;
            if (listEntity != null && listEntity.Any())
                resDelete = await _userRoleService.DeletesAsync(listEntity);
            var listId = model.RoleId?.Split(',');
            if (listId != null && !string.IsNullOrEmpty(model.RoleId) && listId.Any())
            {
                var listEntityInsert = new List<UserRole>();
                foreach (var variable in listId)
                {
                    if (await _roleService.GetByIdAsync(variable) != null)
                    {
                        var tem = new UserRole()
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = model.UserId,
                            RoleId = variable
                        };
                        listEntityInsert.Add(tem);
                    }
                }

                if (listEntityInsert.Count > 0)
                {
                    var res = await _userRoleService.InsertAsync(listEntityInsert);
                    return Ok(new XBaseResult
                    {
                        success = res > 0,
                        message = res > 0
                            ? string.Format(T("Common.Notify.Authorized"), T("Common.User"))
                            : (string)T("Common.Notify.Error")
                    });
                }
            }
            else
                return Ok(new XBaseResult
                {
                    success = resDelete > 0 || listId == null,
                    message = resDelete > 0 || listId == null
                        ? string.Format(T("Common.Notify.Authorized"), T("Common.User"))
                        : (string)T("Common.Notify.Error")
                });

            return Ok(new XBaseResult
            {
                success = false,
                message = T("Common.Notify.Error")
            });
        }


        [Route("get-manager-by-user-role")]
        [HttpGet]
        public IActionResult GetListManageUserByRole()
        {
            var list = _keycloakService.GetAllUsers();
            return Ok(new XBaseResult
            {
                data = list
            });
        }

        #endregion

        #region Lists

        /// <summary>
        /// Lấy danh sách dữ liệu người dùng cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        public async Task<IActionResult> GetAvailable()
        {
            var results = _userModelHelper.GetAll(true);

            return Ok(new XBaseResult
            {
                data = results
            });
        }

        [Route("get-available-pages")]
        [HttpPost]
        public async Task<IActionResult> GetAvailable(UserSearchModel model)
        {
            var results = _userModelHelper.GetAll(true);
            model.PageIndex = model.PageIndex - 1;
            if (!string.IsNullOrEmpty(model.Keywords))
                results = results.Where(x =>
                    x.Email.Contains(model.Keywords) || x.FullName.Contains(model.Keywords) ||
                    x.UserName.Contains(model.Keywords)).ToList();
            return Ok(new XBaseResult
            {
                data = results.Skip(model.PageIndex * model.PageSize).Take(model.PageSize)
            });
        }

        #endregion

        #region Helpers

        #endregion

        #region Utilities

        #endregion
    }
}