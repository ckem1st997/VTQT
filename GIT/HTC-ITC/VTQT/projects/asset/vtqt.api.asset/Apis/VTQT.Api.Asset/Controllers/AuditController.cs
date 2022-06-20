using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.Services.Asset.Helper;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("audit")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AuditOffice")]
    public class AuditController : AdminApiController
    {
        #region Fields

        private readonly IAuditService _auditService;
        private readonly IUserModelHelper _userModelHelper;

        #endregion Fields

        #region Ctor

        public AuditController(
            IAuditService auditService,
            IUserModelHelper userModelHelper)
        {
            _auditService = auditService;
            _userModelHelper = userModelHelper;
        }

        #endregion Ctor

        #region List
        [Route("get-organizationUnit")]
        [HttpGet]
        public async Task<IActionResult> GetSelect()
        {
            var entity = _auditService.GetAll();
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.audit"))
                });

            var models = new List<SelectItem>();
            foreach (var e in entity)
            {
                var tem = new SelectItem();
                tem.id = e.Id.ToString();
                tem.text = e.Name;
                models.Add(tem);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }
        /// <summary>
        /// Lấy danh sách  kiểm kê tài sản phân trang
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
                OrganizationId = searchModel.OrganizationId,
                AssetType = (int)searchModel.AssetType,
                EmployeeId = searchModel.EmployeeId,
                FromDate = searchModel.FromDate,
                ToDate = searchModel.ToDate,
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
        /// Lấy danh sách kiểm kê tài sản phân trang hiển thị name join to table
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-list-name")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetShowName([FromQuery] AuditSearchModel searchModel)
        {
            var searchContext = new AuditSearchContext
            {
                Keywords = searchModel.Keywords,
                FromDate = searchModel.FromDate,
                ToDate = searchModel.ToDate,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                OrganizationId = searchModel.OrganizationId,
                AssetType = (int)searchModel.AssetType,
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
            var entities = _auditService.GetListShowName(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
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

        #endregion List

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AuditOffices.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy chi tiết danh sách kiểm kê tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AuditOffices.Details")]
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

            PrepareModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Tạo mới danh sách kiểm kê tài sản
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AuditOffices.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new AuditModel();

            PrepareModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới danh sách kiểm kê tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AuditOffices.Create")]
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
            entity.AuditLocation = model.AuditLocation;

            var detailEntities = new List<AuditDetail>();
            if (model.AuditDetails != null && model.AuditDetails.Any())
            {
                detailEntities = model.AuditDetails.Select(mDetail =>
                {
                    var eDetail = mDetail.ToEntity();
                    eDetail.AuditId = entity.Id;

                    eDetail.FK_AuditDetail_Id_BackReferences = mDetail.AuditDetailSerials.Select(mSerial =>
                    {
                        var eSerial = mSerial.ToEntity();
                        eSerial.AssetId = eDetail.ItemId;
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
        /// Trả về danh sách kiểm kê tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AuditOffices.Edit")]
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

            PrepareModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật danh sách kiểm kê tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AuditOffices.Edit")]
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
        /// Xóa danh sách kiểm kê tài sản theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AuditOffices.Deletes")]
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

        #endregion Methods

        #region Utilities

        private void PrepareModel(AuditModel model)
        {
            model.AvailableCreatedBy = _userModelHelper.GetMvcListItems();
        }

        #endregion Utilities
    }
}