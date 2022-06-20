using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using VTQT.Web.Framework.Security;


namespace VTQT.Web.Asset.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class AssetOfficeController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public AssetOfficeController(IWorkContext workContext)
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
            AssetSearchModel searchModel = new AssetSearchModel();
            var model = new AssetModel();
            Task getCategories = AssetHelper.GetAvailableAssetCategories(model);
            Task getDurations = AssetHelper.GetAvailableDurations(model);
            Task getUsers = AssetHelper.GetAvailableUsers(model);            
            
            var res = await ApiHelper
                .ExecuteAsync<AssetOfficeModel>("/asset-office/create", null, Method.GET, ApiHosts.Asset);
            var resLastSelected = await ApiHelper
                .ExecuteAsync<string>($"/organization/get-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AssetOffice", null, Method.GET, ApiHosts.Master);

            await Task.WhenAll(new List<Task> { getCategories, getDurations, getUsers });

            ViewData["categories"] = model.AvailableCategories;
            ViewData["assetStatus"] = res.data?.AvailableAssetStatus;
            ViewData["durations"] = model.AvailableDurations;
            ViewData["employees"] = model.AvailableUsers;

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
                .ExecuteAsync<AssetOfficeModel>("/asset-office/create", null, Method.GET, ApiHosts.Asset);
            var resLastSelected = await ApiHelper
                .ExecuteAsync<string>($"/organization/get-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AssetOffice", null, Method.GET, ApiHosts.Master);

            var model = res.data;

            if (model != null)
            {
                Task getDurations = AssetHelper.GetAvailableDurations(model);
                Task getUnits = AssetHelper.GetAvailableUnits(model);
                Task getUsers = AssetHelper.GetAvailableUsers(model);

                await Task.WhenAll(new List<Task> { getDurations, getUnits, getUsers });
                ViewData["users"] = model.AvailableUsers;
                model.CreatedBy = _workContext.UserId;
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.AllocationDate = model.AllocationDate?.ToLocalTime();
                model.ModifiedDate = model.ModifiedDate.ToLocalTime();

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
        public async Task<IActionResult> Create(AssetOfficeModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            await AssetHelper.GetAvailableData(model);

            if (!string.IsNullOrEmpty(model.SelectedDepreciationUnit))
            {
                int.TryParse(model.SelectedDepreciationUnit, out int depreciationUnit);
                model.DepreciationUnit = depreciationUnit;
            }

            if (!string.IsNullOrEmpty(model.SelectedWarrantyUnit))
            {
                int.TryParse(model.SelectedWarrantyUnit, out int warrantyUnit);
                model.WarrantyUnit = warrantyUnit;
            }

            if (string.IsNullOrEmpty(model.Code))
            {
                var code = await ApiHelper.ExecuteAsync("/generate-code/get", new { tableName = nameof(Core.Domain.Asset.Asset) }, Method.GET, ApiHosts.Master);
                model.Code = code.data;
            }
            
            model.AssetType = (int)AssetType.Office;
            model.CreatedBy = model.ModifiedBy = _workContext.UserId;

            AssetHelper.UpdatePropertyNameAvailable(model);

            var res = await ApiHelper
                .ExecuteAsync("/asset-office/create", model, Method.POST, ApiHosts.Asset);
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
                .ExecuteAsync<AssetOfficeModel>("/asset-office/edit", new { id = id }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                Task getDurations = AssetHelper.GetAvailableDurations(model);
                Task getUnits = AssetHelper.GetAvailableUnits(model);
                Task getUsers = AssetHelper.GetAvailableUsers(model);

                await Task.WhenAll(new List<Task> { getDurations, getUnits, getUsers });
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.AllocationDate = model.AllocationDate?.ToLocalTime();
                model.ModifiedDate = model.ModifiedDate.ToLocalTime();
                ViewData["users"] = model.AvailableUsers;

                AssetHelper.ConvertNumberStr(model);
            }

            var searchModel = new HistorySearchModel
            {
                AssetId = id,
                PageIndex = 1,
                PageSize = 1000
            };
            var resHistory = await ApiHelper
                .ExecuteAsync<List<HistoryModel>>("/history/get", searchModel, Method.GET, ApiHosts.Asset);
            ViewData["histories"] = resHistory.data;

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AssetOfficeModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            await AssetHelper.GetAvailableData(model);

            if (!string.IsNullOrEmpty(model.SelectedDepreciationUnit))
            {
                int.TryParse(model.SelectedDepreciationUnit, out int depreciationUnit);
                model.DepreciationUnit = depreciationUnit;
            }

            if (!string.IsNullOrEmpty(model.SelectedWarrantyUnit))
            {
                int.TryParse(model.SelectedWarrantyUnit, out int warrantyUnit);
                model.WarrantyUnit = warrantyUnit;
            }
            model.AssetType = (int)AssetType.Office;
            model.ModifiedBy = _workContext.UserId;

            AssetHelper.UpdatePropertyNameAvailable(model);

            var res = await ApiHelper
                .ExecuteAsync("/asset-office/edit", model, Method.POST, ApiHosts.Asset);
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
                .ExecuteAsync<AssetOfficeModel>("/asset-office/details", new { id = id }, Method.GET, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            if (model != null)
            {
                await AssetHelper.GetAvailableData(model);
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.AllocationDate = model.AllocationDate?.ToLocalTime();
                model.ModifiedDate = model.ModifiedDate.ToLocalTime();
                ViewData["users"] = model.AvailableUsers;
            }

            model.SelectedDepreciationUnit = model.DepreciationUnit.ToString();
            model.SelectedWarrantyUnit = model.WarrantyUnit.ToString();

            var searchModel = new HistorySearchModel
            {
                AssetId = id,
                PageIndex = 1,
                PageSize = 1000
            };
            var resHistory = await ApiHelper
                .ExecuteAsync<List<HistoryModel>>("/history/get", searchModel, Method.GET, ApiHosts.Asset);
            ViewData["histories"] = resHistory.data;

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
                .ExecuteAsync("/asset-office/deletes", ids, Method.POST, ApiHosts.Asset);
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
                                             AssetSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.AssetType = AssetType.Office;

            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper
                .ExecuteAsync<List<AssetOfficeModel>>("/asset-office/get", searchModel, Method.GET, ApiHosts.Asset);
            var data = res.data;

            if (!string.IsNullOrEmpty(searchModel.OrganizationId))
            {
                var resLastSelected = await ApiHelper
                    .ExecuteAsync<string>($"/organization/update-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AssetOffice&departmentId={searchModel.OrganizationId}", null, Method.POST, ApiHosts.Master);
            }

            if (data?.Count > 0)
            {
                data.ForEach(item =>
                {
                    item.SelectedDepreciationUnit = item.DepreciationUnit.ToString();
                    item.SelectedWarrantyUnit = item.WarrantyUnit.ToString();
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
                ExecuteAsync<AssetDecreasedModel>("/asset-decreased/create", new { assetType = (int)AssetType.Office }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                model.EmployeeId = _workContext.UserId;
                model.DecreaseDate = DateTime.UtcNow.ToLocalTime();
                if (!string.IsNullOrEmpty(id) && id != "undefined")
                {
                    model.AssetId = id;
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
        public async Task<IActionResult> Recall(AssetDecreasedModel model)
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

            var resWarehouse = await ApiHelper.
                ExecuteAsync<WareHouseModel>("/warehouse/get-by-code", new { code = model.WareHouseCode }, Method.GET, ApiHosts.Warehouse);

            var warehouse = resWarehouse.data;

            if (warehouse != null)
            {
                model.WareHouseName = warehouse.Name;
            }

            var res = await ApiHelper.ExecuteAsync("/asset-decreased/create", model, Method.POST, ApiHosts.Asset);

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
            var res = await ApiHelper.ExecuteAsync<MaintenanceModel>("/maintenance/create-maintenance", ids, Method.POST, ApiHosts.Asset);

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

            var res = await ApiHelper.ExecuteAsync("/maintenance/create", model, Method.POST, ApiHosts.Asset);
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
            searchModel.AssetTypeMaintenanceDetail = AssetType.Office;

            var res = await ApiHelper.ExecuteAsync<List<AssetModel>>("/maintenance/detail-create", searchModel, Method.GET, ApiHosts.Asset);

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
                ExecuteAsync<AssetTransferenceModel>($"/asset-transference/create?assetType={(int)AssetType.Office}&id={id}", null, Method.GET, ApiHosts.Asset);

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
        public async Task<IActionResult> Transference(AssetTransferenceModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            if (!string.IsNullOrEmpty(model.ToCustomerCode))
            {
                var resCustomer = await ApiHelper
                    .ExecuteAsync<CustomerModel>("/customer/get-by-code", new { code = model.ToCustomerCode }, Method.GET, ApiHosts.Master);

                var customer = resCustomer.data;

                if (customer != null)
                {
                    model.CustomerName = $"[{customer.Code}] {customer.Name}";
                }
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

            var res = await ApiHelper.ExecuteAsync("/asset-transference/create", model, Method.POST, ApiHosts.Asset);

            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }
        #endregion

        #region Export        
        private string CheckNullString(string check, IList<SelectListItem> selectItems)
        {
            if (string.IsNullOrEmpty(check))
                return "";
            return selectItems.FirstOrDefault(x => x.Value.Equals(check)).Text;
        }

        public ActionResult DownloadExcel(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                return null;
            }
        }

        public async Task<ActionResult> GetExcelReport(AssetSearchModel model)
        {
            model.AssetType = AssetType.Office;
            var res = await ApiHelper.ExecuteAsync<List<AssetOfficeModel>>("/asset-office/get", model, Method.GET, ApiHosts.Asset);
            var data = res?.data;
            var stt = 1;
            var models = new List<AssetOfficeExportModel>();
            if (data?.Count > 0)
            {
                var modelc = new AssetModel();
                await AssetHelper.GetAvailableAssetCategories(modelc);
                await AssetHelper.GetAvailableDurations(modelc);
                var resType = await ApiHelper.ExecuteAsync<AssetOfficeModel>("/asset-office/create", null, Method.GET, ApiHosts.Asset);

                var checkName = modelc.AvailableCategories;
                var checkNameStatus = resType.data?.AvailableAssetStatus;
                foreach (var order in data)
                {
                    var m = new AssetOfficeExportModel
                    {
                        STT = stt,
                        CreatedDate = order.CreatedDate.ToString("0:dd/MM/yyyy"),
                        Code = order.Code,
                        Name = order.Name,
                        Type = CheckNullString(order?.CategoryId, checkName),
                        OriginQuantity = order.OriginQuantity,
                        RecallQuantity = order.RecallQuantity,
                        BrokenQuantity = order.BrokenQuantity,
                        SoldQuantity = order.SoldQuantity,
                        MinusQuantity = order.OriginQuantity - order.RecallQuantity - order.BrokenQuantity - order.SoldQuantity,
                        UnitName = order.UnitName,
                        OrganizationUnitName = order.OrganizationUnitName,
                        CurrentUsageStatus = CheckNullString(order?.CurrentUsageStatus, checkNameStatus),
                        DateBaoDuong = order.MaintenancedDate?.ToString("0:dd/MM/yyyy")

                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("Asset");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết danh sách tài sản";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/AssetOffice_vi.xlsx");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();

            var handler = Guid.NewGuid().ToString();

            var ms = new MemoryStream();
            wb.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
            ms.Seek(0, SeekOrigin.Begin);
            TempData[handler] = ms.ToArray();

            var fileDownloadName = "bao_cao_chi_tiet_danh_sach_tai_san.xlsx";

            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", fileDownloadName);

            //  return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }

        public async Task<IActionResult> GetExcelData(AssetSearchModel searchModel)
        {
            searchModel.AssetType = AssetType.Office;

            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper
                .ExecuteAsync<List<AssetOfficeModel>>("/asset-office/get-excel", searchModel, Method.GET, ApiHosts.Asset);
 
            var data = res.data;

            if (data?.Count > 0)
            {
                var assetOffice = new DataTable();
                assetOffice.Columns.Add(nameof(AssetExcelModel.Stt), typeof(int));
                assetOffice.Columns.Add(nameof(AssetExcelModel.AllocationDate), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.Code), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.Name), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.CategoryName), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.OriginQuantity), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.RecallQuantity), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.BrokenQuantity), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.SoldQuantity), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.BalanceQuantity), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.UnitName), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.OrganizationUnitName), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.EmployeeName), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.CurrentUsageStatus), typeof(string));
                assetOffice.Columns.Add(nameof(AssetExcelModel.MaintenancedDate), typeof(string));

                var info = new DataTable();
                info.Columns.Add("title", typeof(string));

                info.Rows.Add("Danh sách tài sản hành chính");
                info.TableName = "info";
                var stt = 1;
                data.ForEach(x =>
                {                                        
                    var m = new AssetExcelModel
                    {
                        Stt = stt++,
                        AllocationDate = x.AllocationDate?.ToString("dd/MM/yyyy"),
                        Code = x.Code,
                        Name = x.Name,
                        OriginQuantity = x.OriginQuantity,
                        RecallQuantity = x.RecallQuantity,
                        BrokenQuantity = x.BrokenQuantity,
                        SoldQuantity = x.SoldQuantity,
                        BalanceQuantity = x.OriginQuantity - x.RecallQuantity - x.BrokenQuantity - x.SoldQuantity,
                        UnitName = x.UnitName,
                        OrganizationUnitName = x.OrganizationUnitName,
                        EmployeeName = x.EmployeeName,
                        MaintenancedDate = x.MaintenancedDate?.ToString("dd/MM/yyyy")
                    };

                    if (!string.IsNullOrEmpty(x.CategoryId) && !string.IsNullOrEmpty(x.CurrentUsageStatus))
                    {
                        Task<XBaseResult<AssetCategoryModel>> getCategory = ApiHelper.ExecuteAsync<AssetCategoryModel>("asset-category/get-by-id", new { id = x.CategoryId }, Method.GET, ApiHosts.Asset);
                        Task<XBaseResult<UsageStatusModel>> getUsageStatus = ApiHelper.ExecuteAsync<UsageStatusModel>("usage-status/get-by-id", new { id = x.CurrentUsageStatus }, Method.GET, ApiHosts.Asset);

                        Task.WhenAll(new List<Task> { getCategory, getUsageStatus });

                        var category = getCategory.Result.data;
                        var usage = getUsageStatus.Result.data;

                        m.CategoryName = category?.Name;
                        m.CurrentUsageStatus = usage?.Description;
                    }

                    assetOffice.Rows.Add(m.Stt, m.AllocationDate, m.Code,
                                         m.Name, m.CategoryName, m.OriginQuantity,
                                         m.RecallQuantity, m.BrokenQuantity, m.SoldQuantity,
                                         m.BalanceQuantity, m.UnitName, m.OrganizationUnitName,
                                         m.EmployeeName, m.CurrentUsageStatus, m.MaintenancedDate);                    
                });
                assetOffice.TableName = "assetOffice";
                var ds = new DataSet();
                ds.Tables.Add(assetOffice);
                ds.Tables.Add(info);
                var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/AssetOffice.xlsx");
                var bytes = TemplateExcel.FillReport("AssetOffice.xlsx", tmpPath, ds, new string[] { "{", "}" });             
                
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ds_taisanhc.xlsx");
            }

            return Ok();
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

            var res = await ApiHelper.ExecuteAsync<AssetOfficeModel>("/asset-office/get-by-id", new { id = id }, Method.GET, ApiHosts.Asset);

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

            var res = await ApiHelper.ExecuteAsync<AssetOfficeModel>("/asset-office/get-by-id", new { id = assetId }, Method.GET, ApiHosts.Asset);

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

            var res = await ApiHelper.ExecuteAsync<AssetOfficeModel>("/asset-office/get-by-id", new { id = assetId }, Method.GET, ApiHosts.Asset);

            var assetModel = res.data;

            assetModel.BalanceQuantity = assetModel.OriginQuantity - assetModel.BrokenQuantity - assetModel.RecallQuantity - assetModel.SoldQuantity;

            if (assetModel.BalanceQuantity < recallQuantity || assetModel.BalanceQuantity < 0)
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
        /// Lấy dữ liệu loại tài sản
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCategories(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<AssetCategoryModel>>(
                $"/asset-category/get-available?assetType={(int)AssetType.Office}", null, Method.GET, ApiHosts.Asset);

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
        #endregion        
    }
}
