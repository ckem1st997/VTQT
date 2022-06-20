using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Localization;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("warehouse-item-unit")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("WareHouse.Controllers.WareHouseItemUnit")]
    public class WareHouseItemUnitController : AdminApiController
    {
        #region Fields

        private readonly IWareHouseItemService _wareHouseItemService;
        private readonly IWareHouseItemUnitService _wareHouseItemUnitService;
        private readonly IUnitService _unitService;

        #endregion

        #region Ctor

        public WareHouseItemUnitController(
            IWareHouseItemService wareHouseItemService,
            IWareHouseItemUnitService wareHouseItemUnitService,
            IUnitService unitService)
        {
            _wareHouseItemUnitService = wareHouseItemUnitService;
            _wareHouseItemService = wareHouseItemService;
            _unitService = unitService;
        }

        #endregion

        #region Details

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemUnits.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }
        /// <summary>
        /// lấy chi tiết đơn vị tính
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemUnits.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new WareHouseItemUnitModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        ///  Trả về chi tiết đơn vị tính
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemUnits.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _wareHouseItemUnitService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouseItemUnit"))
                });

            var model = entity.ToModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// tạo mới chi tiết đơn vị tính
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItemUnits.Create")]
        public async Task<IActionResult> Create(WareHouseItemUnitModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _wareHouseItemUnitService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.WareHouseItemUnit"))
            });
        }
        /// <summary>
        /// Cập nhật chi tiết đơn vị tính
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItemUnits.Edit")]
        public async Task<IActionResult> Edit(WareHouseItemUnitModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _wareHouseItemUnitService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouseItemUnit"))
                });

            entity = model.ToEntity(entity);

            await _wareHouseItemUnitService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.WareHouseItemUnit"))
            });
        }
        /// <summary>
        /// Xóa chi tiết đơn vị tính theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItemUnits.Deletes")]
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

            await _wareHouseItemUnitService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.WareHouseItemUnit"))
            });
        }

        #endregion

        #region Lists
        /// <summary>
        /// Lấy chi tiết đơn vị tính phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] WareHouseItemUnitSearchModel searchModel)
        {
            var searchContext = new WareHouseItemUnitSearchContext
            {
                ItemId = searchModel.ItemId
            };

            var models = new List<WareHouseItemUnitModel>();
            var entities = _wareHouseItemUnitService.GetByWareHouseItemUnitId(searchContext);

            var units = _unitService.GetAll(true);

            foreach (var e in entities)
            {
                var m = e.ToModel();

                if (!string.IsNullOrWhiteSpace(m.UnitId))

                    m.UnitName = units.FirstOrDefault(w => w.Id == m.UnitId)?.UnitName;

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }


        [Route("get-convert-rate")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetConvertRate(string itemId, string unitId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(unitId))
                return Ok(new XBaseResult
                {
                    data = 0
                });
            return Ok(new XBaseResult
            {
                totalCount = await _wareHouseItemUnitService.GetConvertRate(itemId, unitId)
            });
        }
        #endregion

        #region Helpers


        [Route("check-unit-by-item-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult CheckUnitByItemIdAsync(string ItemId, string UnitId)
        {
            if (string.IsNullOrEmpty(ItemId) || string.IsNullOrEmpty(UnitId))
                return Ok(new XBaseResult
                {
                    success = false
                });
            var entity = _wareHouseItemUnitService.Exists(UnitId, ItemId);
            return Ok(new XBaseResult
            {
                success = entity
            });
        }

        #endregion

        #region Utilities


        private void PrepareDetailModel(WareHouseItemUnitModel model)
        {
            var units = _unitService.GetAll(true);

            if (!string.IsNullOrWhiteSpace(model.UnitId))
                model.UnitName = units.FirstOrDefault(w => w.Id == model.UnitId)?.UnitName;

            model.AvailableUnits = units
                .Where(w => !w.Inactive || w.Id.Equals(model.UnitId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.GetLocalized(x => x.UnitName)
                }).ToList();
        }

        #endregion
    }
}