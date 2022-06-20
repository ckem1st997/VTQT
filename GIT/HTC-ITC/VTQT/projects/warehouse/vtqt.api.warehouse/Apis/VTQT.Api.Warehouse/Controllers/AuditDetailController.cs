using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Localization;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("audit-detail")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.AuditDetail")]
    public class AuditDetailController : AdminApiController
    {
        #region Fields
        private readonly IAuditDetailService _auditDetailService;
        private readonly IWareHouseItemService _wareHouseItemService;
        private readonly IUserModelHelper _userModelHelper;

        #endregion

        #region Ctor
        public AuditDetailController(
            IAuditDetailService auditDetailService,
            IWareHouseItemService wareHouseItemService,
            IUserModelHelper userModelHelper)
        {
            _auditDetailService = auditDetailService;
            _wareHouseItemService = wareHouseItemService;
            _userModelHelper = userModelHelper;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.AuditDetails.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// lấy chi tiết danh sách kiểm kê kho
        /// </summary>
        /// <returns></returns>
        [Route("detail-create")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailCreate()
        {
            var model = new AuditDetailModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// tạo mới chi tiết danh sách kiểm kê kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("detail-create")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailCreate(AuditDetailModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.AuditDetailSerials = model.AuditDetailSerials.Select(mSerial =>
            {
                var eSerial = mSerial.ToEntity();
                eSerial.Id = Guid.NewGuid().ToString();
                eSerial.ItemId = entity.ItemId;
                eSerial.AuditDetailId = entity.Id;

                return eSerial;
            });

            await _auditDetailService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.AuditDetail"))
            });
        }
        /// <summary>
        /// Trả về chi tiết danh sách kiểm kê kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("detail-edit")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailEdit(string id)
        {
            var entity = await _auditDetailService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuditDetail"))
                });

            var model = entity.ToModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// Cập nhật chi tiết danh sách kiểm kê kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("detail-edit")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailEdit(AuditDetailModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _auditDetailService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuditDetail"))
                });

            entity = model.ToEntity(entity);
            entity.AuditDetailSerials = model.AuditDetailSerials.Select(mSerial =>
            {
                var eSerial = mSerial.ToEntity();
                eSerial.Id = Guid.NewGuid().ToString();
                eSerial.ItemId = entity.ItemId;
                eSerial.AuditDetailId = entity.Id;

                return eSerial;
            });

            await _auditDetailService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.AuditDetail"))
            });
        }

        /// <summary>
        /// Xóa chi tiết danh sách kiểm kê kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("detail-deletes")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailDeletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _auditDetailService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.AuditDetail"))
            });
        }
        #endregion

        #region Lists
        /// <summary>
        /// Lấy chi tiết danh sách kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] AuditDetailSearchModel searchModel)
        {
            var searchContext = new AuditDetailSearchContext
            {
                AuditId = searchModel.AuditId
            };

            var models = new List<AuditDetailModel>();
            var entities = _auditDetailService.GetByAuditId(searchContext);
            var whItems = _wareHouseItemService.GetAll(true);
            var userModels = _userModelHelper.GetAll(true);
          

            foreach (var e in entities)
            {
                var m = e.ToModel();

                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.ItemName = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.Name;
                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.WareHouseItem = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.ToModel();
                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.WareHouseItem = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.ToModel();
           
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get-list-audit-detail")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAuditDetail([FromQuery]AuditDetailSearchModel searchModel)
        {
            var searchContext = new AuditDetailSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                WareHouesId = searchModel.WareHouesId
            };

            var models = new List<AuditDetailModel>();
            var entities = await _auditDetailService.GetAuditDetail(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Quantity = int.Parse(e.ItemId);
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.Count
            });
        }


        [Route("get-list-item-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditDetailById(string dateTime,string idw, string item)
        {

            var models = new List<AuditDetailModel>();
            var entities = await _auditDetailService.GetAuditDetailByWareHouseId(dateTime ,idw,item);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.ItemName = e.AuditId;
                m.AuditId = "";
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
            });
        }

        [Route("detail-edit-list")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(IEnumerable<AuditDetailModel> models)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entities = new List<AuditDetail>();
            var modelsExist = new List<AuditDetailModel>();
            foreach (var model in models)
            {
                    var entity = model.ToEntity();
                    entities.Add(entity);
            }

            if (entities?.Count > 0 && modelsExist.Count <= 0)
            {
                await _auditDetailService.InsertRangeAsync(entities);

                return Ok(new XBaseResult
                {
                    data = null,
                    success = true,
                    message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.AuditDetailModel"))
                });
            }

            return Ok(new XBaseResult
            {
                success = false,
                data = modelsExist
            });
        }
        #endregion

        #region Utilities
        private void PrepareDetailModel(AuditDetailModel model)
        {
            var whItems = _wareHouseItemService.GetAll(true);
            var userModels = _userModelHelper.GetAll(true);

            if (!string.IsNullOrWhiteSpace(model.ItemId))
                model.ItemName = whItems.FirstOrDefault(w => w.Id == model.ItemId)?.Name;

            model.AvailableItems = whItems
                .Where(w => !w.Inactive || w.Id.Equals(model.ItemId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"[{s.Code}] {s.GetLocalized(x => x.Name)}"
                }).ToList();
        }
        #endregion
    }
}
