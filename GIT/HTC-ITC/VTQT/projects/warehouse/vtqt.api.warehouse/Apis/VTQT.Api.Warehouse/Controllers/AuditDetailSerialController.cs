using Microsoft.AspNetCore.Mvc;
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
    [Route("audit-detail-serial")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.AuditDetailSerial")]
    public class AuditDetailSerialController : AdminApiController
    {
        #region Fields
        private readonly IAuditDetailSerialService _auditDSService;

        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public AuditDetailSerialController(
            IAuditDetailSerialService auditDSService,
            ILocalizationService localizationService)
        {
            _auditDSService = auditDSService;
            _localizationService = localizationService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.AuditDetailSerials.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Thêm mới Serial chi tiết kiểm kê
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.AuditDetailSerials.Create")]
        public async Task<IActionResult> Create(AuditDetailSerialModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _auditDSService.ExistsAsync(model.ItemId))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Common.AuditDetailSerial"))
                });

            var entity = model.ToEntity();

            await _auditDSService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.AuditDetailSerial"))
            });
        }

        /// <summary>
        /// Cập nhật Serial chi tiết kiểm kê
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.AuditDetailSerials.Edit")]
        public async Task<IActionResult> Edit(AuditDetailSerialModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _auditDSService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AuditDetailSerial"))
                });

            entity = model.ToEntity(entity);

            await _auditDSService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.AuditDetailSerial"))
            });
        }

        /// <summary>
        /// Xóa Serial chi tiết kiểm kê theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.AuditDetailSerials.Deletes")]
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

            await _auditDSService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.AuditDetailSerial"))
            });
        }

        /// <summary>
        /// Xóa danh sách Chi tiết  serial theo Id phiếu nhập
        /// </summary>
        /// <param name="idAuditDetailSerial"></param>
        /// <returns></returns>

        [Route("deletes-list")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.AuditDetailSerials.Deletes")]
        public async Task<IActionResult> Deletes(string idAuditDetailSerial)
        {
            if (idAuditDetailSerial == null || !idAuditDetailSerial.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }
            var list = _auditDSService.GetListById(idAuditDetailSerial);
            await _auditDSService.DeletesAsync(list);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.AuditDetailSerial"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu Serial chi tiết kiểm kê theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _auditDSService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AuditDetailSerial"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy danh sách  Serial chi tiết kiểm kê theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [Route("get-list-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetListById(string id)
        {
            var entity = _auditDSService.GetListById(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AuditDetailSerial"))
                });

            var models = new List<AuditDetailSerialModel>();
            foreach (var e in entity)
            {
                var m = e.ToModel();
                models.Add(m);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = models.Count()
            });
        }
        #endregion

        #region Utilities

        #endregion
    }
}
