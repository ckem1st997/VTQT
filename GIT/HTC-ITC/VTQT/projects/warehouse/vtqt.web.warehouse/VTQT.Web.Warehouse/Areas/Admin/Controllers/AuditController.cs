using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.SharedMvc.Warehouse.Models.WareHouse;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class AuditController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public AuditController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion Ctor

        #region Methos

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AuditModel>("/audit/details", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Create(string IdWareHouse)
        {
            var res = await ApiHelper.ExecuteAsync<AuditModel>("/audit/create", null, Method.GET, ApiHosts.Warehouse);
            var code = ApiHelper.Execute("/generate-code/get?tableName=Audit", null, Method.GET, ApiHosts.Master);

            var model = res.data;
            model.VoucherDate = DateTime.Now;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = _workContext.UserId;
            model.VoucherCode = code.data;
            if (!string.IsNullOrEmpty(IdWareHouse))
                model.WareHouseId = IdWareHouse;
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
                            ItemId = item.ItemId,
                            Serial = s
                        });
                    }
                }
            }
            model.AuditDetails = modelDetails.ToList();
            model.AuditCouncils = auditCouncilModels.ToList();
            var res = await ApiHelper.ExecuteAsync("/audit/create", model, Method.POST, ApiHosts.Warehouse);
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
            var res = await ApiHelper.ExecuteAsync<AuditModel>("/audit/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSave(AuditModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/audit/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/audit/deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methos

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
            var res = await ApiHelper.ExecuteAsync<AuditDetailModel>("/audit-detail/detail-create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> CreateItem(string auditId)
        {
            var res = await ApiHelper.ExecuteAsync<AuditDetailModel>("/audit-detail/detail-create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.AuditId = auditId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(AuditDetailModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/audit-detail/detail-create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditItem1(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AuditDetailModel>("/audit-detail/detail-edit", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(AuditDetailModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/audit-detail/detail-edit", model, Method.POST, ApiHosts.Warehouse);
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
            var res = await ApiHelper.ExecuteAsync("/audit-detail/detail-edit-list", models, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/audit-detail/detail-deletes", ids, Method.POST, ApiHosts.Warehouse);
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

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetWareHouseId()
        {
            var res = await ApiHelper.ExecuteAsync<InwardModel>("/inward/create", null, Method.GET, ApiHosts.Warehouse);
            var list = new List<SelectItem>();
            foreach (var item in res.data.AvailableWareHouses)
            {
                var tem = new SelectItem();
                tem.text = item.Text;
                tem.id = item.Value;
                list.Add(tem);
            }
            return Ok(list);
        }

        public async Task<IActionResult> Index()
        {
            var checkRole = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse-user/check-role-user?idUser=" + _workContext.UserId + "", null, Method.GET, ApiHosts.Warehouse);

            if (!checkRole.success)
                return RedirectToAction("AccessDeniedPath", "WareHouse");
            var result = new AuditSearchModel();
            var model = new AuditModel();

            var resLastSelected = await ApiHelper.
                ExecuteAsync<string>($"/warehouse/get-last-selected/?appId=5&userId={_workContext.UserId}&path=/Admin/Audit", null, Method.GET, ApiHosts.Warehouse);
            var resEmployees = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            var list = new List<SelectListItem>();
            foreach (var item in resEmployees.data)
            {
                var tem= new SelectListItem()
                {
                    Text=item.FullName+"-"+item.UserName,
                    Value=item.Id
                };
                list.Add(tem);
            }
            ViewData["employees"] = list;

            if (!string.IsNullOrEmpty(resLastSelected.data))
            {
                result.WareHouesId = resLastSelected.data;
            }

            return View(result);
        }

        /// <summary>
        /// Gọi Api lấy cấu trúc cây kho
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseTreeModel>>("/warehouse/get-tree?showHidden=true", null, Method.POST, ApiHosts.Warehouse);
            var data = res.data;
            IList<WareHouseTreeModel> cg = new List<WareHouseTreeModel>();
            foreach (var item in data)
            {
                cg.Add(item);
            }
            var all = new WareHouseTreeModel()
            {
                Name = "Tất cả",
                key = "",
                title = "Tất cả",
                tooltip = "Tất cả",
                children = cg,
                level = 1,
                expanded = true
            };
            res.data.Clear();
            res.data.Add(all);
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request, AuditSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper.ExecuteAsync<List<AuditModel>>("/audit/get-list-name", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var resLastSelected = await ApiHelper
                    .ExecuteAsync<string>($"/warehouse/update-last-selected/?appId=5&userId={_workContext.UserId}&path=/Admin/Audit&warehouseId={searchModel.WareHouesId}", null, Method.POST, ApiHosts.Warehouse);

            if (data?.Count > 0)
            {
                var listUser = await ApiHelper.ExecuteAsync<AuditModel>("/audit/create", null, Method.GET, ApiHosts.Warehouse);

                data.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedBy))
                    {
                        var userName = listUser.data.AvailableCreatedBy.FirstOrDefault(z => z.Value.Equals(x.CreatedBy)).Text;
                        x.CreatedBy = userName.Split("-").Length > 0 ? userName.Split("-")[0] : string.Empty;
                    }

                });
            }
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
            var res = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>("/audit-detail/detail-get", searchModel, Method.GET, ApiHosts.Warehouse);
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
            var res = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>($"/audit-detail/get-list-item-by-id?dateTime={dateTime}%2012%3A00%3A00&idw={id}", null, Method.GET, ApiHosts.Warehouse);
            var data = res.data;
            if (data != null)
            {
                var lisst = new List<AuditDetailModel>();
                foreach (var item in data)
                {
                    if (item.Quantity > 0 || item.Quantity<0)
                    {
                        var tem = new AuditDetailModel()
                        {
                            ItemId = item.ItemId,
                            ItemName=item.ItemName,
                            UnitName = item.UnitName,
                            Quantity = item.Quantity
                        };
                        lisst.Add(tem);
                    }
                }

                var result1 = new DataSourceResult
                {
                    Data = lisst,
                };
                return Ok(result1);
            }

            var result = new DataSourceResult
            {
                Data = data,
            };
            return Ok(result);
        }

        public static string ConvertDateTimeToDateTimeSql(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return "";
            return "" + dateTime.Value.Year + "-" + dateTime.Value.Month + "-" + dateTime.Value.Day + "";

        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update_Read([DataSourceRequest] DataSourceRequest request, AuditDetailSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>("/audit-detail/detail-get", searchModel, Method.GET, ApiHosts.Warehouse);
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
            var res = await ApiHelper.ExecuteAsync<List<AuditCouncilModel>>("/audit-council/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count : res.totalCount
            };
            return Ok(result);
        }
        #endregion Lists

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
            var res = await ApiHelper.ExecuteAsync<AuditCouncilModel>("audit-council/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> CreateAuditCouncil(string auditId)
        {
            var res = await ApiHelper.ExecuteAsync<AuditCouncilModel>("/audit-council/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.AuditId = auditId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuditCouncil(AuditCouncilModel model)
        {
            model.EmployeeName = "trung";
            var res = await ApiHelper.ExecuteAsync("/audit-council/create", model, Method.POST, ApiHosts.Warehouse);
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
            var res = await ApiHelper.ExecuteAsync<AuditCouncilModel>("/audit-council/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAuditCouncil(AuditCouncilModel model)
        {
            model.EmployeeName = "trung";
            var res = await ApiHelper.ExecuteAsync("/audit-council/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/audit-council/deletes", ids, Method.POST, ApiHosts.Warehouse);
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

        public async Task<ActionResult> ExportOrder(AuditSearchModel model)
        {
            var fileName = "danh-sach-kiem-ke-kho.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<AuditModel>>("/audit/get-list-name", model, Method.GET, ApiHosts.Warehouse);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách kiểm kê kho");
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
            ws.Cells[2, 2].Value = "Số phiếu";
            ws.Cells[2, 3].Value = "Ngày phiếu";
            ws.Cells[2, 4].Value = "Kho";
            ws.Cells[2, 5].Value = "Tên đợt kiểm kê";
            ws.Cells[2, 6].Value = "Ngày tạo";
            ws.Cells[2, 7].Value = "Người tạo";
            ws.Cells[2, 8].Value = "Ngày sửa";
            var i = 3;

            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.VoucherCode;
                    ws.Cells[i, 3].Value = order.VoucherDate.ToString("dd/MM/yyyy HH:mm");
                    ws.Cells[i, 4].Value = order.WareHouse.Name;
                    ws.Cells[i, 5].Value = order.Description;
                    ws.Cells[i, 6].Value = order.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                    ws.Cells[i, 7].Value = order.CreatedBy;
                    ws.Cells[i, 8].Value = order.ModifiedDate.ToString("dd/MM/yyyy HH:mm");
                    i++;
                }


            // set style title

            using (var rng = ws.Cells["D1:E1"])
            {
                rng.Value = "Danh sách tồn kho đầu kì";
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
            var res = await ApiHelper.ExecuteAsync<List<AuditModel>>("/audit/get-list-name", model, Method.GET, ApiHosts.Warehouse);
            var data = res?.data;
            var stt = 1;
            var models = new List<AuditExportModel>();

            var listUser = await ApiHelper.ExecuteAsync<AuditModel>("/audit/create", null, Method.GET, ApiHosts.Warehouse);
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
            if (data?.Count > 0)
            {
                foreach (var e in data)
                {
                    var m = new AuditExportModel
                    {
                        STT = stt,
                        VoucherCode = e.VoucherCode,
                        VoucherDate = e.VoucherDate.ToLocalTime().ToString("dd/MM/yyyy"),
                        WareHouseId = e.WareHouse.Name,
                        Description = e.Description,
                        CreatedDate = e.CreatedDate.ToLocalTime().ToString("dd/MM/yyyy"),
                        CreatedBy = e.CreatedBy,
                        ModifiedDate = e.ModifiedDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Audit");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo kiểm kê kho";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/Audit_vi.xlsx");
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

            var fileDownloadName = "bao_cao_kiem_ke_kho.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
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
                var check1 = (IEnumerable<SelectListItem>)ViewData["user"];
                item.CreatedBy = GetReason(check1, item.CreatedBy.Trim());

                var check = (IEnumerable<SelectListItem>)ViewData["warehouses"];
                item.WareHouseId = GetReason(check, item.WareHouseId.Trim());
                countColumn++;
                var res = await ApiHelper.ExecuteAsync("/audit/create", item, Method.POST, ApiHosts.Warehouse);

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
            var fileName = "danh-sach-kiem-ke-kho.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách kiểm kê kho");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 30;
            ws.Column(2).Width = 15;
            ws.Column(4).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Value = "Số chứng từ";
            ws.Cells[1, 1].AddComment("Nhập số chứng từ vào hệ thống");

            ws.Cells[1, 2].Value = "Ngày phiếu";
            ws.Cells[1, 2].AddComment("Nhập ngày phiếu trên hệ thống");

            ws.Cells[1, 3].Value = "Kho";
            ws.Cells[1, 3].AddComment("Nhập mã kho trên phần mềm vào hệ thống");

            ws.Cells[1, 4].Value = "Tên đợt kiểm kê";
            ws.Cells[1, 4].AddComment("Nhập tên đợt kiểm kê vào phần mềm");

            ws.Cells[1, 5].Value = "Ngày tạo";
            ws.Cells[1, 5].AddComment("Nhập ngày tạo vào phần mềm");

            ws.Cells[1, 6].Value = "Người tạo";
            ws.Cells[1, 6].AddComment("Nhập người tạo vào phần mềm");

            ws.Cells[1, 7].Value = "Ngày sửa";
            ws.Cells[1, 7].AddComment("Nhập ngày sửa vào phần mềm");

            // set style name column
            using (var rng = ws.Cells["A1:G1"])
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

        private async Task GetAvailable()
        {
            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseModel>>("/warehouse/get-available", null, Method.GET, ApiHosts.Warehouse);
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
                        Value = m.Id,
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
            ViewData["warehouses"] = categories;
            ViewData["user"] = categories1;
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
            var models = new List<UnitExportModel>();

            if (model?.Count > 0)
            {
                foreach (var e in model)
                {
                    var m = new UnitExportModel
                    {
                        STT = e.Key,
                        Name = e.Value
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Unit");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Danh sách nhập lỗi kiểm kê kho";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-audit.xlsx");
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

        #endregion ImportExcel

        #region Inphieu
        public async Task<ActionResult> ExportDone(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<AuditModel>("/print/get-by-id-to-audit?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<AuditDetailModel>>($"/audit-detail/detail-get?AuditId={id}", null, Method.GET, ApiHosts.Warehouse);
            var result1 = await ApiHelper.ExecuteAsync<List<AuditCouncilModel>>($"/audit-council/get?AuditId={id}", null, Method.GET, ApiHosts.Warehouse);

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
                        UnitName=order.UnitName,
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
            dtInfo.Columns.Add("Address", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Description"] = models.Any() ? res.data.Description : string.Empty;
            infoRow["StringVoucherDate"] =res.data.CreatedDate.ToString();
            infoRow["Name"] = res.data.WareHouse.Name;
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

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/bienbankiemke.xls");
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


        #region Get Nhanvien

        public async Task<IActionResult> GetNhanVien(string search, int page)
        {
            var res = await ApiHelper .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);

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



        #endregion


    }
}