using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("dashboard-user")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    //[AppApiController("Dashboard.Controllers.DashBoardUser")]
    public class DashBoardUserController : AdminApiController
    {
        #region Fields

        private readonly IDashboardUserService _iWareHouseUserService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public DashBoardUserController(
            IDashboardUserService iWareHouseUserService, IWorkContext workContext)
        {
            _iWareHouseUserService = iWareHouseUserService;
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
        }

        #endregion


        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        //[AppApiAction("Dashboard.Controllers.DashBoardUser.Create")]
        public async Task<IActionResult> Create(IEnumerable<DashBoardUserModel> models)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entities = new List<DashBoardUser>();
            foreach (var model in models)
            {
                if (await _iWareHouseUserService.ExistAsync(model.TypeValueId, model.UserId) == false)
                {
                    var entity = model.ToEntity();
                    entity.Id = Guid.NewGuid().ToString();
                    entity.CreatedDate = DateTime.UtcNow;
                    entities.Add(entity);
                }
            }

            if (entities?.Count > 0)
            {
                var res = await _iWareHouseUserService.InsertRangeAsync(entities);

                return Ok(new XBaseResult
                {
                    data = null,
                    success = true,
                    message = res > 0
                    ? "Phân quyền thành công !"
                    : "Phân quyền thất bại !"
                });
            }

            return Ok(new XBaseResult
            {
                success = false,
                message = "Xóa quyền thất bại !"
            });
        }

        /// <summary>
        /// Set Role a list Warehouses by User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Activated successfully</response>
        [Route("setlist-role-by-user")]
        [HttpPost]
       // [MapAppApiAction("Dashboard.Controllers.DashBoardUser.Create")]
        public async Task<IActionResult> SetListRoleByUser(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var listEntity = new List<DashBoardUser>();
            foreach (var item in model.Ids)
            {
                var check = await _iWareHouseUserService.ExistAsync(item, _workContext.UserId);
                if (!check)
                {
                    var tem = new DashBoardUser()
                    {
                        CreatedBy = _workContext.UserId,
                        CreatedDate = DateTime.UtcNow,
                        UserId = _workContext.UserId,
                        TypeValueId = item
                    };
                    listEntity.Add(tem);
                }
            }

            var res = await _iWareHouseUserService.InsertRangeAsync(listEntity);

            return Ok(new XBaseResult
            {
                message = res > 0
                    ? "Phân quyền thành công !"
                    : "Phân quyền thất bại !"
            });
        }


        [Route("edit")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        //[AppApiAction("Dashboard.Controllers.DashBoardUser.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<DashBoardUserModel> models)
        {
            string error = "";
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var entities = new List<DashBoardUser>();
            int i = 0;
            foreach (var model in models)
            {
                var entity = await _iWareHouseUserService.GetByIdAsync(model.Id);
                if (entity != null)
                {
                    entity = model.ToEntity(entity);
                    entities.Add(entity);
                }
            }

            await _iWareHouseUserService.UpdateRangeAsync(entities);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.BeginningWareHouse"))
            });
        }

        /// <summary>
        /// Delete a list by list id
        /// </summary>
        /// <param name="ids">list id PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpDelete]
        //[AppApiAction("Dashboard.Controllers.DashBoardUser.Deletes")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
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

            var res = await _iWareHouseUserService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = res > 0
                    ? "Xóa quyền thành công !"
                    : "Xóa quyền thất bại !"
            });
        }


        [Route("delete")]
        [HttpDelete]
        //[AppApiAction("Dashboard.Controllers.DashBoardUser.Deletes")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Deletes(string idTypeValue, string idUser)
        {
            if (idTypeValue is null || idUser is null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var res = await _iWareHouseUserService.DeletesAsync(idUser, idTypeValue);

            return Ok(new XBaseResult
            {
                success = true,
                message = res > 0
                    ? "Xóa quyền thành công !"
                    : "Xóa quyền thất bại !"
            });
        }
        [Route("index")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.DashBoardUser.Index")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("check")]
        [HttpGet]
        //[AppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Check(string idUser, string idTypeValue)
        {
            if (idUser is null || idTypeValue is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check = await _iWareHouseUserService.ExistAsync(idTypeValue, idUser);
            return Ok(new XBaseResult
            {
                success = check
            });
        }


        [Route("check-role-user")]
        [HttpGet]
        //[AppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Check(string idUser)
        {
            if (idUser is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check = await _iWareHouseUserService.ExistAsync(idUser);
            return Ok(new XBaseResult
            {
                success = check
            });
        }

        [Route("get-role")]
        [HttpGet]
        //[AppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public IActionResult GetRole(string idTypeValue)
        {
            if (idTypeValue is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check = _iWareHouseUserService.GetListRole(idTypeValue);
            var model = new List<DashBoardUserModel>();
            foreach (var item in check)
            {
                var tem = new DashBoardUserModel()
                {
                    UserId = item.UserId
                };
                model.Add(tem);
            }

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("get-role-by-user")]
        [HttpGet]
        //[AppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRole()
        {
            var id = _workContext.UserId;
            if (id is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check = await _iWareHouseUserService.GetListByUser(id);
            var model = new List<DashBoardUserModel>();
            foreach (var item in check)
            {
                var tem = new DashBoardUserModel()
                {
                    Id = item.Id,
                    WarehouseName = item.Name
                };
                model.Add(tem);
            }

            return Ok(new XBaseResult
            {
                data = model
            });
        }
    }
}