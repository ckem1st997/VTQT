using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Asset.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class AuditInfrastructorController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public AuditInfrastructorController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion

        #region Methos

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AuditModel>("/audit-infrastructor/details", new { id = id }, Method.GET, ApiHosts.Asset);

            var stations = ApiHelper.Execute<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);

            var model = res.data;
            foreach (var item in stations.data)
            {
                var tem = new SelectListItem()
                {
                    Text = "[" + item.Code + "] " + item.Name + "",
                    Value = item.Code.ToString()
                };
                model.AvailableStations.Add(tem);
            }
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<AuditModel>("/audit-infrastructor/create", null, Method.GET, ApiHosts.Asset);
            var code = ApiHelper.Execute("/generate-code/get?tableName=Audit", null, Method.GET, ApiHosts.Master);
            var stations = ApiHelper.Execute<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);
            var model = res.data;
            model.VoucherDate = DateTime.Now;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = _workContext.UserId;
            model.VoucherCode = code.data;
            foreach (var item in stations.data)
            {
                var tem = new SelectListItem()
                {
                    Text = "[" + item.Code + "] " + item.Name + "",
                    Value = item.Code.ToString()
                };
                model.AvailableStations.Add(tem);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSave(AuditModel model, IEnumerable<AuditDetailModel> modelDetails, IEnumerable<AuditCouncilModel> auditCouncilModels)
        {
            foreach (var item in modelDetails)
            {
                if (!string.IsNullOrEmpty(item.Serial))
                {
                    var serials = item.Serial.Split(',');
                    foreach (var s in serials)
                    {
                        item.AuditDetailSerials.Add(new AuditDetailSerialModel
                        {
                            AssetId = item.ItemId,
                            Serial = s
                        });
                    }
                }
            }
            model.AuditDetails = modelDetails.ToList();
            model.AuditCouncils = auditCouncilModels.ToList();

            model.AssetType = (int)AssetType.Infrastructure;
            var res = await ApiHelper.ExecuteAsync("/audit-infrastructor/create", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult
            {
                data = model.Id
            });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AuditModel>("/audit-infrastructor/edit", new { id = id }, Method.GET, ApiHosts.Asset);
            var stations = ApiHelper.Execute<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);

            var model = res.data;
            foreach (var item in stations.data)
            {
                var tem = new SelectListItem()
                {
                    Text = "[" + item.Code + "] " + item.Name + "",
                    Value = item.Code.ToString()
                };
                model.AvailableStations.Add(tem);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSave(AuditModel model)
        {
            model.AssetType = (int)AssetType.Infrastructure;
            var res = await ApiHelper.ExecuteAsync("/audit-infrastructor/edit", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/audit-infrastructor/deletes", ids, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region AuditDetail
        public IActionResult TabAuditDetail()
        {
            var searchModel = new AuditDetailSearchModel();

            return PartialView(searchModel);
        }
        public IActionResult TabEditAuditDetail()
        {
            var searchModel = new AuditDetailSearchModel();

            return PartialView(searchModel);
        }

        public IActionResult TabDetailAuditDetail()
        {
            var searchModel = new AuditDetailSearchModel();

            return PartialView(searchModel);
        }

        public async Task<IActionResult> AddItem()
        {
            var res = await ApiHelper.ExecuteAsync<AuditDetailModel>("/audit-detail-infrastructor/detail-create", null, Method.GET, ApiHosts.Asset);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> CreateItem(string auditId)
        {
            var res = await ApiHelper.ExecuteAsync<AuditDetailModel>("/audit-detail-infrastructor/detail-create", null, Method.GET, ApiHosts.Asset);

            var model = res.data;
            model.AuditId = auditId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(AuditDetailModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/audit-detail-infrastructor/detail-create", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditItem(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AuditDetailModel>("/audit-detail-infrastructor/detail-edit", new { id = id }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(AuditDetailModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/audit-detail-infrastructor/detail-edit", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItems(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/audit-detail-infrastructor/detail-deletes", ids, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> EditListItem([Bind(Prefix = "models")] IEnumerable<AuditDetailModel> models)
        {
            foreach (var item in models)
            {
                item.AssetItemModel.SelectedDepreciationUnit = item.AssetItemModel.DepreciationUnit.ToString();
                item.AssetItemModel.SelectedWarrantyUnit = item.AssetItemModel.WarrantyUnit.ToString();
            }
            var res = await ApiHelper.ExecuteAsync("/audit-detail-infrastructor/detail-edit-list", models, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region Lists

        public async Task<IActionResult> Index()
        {
            var result = new AuditSearchModel();
            GetAvailableData();
            var resLastSelected = await ApiHelper.
               ExecuteAsync<string>($"/organization/get-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AuditInfrastructor", null, Method.GET, ApiHosts.Master);
            var resEmployees = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            var list = new List<SelectListItem>();
            foreach (var item in resEmployees.data)
            {
                var tem = new SelectListItem()
                {
                    Text = item.FullName + "-" + item.UserName,
                    Value = item.Id
                };
                list.Add(tem);
            }
            ViewData["employees"] = list;
            if (!string.IsNullOrEmpty(resLastSelected.data))
            {
                result.OrganizationId = resLastSelected.data;
            }
            return View(result);
        }

        /// <summary>
        /// Gọi Api lấy cấu trúc cây tổ chức
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<OrganizationTreeModel>>("/organization/get-tree", null, Method.POST, ApiHosts.Master);

            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request, AuditSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.AssetType = AssetType.Infrastructure;
            searchModel.BindRequest(request);
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper.ExecuteAsync<List<AuditModel>>("/audit-infrastructor/get-list-name", searchModel, Method.GET, ApiHosts.Asset);
            var data = res.data;
            if (!string.IsNullOrEmpty(searchModel.OrganizationId))
            {
                var resLastSelected = await ApiHelper
                    .ExecuteAsync<string>($"/organization/update-last-selected/?appId=6&userId={_workContext.UserId}&path=/Admin/AuditInfrastructor&departmentId={searchModel.OrganizationId}", null, Method.POST, ApiHosts.Master);
            }

            if (data?.Count > 0)
            {
                var listUser = await ApiHelper.ExecuteAsync<AuditModel>("/audit-infrastructor/create", null, Method.GET, ApiHosts.Asset);

                data.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedBy))
                    {
                        var userName = listUser.data.AvailableCreatedBy.FirstOrDefault(z => z.Value.Equals(x.CreatedBy)).Text;
                        x.CreatedBy = userName.Split("-").Length > 0 ? userName.Split("-")[0] : string.Empty;
                    }

                });
            }
            GetAvailableData();
            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Details([DataSourceRequest] DataSourceRequest request, AuditDetailSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>("/audit-detail-infrastructor/detail-get", searchModel, Method.GET, ApiHosts.Asset);
            var result = new DataSourceResult();
            if (res.data == null)
            {
                return Ok(result);
            }
            var data = res.data;
            result.Data = data;
            result.Total = res.totalCount == 0 ? res.data.Count : res.totalCount;
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Details_Read([DataSourceRequest] DataSourceRequest request, string id, string dateTime)
        {
            var res = await ApiHelper.ExecuteAsync<List<AssetModel>>($"/audit-detail-infrastructor/get-bt-stationCodeId?idStationCode={id}&dateTime={dateTime}%2023%3A59%3A59&assetType=20", null, Method.GET, ApiHosts.Asset);
            var data = res.data;
            if (data != null)
            {
                var lisst = new List<AuditDetailModel>();
                foreach (var item in data)
                {
                    if (item.OriginQuantity > 0|| item.OriginQuantity<0)
                    {
                        var tem = new AuditDetailModel()
                        {
                            ItemId = item.WareHouseItemCode,
                            ItemName = item.Name,
                            UnitName = item.UnitName,
                            Quantity = item.OriginQuantity
                        };
                        lisst.Add(tem);
                    }
                }

                var result1 = new DataSourceResult
                {
                    Data = lisst,
                    Total = lisst.Count()
                };
                return Ok(result1);
            }

            var result = new DataSourceResult
            {
                Data = data,
                Total = 0
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update_Read([DataSourceRequest] DataSourceRequest request, AuditDetailSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>("/audit-detail-infrastructor/detail-get", searchModel, Method.GET, ApiHosts.Asset);

            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AuditCouncil_Read([DataSourceRequest] DataSourceRequest request, AuditCouncilSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<AuditCouncilModel>>("/audit-council-infrastructor/get", searchModel, Method.GET, ApiHosts.Asset);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count : res.totalCount
            };
            return Ok(result);
        }
        #endregion

        #region Recall

        /// <summary>
        /// Khởi tạo đối tượng thu hồi tài sản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Recall()
        {
            var res = await ApiHelper.
                ExecuteAsync<AssetDecreasedModel>("/asset-decreased/create", new { assetType = (int)AssetType.Infrastructure }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            if (model != null)
            {
                model.EmployeeId = _workContext.UserId;
                model.DecreaseDate = DateTime.UtcNow.ToLocalTime();
            }

            return View(model);
        }

        #endregion

        #region AuditCouncil
        public IActionResult TabAuditCouncil()
        {
            var searchModel = new AuditCouncilSearchModel();

            return PartialView(searchModel);
        }

        public IActionResult TabEditAuditCouncil()
        {
            var searchModel = new AuditCouncilSearchModel();

            return PartialView(searchModel);
        }

        public IActionResult TabDetailAuditCouncil()
        {
            var searchModel = new AuditCouncilSearchModel();

            return PartialView(searchModel);
        }

        public async Task<IActionResult> AddAuditCouncil()
        {
            var res = await ApiHelper.ExecuteAsync<AuditCouncilModel>("audit-council-infrastructor/create", null, Method.GET, ApiHosts.Asset);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> CreateAuditCouncil(string auditId)
        {
            var res = await ApiHelper.ExecuteAsync<AuditCouncilModel>("/audit-council-infrastructor/create", null, Method.GET, ApiHosts.Asset);

            var model = res.data;
            model.AuditId = auditId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuditCouncil(AuditCouncilModel model)
        {
            model.EmployeeName = "trung";
            var res = await ApiHelper.ExecuteAsync("/audit-council-infrastructor/create", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditAuditCouncil(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AuditCouncilModel>("/audit-council-infrastructor/edit", new { id = id }, Method.GET, ApiHosts.Asset);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAuditCouncil(AuditCouncilModel model)
        {
            model.EmployeeName = "trung";
            var res = await ApiHelper.ExecuteAsync("/audit-council-infrastructor/edit", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAuditCouncil(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/audit-council-infrastructor/deletes", ids, Method.POST, ApiHosts.Asset);
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
        private string GetAuditLocation(List<OrganizationModel> data, string code)
        {
            if (data is null || string.IsNullOrEmpty(code))
                return "";
            return data.FirstOrDefault(x => x.Id.Equals(code)).Name;
        }

        public async Task<ActionResult> ExportOrder(AuditSearchModel model)
        {
            var fileName = "danh-sach-kiem-ke-tai-san-hanh-chinh.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<AuditModel>>("/audit-infrastructor/get-list-name", model, Method.GET, ApiHosts.Asset);
            var organizations = ApiHelper.Execute<List<OrganizationModel>>("/audit/get-organizationUnit", null, Method.GET, ApiHosts.Asset);
            var orders = res.data;


            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách kiểm kê TS hạ tầng");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Column(1).Width = 10;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(3).Width = 20;
            ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(6).Width = 20;
            ws.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(8).Width = 20;
            ws.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Ngày kiểm kê";
            ws.Cells[2, 3].Value = "Loại tài sản";
            ws.Cells[2, 4].Value = "Nơi thực hiện";
            ws.Cells[2, 5].Value = "Tên đợt kiểm kê";
            ws.Cells[2, 6].Value = "Ngày tạo";
            ws.Cells[2, 7].Value = "Người tạo";
            var i = 3;
            int team;
            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.VoucherDate.ToString("dd/MM/yyyy HH:mm");
                    ws.Cells[i, 3].Value = GetReason(out team, order.OrganizationUnitId, GetEnumDescription((AssetType)order.AssetType));
                    ws.Cells[i, 4].Value = order.AuditLocation;
                    ws.Cells[i, 5].Value = order.Description;
                    ws.Cells[i, 6].Value = order.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                    ws.Cells[i, 7].Value = order.CreatedBy;
                    i++;
                }


            // set style title

            using (var rng = ws.Cells["D1:G1"])
            {
                rng.Value = "Danh sách kiểm kê tài sản hành chính";
                rng.Merge = true;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            }

            // set style name column
            using (var rng = ws.Cells["A2:H2"])
            {
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<ActionResult> GetExcelReport(AuditSearchModel model)
        {
            model.AssetType = AssetType.Infrastructure;
            var res = await ApiHelper.ExecuteAsync<List<AuditModel>>("/audit-infrastructor/get-list-name", model, Method.GET, ApiHosts.Asset);
            var stations = ApiHelper.Execute<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);
            var data = res?.data;
            var orders1 = stations.data;
            var stt = 1;
            var models = new List<AuditExportModel>();

            var listUser = await ApiHelper.ExecuteAsync<AuditModel>("/audit-infrastructor/create", null, Method.GET, ApiHosts.Asset);
            if (data?.Count > 0)
            {
                data.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedBy))
                    {
                        var userName = listUser.data.AvailableCreatedBy.FirstOrDefault(z => z.Value.Equals(x.CreatedBy)).Text;
                        x.CreatedBy = userName.Split("-").Length > 0 ? userName.Split("-")[0] : string.Empty;
                    }
                });
            }
            GetAvailableData();
            var getAuditLocation = (IEnumerable<SelectListItem>)ViewData["inwardReason"];
            if (data?.Count > 0)
            {
                foreach (var e in data)
                {
                    var m = new AuditExportModel
                    {
                        STT = stt,
                        VoucherDate = e.VoucherDate.ToLocalTime().ToString("dd/MM/yyyy"),
                        AssetType = "Tài sản hạ tầng",
                        Description = e.Description,
                        AuditLocation = orders1.FirstOrDefault(x => x.Code.Equals(e.AuditLocation)).Name,
                        CreatedDate = e.CreatedDate.ToLocalTime().ToString("dd/MM/yyyy"),
                        CreatedBy = e.CreatedBy,
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Audit");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo kiểm kê tài sản hạ tầng";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/AuditInfrastructor_vi.xlsx");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();

            var handler = Guid.NewGuid().ToString();

            var ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);
            ms.Seek(0, SeekOrigin.Begin);
            TempData[handler] = ms.ToArray();

            var fileDownloadName = "bao_cao_kiem_ke_tai_san_ha_tang.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }
        private static string GetReason(out int team, string Type, string Show)
        {
            return int.TryParse(Type, out team) ? Show : "";
        }
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
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
        #endregion Export

        #region ImportExcel

        public IActionResult ImportExcel()
        {
            var audit = new AuditModel();
            return View(audit);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportExcel(IEnumerable<AuditModel> models)
        {
            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();
            var db = new List<AuditModel>();
            await GetAvailable();
            foreach (var item in models)
            {
                var check = (IEnumerable<SelectListItem>)ViewData["user"];
                item.CreatedBy = GetReason(check, item.CreatedBy.Trim());

                var check1 = (IEnumerable<SelectListItem>)ViewData["station"];
                item.AuditLocation = GetReason(check1, item.AuditLocation.Trim());

                var teamCreate = new List<AuditModel>();
                teamCreate.Add(item);
                countColumn++;
                var res = await ApiHelper.ExecuteAsync("/audit-infrastructor/create", item, Method.POST, ApiHosts.Asset);

                if (!res.success)
                {
                    var temError = "" + countColumn + ";" + res.message + "";
                    vs.Add(temError);
                    openWith.Add(countColumn, res.GetErrorsToHtml().Replace("<ul>", "").Replace("<li>", "").Replace("</li>", ",").Replace("</ul>", "").Replace("<span>", "").Replace("</span>", ""));
                }
            }
            TempData["DowloadFileToError"] = await GetExcelReportError(openWith);
            return Ok(new { totalErr = vs.Count(), data = vs });
        }

        public IActionResult TotalImportExcel(int sum = 0, int err = 0)
        {
            TempData["success"] = sum - err;
            TempData["err"] = err;
            TempData["Sum"] = sum;
            return View();
        }

        public async Task<ActionResult> ExporAudit()
        {
            var fileName = "danh-sach-kiem-ke-ha-tang.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách kiểm kê TS hạ tầng");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 30;
            ws.Column(2).Width = 15;
            ws.Column(4).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[1, 1].Value = "Số phiếu kiểm kê";
            ws.Cells[1, 1].AddComment("Nhập phiếu kiểm kê vào hệ thống");

            ws.Cells[1, 2].Value = "Ngày kiểm kê";
            ws.Cells[1, 2].AddComment("Nhập ngày kiểm kê vào hệ thống");

            ws.Cells[1, 3].Value = "Loại tài sản";
            ws.Cells[1, 3].AddComment("Nhập loại tài sản trên hệ thống");

            ws.Cells[1, 4].Value = "Nơi thực hiện";
            ws.Cells[1, 4].AddComment("Nhập tên đợt kiểm kê trên hệ thống");

            ws.Cells[1, 5].Value = "Tên đợt kiểm kê";
            ws.Cells[1, 5].AddComment("Nhập tên đợt kiểm kê trên hệ thống");

            ws.Cells[1, 6].Value = "Ngày tạo";
            ws.Cells[1, 6].AddComment("Nhập ngày tạo vào hệ thống");

            ws.Cells[1, 7].Value = "Người tạo";
            ws.Cells[1, 7].AddComment("Nhập tên người tạo vào phần mềm");

            ws.Cells[1, 8].Value = "Ngày sửa";
            ws.Cells[1, 8].AddComment("Nhập ngày sửa vào phần mềm");

            // set style name column
            using (var rng = ws.Cells["A1:H1"])
            {
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private static string GetReason(IEnumerable<SelectListItem> vs, string Show)
        {
            if (Show is null)
                return "";
            var check = vs.FirstOrDefault(x => x.Text.Equals(Show));
            return check is null ? "" : check.Value;
        }

        public async Task<byte[]> GetExcelReportError(Dictionary<int, string> model)
        {
            int stt = 1;
            var models = new List<AssetCategoryImportExportModel>();

            if (model?.Count > 0)
            {
                foreach (var e in model)
                {
                    var m = new AssetCategoryImportExportModel
                    {
                        STT = e.Key,
                        Name = e.Value
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Audit");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Danh sách nhập lỗi kiểm kê hạ tầng";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-auditinfrastructor.xlsx");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();

            var handler = Guid.NewGuid().ToString();

            var ms = new MemoryStream();
            wb.Save(ms, SaveFormat.Xlsx);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        private async Task GetAvailable()
        {
            var res = ApiHelper.Execute<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);

            var resWarehouseitem = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);

            var categories = new List<SelectListItem>();
            var categories1 = new List<SelectListItem>();
            var data = res.data;
            var data1 = resWarehouseitem.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Code,
                        Text = m.Code
                    };
                    categories.Add(item);
                }
            }
            if (data1?.Count > 0)
            {
                foreach (var m in data1)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.UserName
                    };
                    categories1.Add(item);
                }
            }

            categories.OrderBy(e => e.Text);
            categories1.OrderBy(e => e.Text);
            ViewData["station"] = categories;
            ViewData["user"] = categories1;
        }

        #endregion ImportExcel

        #region Inphieu
        public async Task<ActionResult> ExportDone(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<AuditModel>("/print/get-by-id-to-audit?id=" + id + "", null, Method.GET, ApiHosts.Asset);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>($"/audit-detail-infrastructor/detail-get?AuditId={id}", null, Method.GET, ApiHosts.Asset);
            var result1 = await ApiHelper.ExecuteAsync<List<AuditCouncilModel>>($"/audit-council-infrastructor/get?AuditId={id}", null, Method.GET, ApiHosts.Asset);
            var stations = ApiHelper.Execute<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);
            var orders1 = stations.data;

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || result1.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion
            var stt = 1;
            var models = new List<AuditDetailRecallModel>();
            if (result.data?.Count > 0)
            {
                foreach (var order in result.data)
                {
                    var m = new AuditDetailRecallModel()
                    {
                        STT = stt,
                        ItemName = order.ItemName,
                        UnitName = order.UnitName,
                        Quantity = order.Quantity,
                        AuditQuantity = order.AuditQuantity,
                        Conclude = order.Conclude
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("Audit");
            dtInfo.Columns.Add("StringVoucherDate", typeof(string));
            dtInfo.Columns.Add("Name", typeof(string));
            dtInfo.Columns.Add("Description", typeof(string));
            dtInfo.Columns.Add("AuditLocation", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Description"] = models.Any() ? res.data.Description : string.Empty;
            infoRow["StringVoucherDate"] = res.data.CreatedDate.ToString();
            infoRow["AuditLocation"] = orders1.FirstOrDefault(x => x.Code.ToString().Equals(res.data?.AuditLocation)).Name;
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            //AuiditCou
            var stt1 = 1;
            var models1 = new List<AuditCouncilRecallModel>();
            if (result1.data?.Count > 0)
            {
                foreach (var order1 in result1.data)
                {
                    var m1 = new AuditCouncilRecallModel()
                    {
                        STT = stt1,
                        EmployeeName = order1.EmployeeName,
                        Role = order1.Role,
                    };
                    stt1++;
                    models1.Add(m1);
                }
            }


            var dtDataName1 = "Data1";
            var dtData1 = models1.ToDataTable();
            dtData1.TableName = dtDataName1;
            ds.Tables.Add(dtData1);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/bienbankiemkehanhchinh.xls");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();


            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", "bien_ban_kiem_ke-" + res.data.VoucherCode + ".xlsx");

        }

        #endregion

        #region Utilities

        private void GetAvailableData()
        {
            var auditLocation = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)AssetType.Office).ToString(),
                    Text = AssetType.Office.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)AssetType.Infrastructure).ToString(),
                    Text = AssetType.Infrastructure.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)AssetType.Project).ToString(),
                    Text = AssetType.Project.GetEnumDescription()
                },
            };
            ViewData["auditLocation"] = auditLocation;
        }
        #endregion
    }
}
