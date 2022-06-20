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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class VendorController : AdminMvcController
    {
        #region Fields

        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public VendorController(
            ILanguageService languageService)
        {
            _languageService = languageService;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new VendorSearchModel();

            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<VendorModel>("/vendor/details", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<VendorModel>("/vendor/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VendorModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/vendor/create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<VendorModel>("/vendor/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VendorModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/vendor/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/vendor/deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/vendor/activates", model, Method.POST, ApiHosts.Warehouse);
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

        // TODO-Remove
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, VendorSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<VendorModel>>("/vendor/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }

        #endregion

        #region Helpers



        #endregion

        #region Utilities



        #endregion

        #region export
        public async Task<ActionResult> ExportOrder(VendorSearchModel model)
        {
            var fileName = "danh-sach-nha-cung-cap.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<VendorModel>>("/vendor/get", model, Method.GET, ApiHosts.Warehouse);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách nhà cung cấp");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 10;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Mã nhà cung cấp";
            ws.Cells[2, 3].Value = "Tên nhà cung cấp";
            ws.Cells[2, 4].Value = "Địa chỉ";
            ws.Cells[2, 5].Value = "Số điện thoại";
            ws.Cells[2, 6].Value = "Hòm thư";
            ws.Cells[2, 7].Value = "Người liên hệ";
            ws.Cells[2, 8].Value = "Trạng thái";
            var i = 3;
            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.Code;
                    ws.Cells[i, 3].Value = order.Name;
                    ws.Cells[i, 4].Value = order.Address;
                    ws.Cells[i, 5].Value = order.Phone;
                    ws.Cells[i, 6].Value = order.Email;
                    ws.Cells[i, 7].Value = order.ContactPerson;
                    ws.Cells[i, 8].Value = order.Inactive ? "Ngừng kích hoạt" : "Đã kích hoạt";
                    i++;
                }

            // set style title

            using (var rng = ws.Cells["D1:F1"])
            {
                rng.Value = "Danh sách nhà cung cấp";
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
        public async Task<ActionResult> GetExcelReport(VendorSearchModel model)
        {
            var res = await ApiHelper.ExecuteAsync<List<VendorModel>>("/vendor/get", model, Method.GET, ApiHosts.Warehouse);
            var data = res?.data;
            var stt = 1;
            var models = new List<VendorExportModel>();
            if (data?.Count > 0)
            {
                foreach (var e in data)
                {
                    var m = new VendorExportModel
                    {
                        STT = stt,
                        Code = e.Code,
                        Name = e.Name,
                        Address = e.Address,
                        ContactPerson = e.ContactPerson,
                        Phone = e.Phone,
                        Email = e.Email,
                        Inactive = e.Inactive ? "Chưa kích hoạt" : "Đã kích hoạt"
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Vendor");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết nhà cung cấp";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/Vendor_vi.xlsx");
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

            var fileDownloadName = "bao_cao_chi_tiet_nha_cung_cap.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }


        #endregion

        #region ImportExcel

        public IActionResult ImportExcel()
        {
            var ware = new VendorModel();
            return View(ware);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public virtual async Task<IActionResult> ImportExcel([FromBody] IEnumerable<VendorModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));
            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();
            var db = new List<VendorModel>();

            #region GetAvailable

            
            #endregion


            var res = await ApiHelper.ExecuteAsync("/vendor/create-batch", models, Method.POST, ApiHosts.Warehouse);
            var model = res.data;
            if (!res.success)
            {
                // thêm lỗi vào để hiển thị
                foreach (var error in res.errors)
                {
                    vs.Add($"[{error.Key}] {error.Value}");
                    openWith.Add(int.Parse(error.Key), error.Value.StrJoin(";"));
                }
            }

            TempData["DowloadFileToError"] = await GetExcelReportError(openWith);
            return Ok(new { totalErr = vs.Count(), data = model });
        }

        public IActionResult TotalImportExcel(int sum = 0, int err = 0, long countDone = 0)
        {
            if (err > 0)
            {
                TempData["success"] = 0;
            }
            else
            {
                TempData["success"] = countDone;
            }
            TempData["err"] = err;
            TempData["Sum"] = sum;
            return View();
        }

        public async Task<ActionResult> ExporVendor()
        {
            var fileName = "danh-sach-nha-cung-cap.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Nhà cung cấp");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[1, 1].Value = "Mã nhà cung cấp";
            ws.Cells[1, 1].AddComment("Nhập mã nhà cung cấp vào hệ thống");

            ws.Cells[1, 2].Value = "Tên nhà cung cấp";
            ws.Cells[1, 2].AddComment("Nhập tên nhà cung cấp vào hệ thống");

            ws.Cells[1, 3].Value = "Địa chỉ";
            ws.Cells[1, 3].AddComment("Nhập địa chỉ vào hệ thống");

            ws.Cells[1, 4].Value = "Số điện thoại";
            ws.Cells[1, 4].AddComment("Nhập số điện thoại vào hệ thống");

            ws.Cells[1, 5].Value = "Hòm thư";
            ws.Cells[1, 5].AddComment("Nhập hòm thư vào hệ thống");

            ws.Cells[1, 6].Value = "Người liên hệ";
            ws.Cells[1, 6].AddComment("Nhập người liên hệ vào hệ thống");

            ws.Cells[1, 7].Value = "Áp dụng";
            ws.Cells[1, 7].AddComment("Chú ý nhập áp dụng vào hệ thống mặc định là (Đã kích hoạt hoặc Ngừng Kích hoạt)");

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

        private static string GetReason(IEnumerable<SelectListItem> vs, string Show)
        {
            if (Show is null)
                return "";
            var check = vs.FirstOrDefault(x => x.Text.Equals(Show, StringComparison.OrdinalIgnoreCase));
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
            infoRow["Title"] = "Danh sách nhập lỗi nhà cung cấp";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-vendor.xlsx");
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

        #endregion
    }
}