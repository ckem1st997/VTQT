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
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Helper;
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class WareHouseItemCategoryController : AdminMvcController
    {
        #region Fields



        #endregion Fields

        #region Ctor

        public WareHouseItemCategoryController(
            )
        {
        }

        #endregion

        #region Methods

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetParentId()
        {
            var res = await ApiHelper.ExecuteAsync<List<SelectItem>>("/warehouse-item-category/get-select", null, Method.GET, ApiHosts.Warehouse);
            return Ok(res.data);
        }

        public async Task<IActionResult> Index()
        {
            var searchModel = new WareHouseItemCategorySearchModel();
            await GetAvailableCategories();
            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemCategoryModel>("/warehouse-item-category/details", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            await GetAvailableCategories();

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemCategoryModel>("/warehouse-item-category/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            await GetAvailableCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WareHouseItemCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/warehouse-item-category/create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemCategoryModel>("/warehouse-item-category/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            await GetAvailableCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WareHouseItemCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/warehouse-item-category/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/warehouse-item-category/deletes", ids, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/warehouse-item-category/activates", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methods

        #region Lists

        // TODO-Remove
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, WareHouseItemCategorySearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion

        #region Helpers

        #endregion

        #region Utilities
        private async Task GetAvailableCategories()
        {
            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get-available", null, Method.GET, ApiHosts.Warehouse);
            var categories = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.Name
                    };
                    categories.Add(item);
                }
            }
            categories.OrderBy(e => e.Text);
            ViewData["categories"] = categories;
        }


        #endregion

        #region export
        public async Task<ActionResult> ExportOrder(WareHouseItemCategorySearchModel model)
        {
            var fileName = "danh-sach-loai-vat-tu.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get", model, Method.GET, ApiHosts.Warehouse);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách loại vật tư");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Column(1).Width = 10;
            ws.Column(2).Width = 15;
            ws.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Mã loại";
            ws.Cells[2, 3].Value = "Tên loại";
            ws.Cells[2, 4].Value = "Thuộc loại";
            ws.Cells[2, 5].Value = "Áp dụng";
            var i = 3;
            if (orders != null)
            {
                var modelc = new WareHouseItemCategoryModel();
                await WareHouseHelper.GetAvailableWareHouseItemCategories(modelc);
                var checkName = modelc.AvailableCategories;
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.Code;
                    ws.Cells[i, 3].Value = order.Name;
                    ws.Cells[i, 4].Value = CheckNullString(order.ParentId, checkName);
                    ws.Cells[i, 5].Value = order.Inactive ? "Ngừng kích hoạt" : "Đã kích hoạt";
                    i++;
                }
            }


            // set style title

            using (var rng = ws.Cells["C1"])
            {
                rng.Value = "Danh sách loại vật tư";
                rng.Merge = true;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            }

            // set style name column
            using (var rng = ws.Cells["A2:E2"])
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
            var check = vs.FirstOrDefault(x => x.Value.Equals(Show));
            return check is null ? "" : check.Text;
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

        public async Task<ActionResult> GetExcelReport(WareHouseItemCategorySearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res?.data;
            await GetAvailableCategories();

            var check = (IEnumerable<SelectListItem>)ViewData["categories"];
            var stt = 1;
            var models = new List<WareHouseItemCategoryExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new WareHouseItemCategoryExportModel
                    {
                        STT = stt,
                        Code = order.Code,
                        Name = order.Name,
                        ItemType = GetReason(check, order.ParentId),
                        Inactive = order.Inactive ? "Chưa kích hoạt" : "Đã kích hoạt"
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("WareHouseItemCategory");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết loại vật tư";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/WareHouseItemCategory_vi.xlsx");
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

            var fileDownloadName = "bao_cao_chi_tiet_loai_vat_tu.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }



        private string CheckNullString(string check, IList<SelectListItem> selectItems)
        {
            if (string.IsNullOrEmpty(check))
                return "";
            return selectItems.FirstOrDefault(x => x.Value.Equals(check)).Text;
        }
        #endregion

        #region ImportExcel

        public IActionResult ImportExcel()
        {
            var ware = new WareHouseItemCategoryModel();
            return View(ware);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public virtual async Task<IActionResult> ImportExcel([FromBody] IEnumerable<WareHouseItemCategoryModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));
            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();

            #region GetAvailable

            var resWhCats = await ApiHelper
                .ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get-available", null, Method.GET, ApiHosts.Warehouse);

            var selWhCats = new List<SelectListItem>();

            var whCats = resWhCats.data;


            if (whCats?.Count > 0)
            {
                foreach (var m in whCats)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.Name
                    };
                    selWhCats.Add(item);
                }
            }
           
            selWhCats.OrderBy(e => e.Text);

            #endregion

            foreach (var item in models)
            {
                if (!string.IsNullOrEmpty(item.ParentId))
                    item.ParentId = GetReason(selWhCats, item.ParentId.Trim());
            }

            var res = await ApiHelper.ExecuteAsync("/warehouse-item-category/create-batch", models, Method.POST, ApiHosts.Warehouse);
            var model = res.data;
            if (!res.success)
            {
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

        public async Task<ActionResult> ExporWareHouseItemCategory()
        {
            var fileName = "danh-sach-loai-vat-tu.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách loại vật tư");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[1, 1].Value = "Mã loại";
            ws.Cells[1, 1].AddComment("Nhập mã loại vật tư vào hệ thống");

            ws.Cells[1, 2].Value = "Tên loại";
            ws.Cells[1, 2].AddComment("Nhập tên loại vật tư vào hệ thống");

            ws.Cells[1, 3].Value = "Thuộc loại";
            ws.Cells[1, 3].AddComment("Nhập mã loại vật tư vào hệ thống");

            ws.Cells[1, 4].Value = "Mô tả";
            ws.Cells[1, 4].AddComment("Nhập mô tả vào hệ thống");

            ws.Cells[1, 5].Value = "Áp dụng";
            ws.Cells[1, 5].AddComment("Chú ý nhập áp dụng vào hệ thống mặc định là (Đã kích hoạt hoặc Ngừng Kích hoạt)");


            // set style name column
            using (var rng = ws.Cells["A1:E1"])
            {
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
            infoRow["Title"] = "Danh sách nhập lỗi loại vật tư";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-warehouseitemcategory.xlsx");
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