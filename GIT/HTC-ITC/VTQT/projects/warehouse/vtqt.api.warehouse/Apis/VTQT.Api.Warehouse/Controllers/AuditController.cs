using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Api.Warehouse.Helper;
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
    [Route("audit")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.Audit")]
    public class AuditController : AdminApiController
    {
        #region Fields
        private readonly IAuditService _auditService;
        private readonly IWareHouseService _wareHouseService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly IWareHouseModelHelper _wareHouseModelHelper;

        #endregion

        #region Ctor
        public AuditController(
            IAuditService auditService,
            IWareHouseService wareHouseService,
            IUserModelHelper userModelHelper,
            IWareHouseModelHelper wareHouseModelHelper

            )
        {
            _auditService = auditService;
            _wareHouseService = wareHouseService;
            _userModelHelper = userModelHelper;
            _wareHouseModelHelper = wareHouseModelHelper;
        }
        #endregion

        #region List

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Audits.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy danh sách  kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] AuditSearchModel searchModel)
        {
            var searchContext = new AuditSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                WareHouesId = searchModel.WareHouesId
            };

            if (searchModel.FromDate.HasValue)
            {
                searchContext.FromDate = searchModel.FromDate;
            }

            if (searchModel.ToDate.HasValue)
            {
                searchContext.ToDate = searchModel.ToDate;
            }

            var models = new List<AuditModel>();
            var entities = _auditService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.CreatedBy = e.CreatedBy;
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.VoucherDate = e.VoucherDate.ToLocalTime();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<AuditModel>(models, searchContext.PageIndex, searchContext.PageSize, entities.TotalCount)
            });
        }

        /// <summary>
        /// Lấy danh sách kiểm kê kho phân trang hiển thị name join to table
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-list-name")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetShowName([FromQuery] AuditSearchModel searchModel)
        {
            var searchContext = new AuditSearchContext
            {
                Keywords = searchModel.Keywords,
                FromDate = searchModel.FromDate,
                ToDate = searchModel.ToDate,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                WareHouesId = searchModel.WareHouesId,
                EmployeeId = searchModel.EmployeeId,
            };
            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }
            var models = new List<AuditModel>();
            var entities = await _auditService.GetListShowName(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.VoucherDate = e.VoucherDate.ToLocalTime();
                m.WareHouseId = e.WareHouseId;
                m.WareHouse = e.WareHouse == null ? new WareHouseModel
                {
                    Name = ""
                } : new WareHouseModel
                {
                    Id = e.WareHouse.Id,
                    Name = e.WareHouse.Name
                };

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<AuditModel>(models, searchContext.PageIndex, searchContext.PageSize, entities.TotalCount)
            });
        }
        #endregion

        #region Methods

        /// <summary>
        /// Lấy chi tiết danh sách kiểm kê kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //[AppApiAction("WareHouse.AppActions.Audits.Index")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _auditService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Audit"))
                });

            var model = entity.ToModel();

            await PrepareModelAsync(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Tạo mới danh sách kiểm kê kho
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Audits.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new AuditModel();

            await PrepareModelAsync(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới danh sách kiểm kê kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Audits.Create")]
        public async Task<IActionResult> Create(AuditModel model)
        {
            if (model.AuditDetails != null && model.AuditDetails.Any())
            {
                var i = 0;
                foreach (var detail in model.AuditDetails)
                {
                    ModelState.Remove($"AuditDetails[{i}].AuditId");
                    i++;
                }
            }
            if (model.AuditCouncils != null && model.AuditCouncils.Any())
            {
                var i = 0;
                foreach (var auditCouncil in model.AuditCouncils)
                {
                    ModelState.Remove($"AuditCouncils[{i}].AuditId");
                    i++;
                }
            }
            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _auditService.ExistsAsync(model.VoucherCode))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Common.Fields.VoucherCode"))
                });

            var entity = model.ToEntity();
            entity.VoucherCode = model.VoucherCode;
            entity.VoucherDate = model.VoucherDate;

            var detailEntities = new List<AuditDetail>();
            if (model.AuditDetails != null && model.AuditDetails.Any())
            {
                detailEntities = model.AuditDetails.Select(mDetail =>
                {
                    var eDetail = mDetail.ToEntity();
                    eDetail.AuditId = entity.Id;

                    eDetail.AuditDetailSerials = mDetail.AuditDetailSerials.Select(mSerial =>
                    {
                        var eSerial = mSerial.ToEntity();
                        eSerial.ItemId = eDetail.ItemId;
                        eSerial.AuditDetailId = eDetail.Id;

                        return eSerial;
                    });

                    return eDetail;
                }).ToList();
            }

            var auditCouncilEntities = new List<AuditCouncil>();
            if (model.AuditCouncils != null && model.AuditCouncils.Any())
            {
                auditCouncilEntities = model.AuditCouncils.Select(mAuditCouncil =>
                {
                    var eAuditCouncil = mAuditCouncil.ToEntity();
                    eAuditCouncil.AuditId = entity.Id;

                    return eAuditCouncil;
                }).ToList();
            }

            await _auditService.InsertAsync(entity, detailEntities, auditCouncilEntities);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Audit"))
            });
        }

        /// <summary>
        /// Trả về danh sách kiểm kê kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Audits.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _auditService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Audit"))
                });

            var model = entity.ToModel();

            await PrepareModelAsync(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật danh sách kiểm kê kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Audits.Edit")]
        public async Task<IActionResult> Edit(AuditModel model)
        {
            if (model.AuditDetails != null && model.AuditDetails.Any())
            {
                var i = 0;
                foreach (var detail in model.AuditDetails)
                {
                    ModelState.Remove($"AuditDetails[{i}].AuditId");
                    i++;
                }
            }
            ModelState.Remove("VoucherCode");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _auditService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Audit"))
                });

            entity = model.ToEntity(entity);
            entity.VoucherDate = model.VoucherDate;

            await _auditService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Audit"))
            });
        }

        /// <summary>
        /// Xóa danh sách kiểm kê kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Audits.Deletes")]
        public IActionResult Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            _auditService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.Audit"))
            });
        }
        #endregion

        #region Utilities

        private async Task PrepareModelAsync(AuditModel model)
        {
            // model.AvailableWareHouses = _wareHouseService.GetAll()
            //     .Select(s => new SelectListItem
            //     {
            //         Value = s.Id,
            //         Text = $"[{s.Code}] {s.GetLocalized(x => x.Name)}"
            //     }).ToList();
            var list = await _wareHouseModelHelper.GetWareHouseDropdownTreeAsync();
            model.AvailableWareHouses = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            model.AvailableCreatedBy = _userModelHelper.GetMvcListItems();
        }

        #endregion

    }
}
