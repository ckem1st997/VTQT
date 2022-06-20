using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("warehouse-user")]
    [ApiController]
    [XBaseApiAuthorize]
    //  [AppApiController("WareHouseUser.Controllers.Home")]
    public class WareHouseUserController : AdminApiController
    {
        #region Fields

        private readonly IWareHouseUserService _iWareHouseUserService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WareHouseUserController(
            IWareHouseUserService iWareHouseUserService, IWorkContext workContext)
        {
            _iWareHouseUserService = iWareHouseUserService;
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
        }

        #endregion


        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        //  [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Create")]
        public async Task<IActionResult> Create(IEnumerable<WareHouseUserModel> models)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entities = new List<WareHouseUser>();
            foreach (var model in models)
            {
                if (await _iWareHouseUserService.ExistAsync(model.WarehouseId, model.UserId) == false)
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
                    message = string.Format(
                        T("Common.Notify.Added"),
                        T("Common.BeginningWareHouse"))
                });
            }

            return Ok(new XBaseResult
            {
                success = false
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
       // [MapAppApiAction(nameof(Edit))]
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

            var listEntity = new List<WareHouseUser>();
            foreach (var item in model.Ids)
            {
                var check = await _iWareHouseUserService.ExistAsync(item, _workContext.UserId);
                if (!check)
                {
                    var tem = new WareHouseUser()
                    {
                        CreatedBy = _workContext.UserId,
                        CreatedDate = DateTime.UtcNow,
                        UserId = _workContext.UserId,
                        WarehouseId = item
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
        //   [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<WareHouseUserModel> models)
        {
            string error = "";
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var entities = new List<WareHouseUser>();
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
        // [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Deletes")]
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

            await _iWareHouseUserService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.BeginningWareHouse"))
            });
        }


        [Route("delete")]
        [HttpDelete]
        //   [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Deletes")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Deletes(string idWareHouse, string idUser)
        {
            if (idWareHouse is null || idUser is null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _iWareHouseUserService.DeletesAsync(idUser, idWareHouse);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.BeginningWareHouse"))
            });
        }


        [Route("check")]
        [HttpGet]
        // [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Check")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Check(string idUser, string idWareHouse)
        {
            if (idUser is null || idWareHouse is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check = await _iWareHouseUserService.ExistAsync(idWareHouse, idUser);
            return Ok(new XBaseResult
            {
                success = check
            });
        }


        [Route("check-role-user")]
        [HttpGet]
        // [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Check")]
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
        // [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Check")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public IActionResult GetRole(string idWareHouse)
        {
            if (idWareHouse is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check = _iWareHouseUserService.GetListRole(idWareHouse);
            var model = new List<WareHouseUserModel>();
            foreach (var item in check)
            {
                var tem = new WareHouseUserModel()
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
        // [AppApiAction("WareHouseUser.AppActions.BeginningWareHouses.Check")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRole()
        {
            var id = _workContext.UserId;
            if (id is null)
                return Ok(new XBaseResult
                {
                    success = false
                });
            var check =await _iWareHouseUserService.GetListByUser(id);
            var model = new List<WareHouseUserModel>();
            foreach (var item in check)
            {
                var tem = new WareHouseUserModel()
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