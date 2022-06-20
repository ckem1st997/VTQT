﻿using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Asset.Areas.Admin.Helper;
using VTQT.Web.Asset.Models;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Web.Asset.Areas.Admin.Controllers
{
    public class AssetAttachmentController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public AssetAttachmentController(IWorkContext workContext)
        {
            _workContext = workContext;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            AssetAttachmentSearchModel searchModel = new AssetAttachmentSearchModel();
            var model = new AssetModel();
            Task getCategories = AssetHelper.GetAvailableAssetCategories(model);
            Task getProjects = AssetHelper.GetAvailableProject(model);
            var res = await ApiHelper
                .ExecuteAsync<AssetAttachmentModel>("/asset-attachment/create", null, Method.GET, ApiHosts.Asset);
            var resLastSelected = await ApiHelper.
                ExecuteAsync<string>($"/organization/get-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AssetAttachment", null, Method.GET, ApiHosts.Master);

            await Task.WhenAll(new List<Task> { getCategories, getProjects });

            ViewData["categories"] = model.AvailableCategories;
            ViewData["assetStatus"] = res.data?.AvailableAssetStatus;
            ViewData["projects"] = model.AvailableProjects;

            if (!string.IsNullOrEmpty(resLastSelected.data))
            {
                searchModel.OrganizationId = resLastSelected.data;
            }

            return View(searchModel);
        }

        /// <summary>
        /// Tạo mới tài sản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<AssetAttachmentModel>("/asset-attachment/create", null, Method.GET, ApiHosts.Asset);
            var resLastSelected = await ApiHelper
                .ExecuteAsync<string>($"/organization/get-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AssetAttachment", null, Method.GET, ApiHosts.Master);

            var model = res.data;

            if (model != null)
            {
                var assetModel = new AssetModel();
                Task getUnits = AssetHelper.GetAvailableUnits(assetModel);
                Task getUsers = AssetHelper.GetAvailableUsers(assetModel);

                await Task.WhenAll(new List<Task> { getUnits, getUsers });
                ViewData["users"] = assetModel.AvailableUsers;
                model.CreatedBy = _workContext.UserId;
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.AllocationDate = model.AllocationDate.ToLocalTime();
                model.ModifiedDate = model.ModifiedDate.ToLocalTime();

                model.AvailableUnits.AddRange(assetModel.AvailableUnits);
                model.AvailableUsers.AddRange(assetModel.AvailableUsers);

                if (!string.IsNullOrEmpty(resLastSelected.data))
                {
                    model.OrganizationUnitId = resLastSelected.data;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Gọi Api tạo mới tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetAttachmentModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            var assetModel = new AssetModel();
            await AssetHelper.GetAvailableData(assetModel);

            if (string.IsNullOrEmpty(model.Code))
            {
                var code = await ApiHelper.ExecuteAsync("/generate-code/get", new { tableName = nameof(AssetAttachment) }, Method.GET, ApiHosts.Master);
                model.Code = code.data;
            }
            
            model.AssetType = (int)AssetType.Project;
            model.CreatedBy = model.ModifiedBy = _workContext.UserId;

            AssetHelper.UpdatePropertyNameAvailable(assetModel);
            model.WareHouseItemName = assetModel.WareHouseItemName;
            model.UnitName = assetModel.UnitName;
            model.StationName = assetModel.StationName;
            model.ProjectName = assetModel.ProjectName;
            model.OrganizationUnitName = assetModel.OrganizationUnitName;
            model.EmployeeName = assetModel.EmployeeName;

            var res = await ApiHelper
                .ExecuteAsync("/asset-attachment/create", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult
            {
                data = res.data
            });
        }

        /// <summary>
        /// Chỉnh sửa tài sản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<AssetAttachmentModel>("/asset-attachment/edit", new { id = id }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                var assetModel = new AssetModel();
                Task getUnits = AssetHelper.GetAvailableUnits(assetModel);
                Task getUsers = AssetHelper.GetAvailableUsers(assetModel);

                await Task.WhenAll(new List<Task> { getUnits, getUsers });
                ViewData["users"] = assetModel.AvailableUsers;

                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.AllocationDate = model.AllocationDate.ToLocalTime();
                model.ModifiedDate = model.ModifiedDate.ToLocalTime();
                model.AvailableUnits = assetModel.AvailableUnits;
                model.AvailableUsers = assetModel.AvailableUsers;
            }

            var searchModel = new HistorySearchModel
            {
                AssetAttachmentId = id,
                PageIndex = 1,
                PageSize = 1000
            };
            var resHistory = await ApiHelper
                .ExecuteAsync<List<HistoryModel>>("/history/get-history-attachment", searchModel, Method.GET, ApiHosts.Asset);
            ViewData["histories"] = resHistory.data ?? new List<HistoryModel>();

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AssetAttachmentModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var assetModel = new AssetModel();
            await AssetHelper.GetAvailableData(assetModel);

            model.AssetType = (int)AssetType.Project;
            model.ModifiedBy = _workContext.UserId;

            AssetHelper.UpdatePropertyNameAvailable(assetModel);
            model.WareHouseItemName = assetModel.WareHouseItemName;
            model.UnitName = assetModel.UnitName;
            model.StationName = assetModel.StationName;
            model.ProjectName = assetModel.ProjectName;
            model.OrganizationUnitName = assetModel.OrganizationUnitName;
            model.EmployeeName = assetModel.EmployeeName;

            var res = await ApiHelper
                .ExecuteAsync("/asset-attachment/edit", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy chi tiết  tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<AssetAttachmentModel>("/asset-attachment/details", new { id = id }, Method.GET, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            if (model != null)
            {
                var assetModel = new AssetModel();
                await AssetHelper.GetAvailableData(assetModel);

                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.AllocationDate = model.AllocationDate.ToLocalTime();
                model.ModifiedDate = model.ModifiedDate.ToLocalTime();
                model.AvailableCategories = assetModel.AvailableCategories;
                model.AvailableItems = assetModel.AvailableItems;
                model.AvailableOrganizations = assetModel.AvailableOrganizations;
                model.AvailableProjects = assetModel.AvailableProjects;
                model.AvailableUnits = assetModel.AvailableUnits;
                ViewData["users"] = assetModel.AvailableUsers;
            }

            var searchModel = new HistorySearchModel
            {
                AssetAttachmentId = id,
                PageIndex = 1,
                PageSize = 1000
            };
            var resHistory = await ApiHelper
                .ExecuteAsync<List<HistoryModel>>("/history/get-history-attachment", searchModel, Method.GET, ApiHosts.Asset);
            ViewData["histories"] = resHistory.data ?? new List<HistoryModel>();

            return View(model);
        }

        /// <summary>
        /// Gọi Api xóa tài sản
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/asset-attachment/deletes", ids, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }
        #endregion

        #region List
        /// <summary>
        /// Gọi Api lấy cấu trúc cây tổ chức
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<OrganizationTreeModel>>("/organization/get-tree", null, Method.POST, ApiHosts.Master);

            return Ok(res);
        }

        /// <summary>
        /// Lấy danh sách loại tài sản phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             AssetAttachmentSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.AssetType = AssetType.Project;

            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper
                .ExecuteAsync<List<AssetAttachmentModel>>("/asset-attachment/get", searchModel, Method.GET, ApiHosts.Asset);
            var data = res.data;

            if (!string.IsNullOrEmpty(searchModel.OrganizationId))
            {
                var resLastSelected = await ApiHelper
                    .ExecuteAsync<string>($"/organization/update-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AssetAttachment&departmentId={searchModel.OrganizationId}", null, Method.POST, ApiHosts.Master);
            }

            if (data?.Count > 0)
            {
                data.ForEach(item =>
                {
                    item.BalanceQuantity = item.OriginQuantity - item.RecallQuantity - item.SoldQuantity - item.BrokenQuantity;
                });
            }

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }
        #endregion

        #region Recall
        /// <summary>
        /// Khởi tạo đối tượng thu hồi tài sản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Recall(string id)
        {
            var res = await ApiHelper.
                ExecuteAsync<AssetAttachmentDecreasedModel>("/attachment-decreased/create", new { assetType = (int)AssetType.Project }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                model.EmployeeId = _workContext.UserId;
                model.DecreaseDate = DateTime.UtcNow.ToLocalTime();
                if (!string.IsNullOrEmpty(id) && id != "undefined")
                {
                    model.AssetAttachmentId = id;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Thu hồi tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recall(AssetAttachmentDecreasedModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.CreatedBy = model.ModifiedBy = model.EmployeeId = _workContext.UserId;
            var resUser = await ApiHelper.
                ExecuteAsync<UserModel>("/user/get-by-id", new { id = model.EmployeeId }, Method.GET, ApiHosts.Master);

            var user = resUser.data;

            if (user != null)
            {
                model.EmployeeName = $"{user.FullName} - {user.Email} ({user.UserName})";
            }

            if (!string.IsNullOrEmpty(model.WareHouseCode))
            {
                var resWarehouse = await ApiHelper.
                ExecuteAsync<WareHouseModel>("/warehouse/get-by-code", new { code = model.WareHouseCode }, Method.GET, ApiHosts.Warehouse);

                var warehouse = resWarehouse.data;

                if (warehouse != null)
                {
                    model.WareHouseName = warehouse.Name;
                }
            }            

            var res = await ApiHelper.ExecuteAsync("/attachment-decreased/create", model, Method.POST, ApiHosts.Asset);

            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { success = true });
        }
        #endregion

        #region Maintenance
        /// <summary>
        /// Khởi tạo đối tượng bảo dưỡng tài sản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Maintenance([ModelBinder(typeof(CsvModelBinder<string>))] IEnumerable<string> ids)
        {
            var res = await ApiHelper.ExecuteAsync<MaintenanceModel>("/attachment-maintenance/create-maintenance", ids, Method.POST, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                model.EmployeeId = _workContext.UserId;
                model.MaintenancedDate = model.MaintenancedDate.ToLocalTime();
                model.MaintenanceDetailsString = JsonConvert.SerializeObject(model.MaintenanceDetails);
            }

            var assetModel = new AssetModel();
            await AssetHelper.GetAvailableData(assetModel);
            ViewData["categories"] = assetModel.AvailableCategories;

            return View(model);
        }

        /// <summary>
        /// Bảo dưỡng/ sửa chữa tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Maintenance(MaintenanceModel model, IEnumerable<MaintenanceDetailModel> details)
        {
            model.MaintenanceDetails = details.ToList();

            if (!string.IsNullOrEmpty(model.Action))
            {
                var actions = Enum.GetValues(typeof(AssetAction));
                int.TryParse(model.Action, out int action);
                foreach (AssetAction act in actions)
                {
                    if (action == (int)act)
                    {
                        model.Action = act.GetEnumDescription();
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.EmployeeId))
            {
                var resUser = await ApiHelper.ExecuteAsync<UserModel>("/user/get-by-id", new { id = model.EmployeeId }, Method.GET, ApiHosts.Master);
                var user = resUser.data;

                if (user != null)
                {
                    model.EmployeeName = $"{user.FullName} - {user.Email} ({user.UserName})";
                }
            }

            var res = await ApiHelper.ExecuteAsync("/attachment-maintenance/create", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo chi tiết bảo dưỡng
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MaintenanceAddItem()
        {
            var model = new AssetModel();
            await AssetHelper.GetAvailableData(model);
            ViewData["categories"] = model.AvailableCategories;
            ViewData["organizations"] = model.AvailableOrganizations;
            ViewData["stations"] = model.AvailableStations;
            ViewData["projects"] = model.AvailableProjects;
            return View();
        }

        /// <summary>
        /// Lấy danh sách chi tiết bảo dưỡng
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetDetailsCreate([DataSourceRequest] DataSourceRequest request,
                                                           MaintenanceDetailSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.AssetTypeMaintenanceDetail = AssetType.Project;

            var res = await ApiHelper.ExecuteAsync<List<AssetModel>>("/attachment-maintenance/detail-create", searchModel, Method.GET, ApiHosts.Asset);

            var data = res.data;
            if (data?.Count > 0)
            {
                data.ForEach(x =>
                {
                    x.BalanceQuantity = x.OriginQuantity - x.RecallQuantity - x.SoldQuantity;
                });
            }

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };

            return Ok(result);
        }
        #endregion

        #region Transference
        /// <summary>
        /// Khởi tạo đối tượng điều chuyển
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Transference(string id)
        {
            var res = await ApiHelper.
                ExecuteAsync<AssetAttachmentTransferenceModel>($"/attachment-transference/create?assetType={(int)AssetType.Project}&assetAttachmentId={id}", null, Method.GET, ApiHosts.Asset);

            var model = res.data;
            var assetModel = new AssetModel();

            await AssetHelper.GetAvailableData(assetModel);

            if (model != null)
            {
                model.AvailableCustomers = assetModel.AvailableCustomers;
                model.AvailableOrganizations = assetModel.AvailableOrganizations;
                model.AvailableProjects = assetModel.AvailableProjects;
                model.AvailableStations = assetModel.AvailableStations;

                model.Dispatcher = _workContext.UserId;
                model.TransferedDate = DateTime.Now;
            }

            return View(model);
        }

        /// <summary>
        /// Điều chuyển tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transference(AssetAttachmentTransferenceModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            if (!string.IsNullOrEmpty(model.ToStationCode))
            {
                var resStation = await ApiHelper
                    .ExecuteAsync<Station>("/station/get-by-code", new { code = model.ToStationCode }, Method.GET, ApiHosts.Asset);

                var station = resStation.data;

                if (station != null)
                {
                    model.StationName = $"[{station.Code}] {station.Name}";
                }
            }

            if (!string.IsNullOrEmpty(model.ToProjectCode))
            {
                var resProject = await ApiHelper
                    .ExecuteAsync<ProjectModel>("/project/get-by-code", new { code = model.ToProjectCode }, Method.GET, ApiHosts.Master);

                var project = resProject.data;

                if (project != null)
                {
                    model.ProjectName = $"[{project.Code}] {project.Name}";
                }
            }

            if (!string.IsNullOrEmpty(model.ToEmployeeId))
            {
                var resEmployee = await ApiHelper
                    .ExecuteAsync<UserModel>("/user/get-by-id", new { id = model.ToEmployeeId }, Method.GET, ApiHosts.Master);

                var employee = resEmployee.data;

                if (employee != null)
                {
                    model.EmployeeName = $"{employee.FullName} - {employee.Email} ({employee.UserName})";
                }
            }

            if (!string.IsNullOrEmpty(model.ToOrganizationId))
            {
                var resOrganization = await ApiHelper
                    .ExecuteAsync<OrganizationModel>("/organization/get-by-id", new { id = model.ToOrganizationId }, Method.GET, ApiHosts.Master);

                var organization = resOrganization.data;

                if (organization != null)
                {
                    model.OrganizationName = $"[{organization.Code}] {organization.Name}";
                }
            }

            var res = await ApiHelper.ExecuteAsync("/attachment-transference/create", model, Method.POST, ApiHosts.Asset);

            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }
        #endregion

        #region Asset

        public ActionResult AssetAddItem()
        {
            var model = new AssetAttachmentModel();
            return View(model);
        }

        public async Task<IActionResult> GetAssetAttachments(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok();
            }



            return Ok(new XBaseResult
            {

            });
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Lấy dữ liệu id loại tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetCategoryIdByAssetId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var res = await ApiHelper.ExecuteAsync<AssetProjectModel>("/asset-project/get-by-id", new { id = id }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                return model.CategoryId;
            }

            return null;
        }

        /// <summary>
        /// Lấy dữ liệu id đơn vị tính
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetUnitIdByWareHouseItemCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }

            var res = await ApiHelper.ExecuteAsync<WareHouseItemModel>("/warehouse-item/get-by-code", new { code = code }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            if (model != null)
            {
                return model.UnitId;
            }

            return null;
        }

        /// <summary>
        /// Lấy dữ liệu id employee và organization với asset tương ứng
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetDepartmentAndEmployeeByAsset(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                return null;
            }

            var res = await ApiHelper.ExecuteAsync<AssetProjectModel>("/asset-project/get-by-id", new { id = assetId }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                var ids = new List<string>
                {
                    model.OrganizationUnitId,
                    model.EmployeeId,
                    model.StationCode,
                    model.ProjectCode,
                    model.CustomerCode,
                };

                return JsonConvert.SerializeObject(ids);
            }

            return null;
        }

        /// <summary>
        /// So sánh số lượng thu hồi với số lượng còn lại
        /// </summary>
        /// <param name="recallQuantity"></param>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CompareRecallQuantity(decimal recallQuantity, string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                return Ok(new XBaseResult
                {
                    success = true
                });
            }

            var res = await ApiHelper.ExecuteAsync<AssetProjectModel>("/asset-attachment/get-by-id", new { id = assetId }, Method.GET, ApiHosts.Asset);

            var assetAttachment = res.data;

            assetAttachment.BalanceQuantity = assetAttachment.OriginQuantity - assetAttachment.BrokenQuantity - assetAttachment.RecallQuantity - assetAttachment.SoldQuantity;

            if (assetAttachment.BalanceQuantity < recallQuantity || assetAttachment.BalanceQuantity < 0)
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }
            else
            {
                return Ok(new XBaseResult
                {
                    success = true
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu items
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetItems(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>(
                "warehouse-item/get-select", null, Method.GET, ApiHosts.Warehouse);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu người đề xuất
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetUsers(string search, int page)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.FullName.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu phòng ban
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDepartments(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<OrganizationModel>>(
                "organization/get-available", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu dự án
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProjects(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<ProjectModel>>(
                "project/get-available", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCustomers(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<CustomerModel>>(
                "customer/get-available", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu loại tài sản
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCategories(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<AssetCategoryModel>>(
                $"/asset-category/get-available?assetType={(int)AssetType.Project}", null, Method.GET, ApiHosts.Asset);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Khởi tạo dữ liệu người tạo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetUser(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<UserModel>("/user/get-by-id", new { id = id }, Method.GET, ApiHosts.Master);

            var user = res.data;
            return Ok(user);
        }

        /// <summary>
        /// Khởi tạo dữ liệu phòng ban
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDepartment(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<OrganizationModel>("/organization/get-by-id", new { id = id }, Method.GET, ApiHosts.Master);

            var department = res.data;
            return Ok(department);
        }

        /// <summary>
        /// Khởi tạo dữ liệu vật tư
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetItem(string code)
        {
            var res = await ApiHelper
                .ExecuteAsync<WareHouseItemModel>("/warehouse-item/get-by-code", new { code = code }, Method.GET, ApiHosts.Warehouse);

            var item = res.data;
            return Ok(item);
        }

        /// <summary>
        /// Khởi tạo dữ liệu loại tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCategory(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<AssetCategoryModel>("/asset-category/get-by-id", new { id = id }, Method.GET, ApiHosts.Asset);

            var category = res.data;
            return Ok(category);
        }

        /// <summary>
        /// Khởi tạo dữ liệu dự án
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProject(string code)
        {
            var res = await ApiHelper
                .ExecuteAsync<ProjectModel>("/project/get-by-code", new { code = code }, Method.GET, ApiHosts.Master);

            var project = res.data;
            return Ok(project);
        }

        /// <summary>
        /// Khởi tạo dữ liệu khách hàng
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCustomer(string code)
        {
            var res = await ApiHelper
                .ExecuteAsync<CustomerModel>("/customer/get-by-code", new { code = code }, Method.GET, ApiHosts.Master);

            var customer = res.data;
            return Ok(customer);
        }
        #endregion

        #region ExportExcel
        public async Task<IActionResult> GetExcelData(AssetAttachmentSearchModel searchModel)
        {
            searchModel.AssetType = AssetType.Project;

            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper
                .ExecuteAsync<List<AssetAttachmentModel>>("/asset-attachment/get-excel", searchModel, Method.GET, ApiHosts.Asset);

            var data = res.data;

            if (data?.Count > 0)
            {
                var assetProject = new DataTable();
                assetProject.Columns.Add(nameof(AssetExcelModel.Stt), typeof(int));
                assetProject.Columns.Add(nameof(AssetExcelModel.AllocationDate), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.Code), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.Name), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.ProjectName), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.CustomerName), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.CustomerCode), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.CustomerAddress), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.CategoryName), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.OriginQuantity), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.RecallQuantity), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.BrokenQuantity), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.SoldQuantity), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.BalanceQuantity), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.UnitName), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.OrganizationUnitName), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.CurrentUsageStatus), typeof(string));
                assetProject.Columns.Add(nameof(AssetExcelModel.MaintenancedDate), typeof(string));

                var info = new DataTable();
                info.Columns.Add("title", typeof(string));

                info.Rows.Add("Danh sách tài sản đính kèm");
                info.TableName = "info";
                var stt = 1;
                data.ForEach(x =>
                {
                    var m = new AssetExcelModel
                    {
                        Stt = stt++,
                        AllocationDate = x.AllocationDate.ToString("dd/MM/yyyy"),
                        Code = x.Code,
                        Name = x.Name,
                        OriginQuantity = x.OriginQuantity,
                        RecallQuantity = x.RecallQuantity,
                        BrokenQuantity = x.BrokenQuantity,
                        SoldQuantity = x.SoldQuantity,
                        BalanceQuantity = x.OriginQuantity - x.RecallQuantity - x.BrokenQuantity - x.SoldQuantity,
                        CustomerCode = x.CustomerCode,
                        UnitName = x.UnitName,
                        OrganizationUnitName = x.OrganizationUnitName,
                        CustomerName = x.CustomerName,
                        ProjectName = x.ProjectName,
                        MaintenancedDate = x.MaintenancedDate?.ToString("dd/MM/yyyy")
                    };

                    if (!string.IsNullOrEmpty(x.CategoryId) && !string.IsNullOrEmpty(x.CurrentUsageStatus))
                    {
                        Task<XBaseResult<AssetCategoryModel>> getCategory = ApiHelper.ExecuteAsync<AssetCategoryModel>("asset-category/get-by-id", new { id = x.CategoryId }, Method.GET, ApiHosts.Asset);
                        Task<XBaseResult<UsageStatusModel>> getUsageStatus = ApiHelper.ExecuteAsync<UsageStatusModel>("usage-status/get-by-id", new { id = x.CurrentUsageStatus }, Method.GET, ApiHosts.Asset);
                        Task<XBaseResult<ProjectModel>> getProject = ApiHelper.ExecuteAsync<ProjectModel>("project/get-by-code", new { code = x.ProjectCode }, Method.GET, ApiHosts.Master);

                        Task.WhenAll(new List<Task> { getCategory, getUsageStatus, getProject });

                        var category = getCategory.Result.data;
                        var usage = getUsageStatus.Result.data;
                        var project = getProject.Result.data;

                        m.CategoryName = category?.Name;
                        m.CurrentUsageStatus = usage?.Description;
                        m.CustomerAddress = project?.Address;
                    }

                    assetProject.Rows.Add(m.Stt, m.AllocationDate, m.Code,
                                         m.Name, m.ProjectName, m.CustomerName, m.CustomerCode, m.CustomerAddress,
                                         m.CategoryName, m.OriginQuantity, m.RecallQuantity,
                                         m.BrokenQuantity, m.SoldQuantity, m.BalanceQuantity, m.UnitName,
                                         m.OrganizationUnitName, m.CurrentUsageStatus, m.MaintenancedDate);
                });
                assetProject.TableName = "assetProject";
                var ds = new DataSet();
                ds.Tables.Add(assetProject);
                ds.Tables.Add(info);
                var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/AssetProject.xlsx");
                var bytes = TemplateExcel.FillReport("AssetProject.xlsx", tmpPath, ds, new string[] { "{", "}" });

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ds_taisanduan.xlsx");
            }

            return Ok();
        }
        #endregion
    }
}
