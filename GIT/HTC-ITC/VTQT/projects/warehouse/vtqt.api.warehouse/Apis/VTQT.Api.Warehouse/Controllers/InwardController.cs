using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using VTQT.Api.Warehouse.Helper;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Domain.Warehouse.Enum;
using VTQT.Services.Helpers;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Security;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("inward")]
    [ApiController]
  //  [XBaseApiAuthorize]
    [AppApiController("WareHouse.Controllers.Inward")]
    public class InwardController : AdminApiController
    {
        #region Fields

        private readonly IInwardService _inwardService;
        private readonly IInwardDetailService _inwardDetailService;
        private readonly IWareHouseService _wareHouseService;
        private readonly IVendorService _vendorService;
        private readonly IWareHouseItemService _wareHouseItemService;
        private readonly IUnitService _unitService;
        private readonly IOrganizationService _organizationService;
        private readonly IStationService _stationService;
        private readonly IProjectService _projectService;
        private readonly ICustomerService _customerService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly IWareHouseModelHelper _wareHouseModelHelper;
        private readonly IAccObjectService _service;


        #endregion

        #region Ctor

        public InwardController(
            IInwardService inwardService,
            IInwardDetailService inwardDetailService,
            IWareHouseService wareHouseService,
            IVendorService vendorService,
            IWareHouseItemService wareHouseItemService,
            IUnitService unitService,
            IOrganizationService organizationService,
            IStationService stationService,
            IProjectService projectService,
            ICustomerService customerService,
            IUserModelHelper userModelHelper,
            IWareHouseModelHelper wareHouseModelHelper,
            IAccObjectService service
            )
        {
            _inwardService = inwardService;
            _inwardDetailService = inwardDetailService;
            _wareHouseService = wareHouseService;
            _vendorService = vendorService;
            _wareHouseItemService = wareHouseItemService;
            _unitService = unitService;
            _organizationService = organizationService;
            _stationService = stationService;
            _projectService = projectService;
            _customerService = customerService;
            _userModelHelper = userModelHelper;
            _wareHouseModelHelper = wareHouseModelHelper;
            _service = service;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Inwards.Index")]
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
        [AppApiAction("WareHouse.AppActions.Inwards.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _inwardService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Inward"))
                });

            var model = entity.ToModel();            

            await PrepareModelAsync(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Tạo mới danh sách nhập kho
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Inwards.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new InwardModel();

           await PrepareModelAsync(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Inwards.Create")]
        public async Task<IActionResult> Create(InwardModel model)
        {
            if (model.InwardDetails != null && model.InwardDetails.Any())
            {
                var i = 0;
                foreach (var detail in model.InwardDetails)
                {
                    ModelState.Remove($"InwardDetails[{i}].InwardId");
                    i++;
                }
            }
            if (!ModelState.IsValid)
                return InvalidModelResult();

            //if (await _inwardService.ExistsAsync(model.VoucherCode))
            //    return Ok(new XBaseResult
            //    {
            //        success = false,
            //        message = string.Format(T("Common.Notify.AlreadyExist"), T("Common.Fields.VoucherCode"))
            //    });

            var entity = model.ToEntity();
            entity.VoucherCode = model.VoucherCode;
            entity.VoucherDate = model.VoucherDate.ToUniversalTime();
            entity.Reference = JsonConvert.SerializeObject(model.Reference);

            var detailEntities = new List<InwardDetail>();
            if (model.InwardDetails != null && model.InwardDetails.Any())
            {
                detailEntities = model.InwardDetails.Select(mDetail =>
                {
                    var eDetail = mDetail.ToEntity();
                    eDetail.InwardId = entity.Id;

                    eDetail.SerialWareHouses = mDetail.SerialWareHouses.Select(mSerial =>
                    {
                        var eSerial = mSerial.ToEntity();
                        eSerial.ItemId = eDetail.ItemId;
                        eSerial.InwardDetailId = eDetail.Id;

                        return eSerial;
                    });

                    return eDetail;
                }).ToList();
            }

            await _inwardService.InsertAsync(entity, detailEntities);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Inward")),
                data = entity.Id
            });
        }

        /// <summary>
        /// Trả về danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Inwards.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _inwardService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Inward"))
                });

            var model = entity.ToModel();            

            await PrepareModelAsync(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// Cập nhật danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Inwards.Edit")]
        public async Task<IActionResult> Edit(InwardModel model)
        {
            if (model.InwardDetails != null && model.InwardDetails.Any())
            {
                var i = 0;
                foreach (var detail in model.InwardDetails)
                {
                    ModelState.Remove($"InwardDetails[{i}].InwardId");
                    i++;
                }
            }
            ModelState.Remove("VoucherCode");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _inwardService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Inward"))
                });

            entity = model.ToEntity(entity);
            entity.VoucherDate = model.VoucherDate.ToUniversalTime();
            entity.Reference = JsonConvert.SerializeObject(model.Reference);

            await _inwardService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Inward"))
            });
        }
        /// <summary>
        /// Xóa danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Inwards.Deletes")]
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

            await _inwardService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Inward"))
            });
        }

        /// <summary>
        /// Kiểm tra vật tư tồn tại trong kho
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        [Route("check-item-exist")]
        [HttpPost]
        public async Task<IActionResult> CheckItemExist(string itemId, string warehouseId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(warehouseId))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            bool result = _inwardService.CheckItemExistAsync(itemId, warehouseId);

            return Ok(new XBaseResult
            {
                data = result,
                success = true
            });
        }
        #endregion

        #region Details
        /// <summary>
        /// lấy chi tiết danh sách nhập kho
        /// </summary>
        /// <returns></returns>
        [Route("detail-create")]
        [HttpGet]
        [MapAppApiAction(nameof(Create))]
        public async Task<IActionResult> DetailCreate()
        {
            var model = new InwardDetailModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// tạo mới chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("detail-create")]
        [HttpPost]
        [MapAppApiAction(nameof(Create))]
        public async Task<IActionResult> DetailCreate(InwardDetailModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.SerialWareHouses = model.SerialWareHouses.Select(mSerial =>
            {
                var eSerial = mSerial.ToEntity();
                eSerial.Id = Guid.NewGuid().ToString();
                eSerial.ItemId = entity.ItemId;
                eSerial.InwardDetailId = entity.Id;

                return eSerial;
            });

            await _inwardDetailService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.InwardDetail"))
            });
        }
        /// <summary>
        /// Trả về chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("detail-edit")]
        [HttpGet]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> DetailEdit(string id)
        {
            var entity = await _inwardDetailService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.InwardDetail"))
                });

            var model = entity.ToModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }
        /// <summary>
        /// Cập nhật chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("detail-edit")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> DetailEdit(InwardDetailModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _inwardDetailService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.InwardDetail"))
                });

            entity = model.ToEntity(entity);
            entity.SerialWareHouses = model.SerialWareHouses.Select(mSerial =>
            {
                var eSerial = mSerial.ToEntity();
                eSerial.Id = Guid.NewGuid().ToString();
                eSerial.ItemId = entity.ItemId;
                eSerial.InwardDetailId = entity.Id;

                return eSerial;
            });

            await _inwardDetailService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.InwardDetail"))
            });
        }
        /// <summary>
        /// Xóa chi tiết danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("detail-deletes")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
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

            await _inwardDetailService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.InwardDetail"))
            });
        }

        #endregion

        #region Lists
        /// <summary>
        /// Lấy chi tiết danh sách nhập kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("detail-get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] InwardDetailSearchModel searchModel)
        {
            var searchContext = new InwardDetailSearchContext
            {
                InwardId = searchModel.InwardId
            };

            var models = new List<InwardDetailModel>();
            var entities = _inwardDetailService.GetByInwardId(searchContext);

            var whItems = _wareHouseItemService.GetAll(true);
            var units = _unitService.GetAll(true);
            var orgs = _organizationService.GetAll(true);
            var userModels = _userModelHelper.GetAll(true);
            var stations = _stationService.GetAll();
            var projects = _projectService.GetAll(true);
            var customers = _customerService.GetAll(true);

            foreach (var e in entities)
            {
                var m = e.ToModel();

                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.CodeItem = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.Code;

                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.ItemName = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.Name;
                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.WareHouseItem = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.ToModel();
                if (!string.IsNullOrWhiteSpace(m.ItemId))
                    m.WareHouseItem = whItems.FirstOrDefault(w => w.Id == m.ItemId)?.ToModel();
                if (!string.IsNullOrWhiteSpace(m.UnitId))
                    m.UnitName = units.FirstOrDefault(w => w.Id == m.UnitId)?.UnitName;
                //   m.UnitName = units.FirstOrDefault(w => w.Id == m.WareHouseItem.UnitId)?.UnitName;
                if (!string.IsNullOrWhiteSpace(m.DepartmentId))
                    m.DepartmentName = orgs.FirstOrDefault(w => w.Id == int.Parse(m.DepartmentId))?.Name;
                if (!string.IsNullOrWhiteSpace(m.EmployeeId))
                    m.EmployeeName = userModels.FirstOrDefault(w => w.Id == m.EmployeeId)?.FullName;
                if (!string.IsNullOrWhiteSpace(m.StationId))
                    m.StationName = stations.FirstOrDefault(w => w.id == int.Parse(m.StationId))?.ten_tram;
                if (!string.IsNullOrWhiteSpace(m.ProjectId))
                    m.ProjectName = projects.FirstOrDefault(w => w.Id == int.Parse(m.ProjectId))?.ProjectName;
                if (!string.IsNullOrWhiteSpace(m.CustomerId))
                    m.CustomerName = customers.FirstOrDefault(w => w.Id == int.Parse(m.CustomerId))?.FullName;

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        [Route("check-ui-quantity")]
        [HttpGet]
      //  [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> CheckUiQuantity(string IdWareHouse, string IdItem)
        {
            var res = _wareHouseModelHelper.CheckUIQuantity(IdWareHouse, IdItem);
            return Ok(new XBaseResult
            {
                data = res
            });
        }

        #endregion

        #region Helpers



        #endregion

        #region Utilities

        private async Task PrepareModelAsync(InwardModel model)
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
            model.AvailableVendors = _vendorService.GetAll()
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"[{s.Code}] {s.GetLocalized(x => x.Name)}"
                }).ToList();
            model.AvailableReasons = EnumHelper.GetMvcListItems<InwardReason>();
            model.AvailableCreatedBy = _userModelHelper.GetMvcListItems();
            model.AvailableAccObject = _service.GetMvcListItems(true);
        }

        private void PrepareDetailModel(InwardDetailModel model)
        {
            var whItems = _wareHouseItemService.GetAll(true);
            var units = _unitService.GetAll(true);
            var orgs = _organizationService.GetAll(true);
            var userModels = _userModelHelper.GetAll(true);
            var stations = _stationService.GetAll();
            var projects = _projectService.GetAll(true);
            var customers = _customerService.GetAll(true);

            if (!string.IsNullOrWhiteSpace(model.ItemId))
                model.ItemName = whItems.FirstOrDefault(w => w.Id == model.ItemId)?.Name;
            if (!string.IsNullOrWhiteSpace(model.UnitId))
                model.UnitName = units.FirstOrDefault(w => w.Id == model.UnitId)?.UnitName;
            if (!string.IsNullOrWhiteSpace(model.DepartmentId))
                model.DepartmentName = orgs.FirstOrDefault(w => w.Id == int.Parse(model.DepartmentId))?.Name;
            if (!string.IsNullOrWhiteSpace(model.EmployeeId))
                model.EmployeeName = userModels.FirstOrDefault(w => w.Id == model.EmployeeId)?.FullName;
            if (!string.IsNullOrWhiteSpace(model.StationId))
                model.StationName = stations.FirstOrDefault(w => w.id == int.Parse(model.StationId))?.ten_tram;
            if (!string.IsNullOrWhiteSpace(model.ProjectId))
                model.ProjectName = projects.FirstOrDefault(w => w.Id == int.Parse(model.ProjectId))?.ProjectName;
            if (!string.IsNullOrWhiteSpace(model.CustomerId))
                model.CustomerName = customers.FirstOrDefault(w => w.Id == int.Parse(model.CustomerId))?.FullName;

            model.AvailableItems = whItems
                .Where(w => !w.Inactive || w.Id.Equals(model.ItemId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"[{s.Code}] {s.GetLocalized(x => x.Name)}"
                }).ToList();
            model.AvailableUnits = units
                .Where(w => !w.Inactive || w.Id.Equals(model.UnitId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.GetLocalized(x => x.UnitName)
                }).ToList();
            model.AvailableOrganizations = orgs
                .Where(w => w.IsActive || w.Id.ToString().Equals(model.DepartmentId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"[{s.Code}] {s.Name}"
                }).ToList();
            model.AvailableUsers = userModels
                .Where(w => w.Active || w.Id.Equals(model.EmployeeId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FullName} ({s.UserName})"
                }).ToList();
            model.AvailableStations = _stationService.GetAll()
                .Select(s => new SelectListItem
                {
                    Value = s.id.ToString(),
                    Text = $"[{s.ma_tram}] {s.ten_tram}"
                }).ToList();
            model.AvailableProjects = _projectService.GetAll(true)
                .Where(w => w.IsActive || w.Id.ToString().Equals(model.ProjectId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"[{s.ProjectCode}] {s.ProjectName}"
                }).ToList();
            model.AvailableCustomers = _customerService.GetAll(true)
                .Where(w => w.IsActive || w.Id.ToString().Equals(model.CustomerId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FullName} ({s.UserName})"
                }).ToList();
            model.AvailableAccObject = _service.GetMvcListItems(true);

        }

        #endregion
    }
}
