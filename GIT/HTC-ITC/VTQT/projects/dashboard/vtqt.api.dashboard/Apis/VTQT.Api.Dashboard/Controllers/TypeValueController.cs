using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Api.Dashboard.Helper;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Dashboard.Controllers
{
    [Route("type-value")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    //[AppApiController("Dashboard.Controllers.TypeValue")]
    public class TypeValueController : AdminApiController
    {
        #region Fields

        private readonly ITypeValueService _service;
        private readonly ITypeValueModelHelper _wareHouseModelHelper;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion

        #region Ctor

        public TypeValueController(
            ITypeValueService service,
            ITypeValueModelHelper wareHouseModelHelper,
            IXBaseCacheManager cacheManager
        )
        {
            _service = service;
            _wareHouseModelHelper = wareHouseModelHelper;
            _cacheManager = cacheManager;

        }

        [Route("index")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.TypeValue.Index")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }



        /// <summary>
        /// Lấy chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.TypeValue.Details")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Details(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.TypeValue"))
                });

            var model = entity.ToModel();
            var list = await _wareHouseModelHelper.GetTypeValueDropdownTreeAsync(showList: true);
            model.AvailableTypeValue = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            return Ok(new XBaseResult
            {
                data = model
            });
        }


        [Route("create")]
        [HttpGet]
        //[AppApiAction("WareHouse.AppActions.WareHouses.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Create()
        {
            var model = new TypeValueModel();
            var list = await _wareHouseModelHelper.GetTypeValueDropdownTreeAsync(showList: true);
            model.AvailableTypeValue = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("get-tree")]
        [HttpPost]
     //   [MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetTree(bool showHidden = false)
        {
            var entity = await _wareHouseModelHelper.GetTypeValueTree(2, !showHidden);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.TypeValue"))
                });

            return Ok(new XBaseResult
            {
                data = entity
            });
        }


        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.TypeValue.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Create(TypeValueModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            model.Id = Guid.NewGuid().ToString();
            var entity = model.ToEntity();

            await _service.InsertAsync(entity);
            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.TypeValue"))
            });
        }


        /// <summary>
        /// Active a list Warehouses
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Activated successfully</response>
        [Route("activates")]
        [HttpPost]
       // [MapAppApiAction(nameof(Edit))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

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

            await _service.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.TypeValue"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.TypeValue"))
            });
        }


        /// <summary>
        /// Trả về danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.TypeValue.Edit")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Edit(TypeValueModel model)
        {
            var entity = await _service.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.TypeValue"))
                });

            entity = model.ToEntity(entity);

            await _service.UpdateAsync(entity);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.TypeValue"))
            });
        }

        /// <summary>
        /// Xóa danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.TypeValue.Deletes")]
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

            await _service.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.TypeValue"))
            });
        }

        #endregion


        [Route("get-select-tree")]
        [HttpGet]
     //   [MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetSelectTree(bool showList = false)
        {
            var entity = await _wareHouseModelHelper.GetTypeValueDropdownTreeAsync(false, true);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });
            return Ok(new XBaseResult
            {
                success = true,
                data = entity
            });
        }


        /// <summary>
        /// Lấy danh sách  kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
      //  [MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Get([FromQuery] TypeValueSearchModel searchModel)
        {
            var searchContext = new TypeValueSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<TypeValueModel>();
            var entities = _service.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<TypeValueModel>(models, searchContext.PageIndex, searchContext.PageSize),
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// Lấy giá trị lựa chọn kieu du lieu cuối cùng
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        [Route("get-last-selected")]
        [HttpGet]
      //  [MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]


        public async Task<IActionResult> GetLastSelectedDepartment(
            string appId,
            string userId,
            string path,
            string typeValueId)
        {
            if (string.IsNullOrEmpty(appId) ||
                string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(path))
            {
                return Ok(new XBaseResult
                {
                    data = null
                });
            }

            var result = _service.GetLastSelectedNodeTree(appId, userId, path, typeValueId);

            return Ok(new XBaseResult
            {
                success = true,
                data = result
            });
        }

        /// <summary>
        /// Cập nhật cache key chọn kieu du lieu
        /// </summary>        
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        [Route("update-last-selected")]
        [HttpPost]
      //  [MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]


        public async Task<IActionResult> UpdateLastSelectedDepartment(
            string appId,
            string userId,
            string path,
            string typeValueId)
        {
            if (string.IsNullOrEmpty(appId) ||
                string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(path))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            await _cacheManager.HybridProvider.RemoveAsync(string.Format(ModelCacheKeys.TypeValuesTreeModelCacheKey, appId, userId, path));

            var result = _service.GetLastSelectedNodeTree(appId, userId, path, typeValueId);

            return Ok(new XBaseResult
            {
                success = true,
                data = result
            });
        }

    }
}