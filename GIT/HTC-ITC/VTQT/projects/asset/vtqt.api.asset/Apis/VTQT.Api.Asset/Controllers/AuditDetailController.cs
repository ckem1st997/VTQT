using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.Services.Asset;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("audit-detail")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AuditDetailOffice")]
    public class AuditDetailController : AdminApiController
    {
        #region Fields

        private readonly IAuditDetailService _auditDetailService;
        private readonly IAssetService _assetService;
        private readonly IUserModelHelper _userModelHelper;

        #endregion Fields

        #region Ctor

        public AuditDetailController(
            IAuditDetailService auditDetailService,
            IAssetService assetService,
            IUserModelHelper userModelHelper)
        {
            _auditDetailService = auditDetailService;
            _assetService = assetService;
            _userModelHelper = userModelHelper;
        }

        #endregion Ctor

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AuditDetailOffices.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// lấy chi tiết danh sách kiểm kê tài sản
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
        /// tạo mới chi tiết danh sách kiểm kê tài sản
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

            entity.FK_AuditDetail_Id_BackReferences = model.AuditDetailSerials.Select(mSerial =>
            {
                var eSerial = mSerial.ToEntity();
                eSerial.Id = Guid.NewGuid().ToString();
                eSerial.AssetId = entity.ItemId;
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
        /// Trả về chi tiết danh sách kiểm kê tài sản
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
        /// Cập nhật chi tiết danh sách kiểm kê tài sản
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
            entity.FK_AuditDetail_Id_BackReferences = model.AuditDetailSerials.Select(mSerial =>
            {
                var eSerial = mSerial.ToEntity();
                eSerial.Id = Guid.NewGuid().ToString();
                eSerial.AssetId = entity.ItemId;
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
        /// Xóa chi tiết danh sách kiểm kê tài sản theo danh sách
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

        [Route("detail-edit-list")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        [MapAppApiAction(nameof(Index))]
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

        #endregion Methods

        #region Lists

        /// <summary>
        /// Lấy chi tiết danh sách kiểm kê tài sản phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("detail-get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] AuditDetailSearchModel searchModel)
        {
            var searchContext = new AuditDetailSearchContext
            {
                AuditId = searchModel.AuditId
            };

            var models = new List<AuditDetailModel>();
            var entities = _auditDetailService.GetByAuditId(searchContext);
            var assetItems = _assetService.GetAll((int)AssetType.Office);
            var userModels = _userModelHelper.GetAll(true);

            foreach (var e in entities)
            {
                var m = e.ToModel();

                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.ItemName = assetItems.FirstOrDefault(w => w.Id == m.ItemId)?.Name;
                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.AssetItemModel = assetItems.FirstOrDefault(w => w.Id == m.ItemId)?.ToModel();
                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.AssetItemModel = assetItems.FirstOrDefault(w => w.Id == m.ItemId)?.ToModel();

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }


        [Route("get-list-item-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditDetailById(string idOrganization, string idAssetItem)
        {

            var models = new List<AuditDetailModel>();
            var entities = await _auditDetailService.GetAuditDetailByAssetItemId(idOrganization, idAssetItem);
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


        [Route("get-bt-organizationUnitId")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditDetailByOrganizationUnitId(string idOrganization, DateTime dateTime, int assetType)
        {

            var models = new List<AssetModel>();
            var entities = _auditDetailService.GetAuditDetailByOrganizationUnitId(idOrganization, dateTime, assetType);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.AllocationDate = e.AllocationDate;
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
            });
        }

        [Route("get-bt-stationCodeId")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditDetailByStationCode(string idStationCode , DateTime dateTime, int assetType)
        {

            var models = new List<AssetModel>();
            var entities = _auditDetailService.GetAuditDetailByStationCode(idStationCode, dateTime, assetType);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.AllocationDate = e.AllocationDate;
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
            });
        }

        [Route("get-bt-projectCodeId")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditDetailByProjectCode(string idProjectCode, DateTime dateTime, int assetType)
        {

            var models = new List<AssetModel>();
            var entities = _auditDetailService.GetAuditDetailByProjectCode(idProjectCode, dateTime, assetType);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.AllocationDate = e.AllocationDate;
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
            });
        }
        #endregion Lists

        #region Utilities

        private void PrepareDetailModel(AuditDetailModel model)
        {
            var assetItems = _assetService.GetAll((int)AssetType.Office);
            var userModels = _userModelHelper.GetAll(true);

            if (!string.IsNullOrWhiteSpace(model.ItemId))
                model.ItemName = assetItems.FirstOrDefault(w => w.Id == model.ItemId)?.Name;

            model.AvailableItems = assetItems
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"[{s.Code}] {s.GetLocalized(x => x.Name)}"
                }).ToList();
        }

        #endregion Utilities
    }
}