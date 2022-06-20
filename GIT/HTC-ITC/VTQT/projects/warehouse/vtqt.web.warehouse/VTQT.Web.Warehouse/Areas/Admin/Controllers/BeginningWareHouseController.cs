﻿using Aspose.Cells;
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
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.SharedMvc.Warehouse.Models.WareHouse;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class BeginningWareHouseController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public BeginningWareHouseController(IWorkContext workContext)
        {
            _workContext = workContext;
        }
        #endregion

        #region Methods

        public async Task<ActionResult> Index()
        {
            var checkRole = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse-user/check-role-user?idUser=" + _workContext.UserId + "", null, Method.GET, ApiHosts.Warehouse);

            if (!checkRole.success)
                return RedirectToAction("AccessDeniedPath", "WareHouse");
            var searchModel = new BeginningWareHouseSearchModel();
            var resWarehouse = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse/get-select", null, Method.GET, ApiHosts.Warehouse);
            ViewData["warehouses"] = resWarehouse.data;
            ViewData["defaultWarehouse"] = resWarehouse.data.First();

            var resWarehouseItem = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get-select", null, Method.GET, ApiHosts.Warehouse);
            foreach (var item in resWarehouseItem.data)
            {
                item.Name = "[" + item.Code + "] " + item.Name + "";
            }
            ViewData["warehouseItems"] = resWarehouseItem.data;
            ViewData["defaultWarehouseItem"] = resWarehouseItem.data.First();

            var resUnit = await ApiHelper.ExecuteAsync<List<UnitModel>>("/unit/get-select", null, Method.GET, ApiHosts.Warehouse);
            ViewData["units"] = resUnit.data;
            ViewData["defaultUnit"] = resUnit.data.First();

            var resLastSelected = await ApiHelper.
                ExecuteAsync<string>($"/warehouse/get-last-selected/?appId=5&userId={_workContext.UserId}&path=/Admin/BeginningWareHouse", null, Method.GET, ApiHosts.Warehouse);

            if (!string.IsNullOrEmpty(resLastSelected.data))
            {
                searchModel.WareHouesId = resLastSelected.data;
            }

            return View(searchModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind(Prefix = "models")] IEnumerable<BeginningWareHouseModel> models)
        {
            foreach (var m in models)
            {
                m.WareHouseId = m.WareHouseModel.Id;
                m.ItemId = m.WareHouseItemModel.Id;
                m.UnitId = m.UnitModel.Id;
                m.UnitName = m.UnitModel.UnitName;
                m.WareHouseItemName = m.WareHouseItemModel.Name;
            }
            var res = await ApiHelper.ExecuteAsync<List<BeginningWareHouseModel>>("/warehouse-beginning/create", models, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                StringBuilder errors = new StringBuilder();
                var modelsExist = res.data;
                if (modelsExist?.Count > 0)
                {
                    modelsExist.ForEach(x =>
                    {
                        errors.Append($"Vật tư '{x.WareHouseItemName}' đã tồn tại!");
                    });
                }

                NotifyError(errors.ToString());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { });
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind(Prefix = "models")] IEnumerable<BeginningWareHouseModel> models)
        {
            ModelState.Remove("WareHouseModel.Code");
            ModelState.Remove("WareHouseItemModel.UnitId");
            ModelState.Remove("WareHouseItemModel.Code");
            foreach (var m in models)
            {
                m.WareHouseId = m.WareHouseModel.Id;
                m.ItemId = m.WareHouseItemModel.Id;
                m.UnitId = m.UnitModel.Id;
                m.UnitName = m.UnitModel.UnitName;
            }

            var res = await ApiHelper.ExecuteAsync("/warehouse-beginning/edit", models, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        // hông có hiện thông báo hay load lại trang

        [HttpPost]
        public async Task<IActionResult> Deletes([Bind(Prefix = "models")] IEnumerable<BeginningWareHouseModel> models)
        {
            if (models == null)
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }
            IEnumerable<string> deletes = models.Select(s => s.Id);

            var res = await ApiHelper.ExecuteAsync("/warehouse-beginning/deletes", deletes, Method.DELETE, ApiHosts.Warehouse);
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

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request, BeginningWareHouseSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<BeginningWareHouseModel>>("/warehouse-beginning/get-list", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var resLastSelected = await ApiHelper
                    .ExecuteAsync<string>($"/warehouse/update-last-selected/?appId=5&userId={_workContext.UserId}&path=/Admin/BeginningWareHouse&warehouseId={searchModel.WareHouesId}", null, Method.POST, ApiHosts.Warehouse);

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseTreeModel>>("/warehouse/get-tree?showHidden=true", null, Method.POST, ApiHosts.Warehouse);

            var data = res?.data;
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

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetUnitId()
        {
            var res = await ApiHelper.ExecuteAsync<List<SelectItem>>("/unit/get-select", null, Method.GET, ApiHosts.Warehouse);
            return Ok(res.data);
        }

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetItemId()
        {
            var res = await ApiHelper.ExecuteAsync<List<SelectItem>>("/warehouse-item/get-select", null, Method.GET, ApiHosts.Warehouse);
            return Ok(res.data);
        }

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetWareHouseId()
        {
            var res = await ApiHelper.ExecuteAsync<List<SelectItem>>("/warehouse/get-select", null, Method.GET, ApiHosts.Warehouse);
            return Ok(res.data);
        }


        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetUnitIdByItemCode(string id)
        {
            var res = await ApiHelper.ExecuteAsync($"/unit/get-unit-by-code-item?code={id}", null, Method.GET, ApiHosts.Warehouse);
            return Ok(res);
        }

        #endregion Lists

        #region Export

        public async Task<ActionResult> ExportOrder(BeginningWareHouseSearchModel model)
        {
            var fileName = "danh-sach-ton-kho-dau-ki.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<BeginningWareHouseModel>>("/warehouse-beginning/get-list", model, Method.GET, ApiHosts.Warehouse);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách tồn kho đầu kì");
            ws.DefaultColWidth = 20;
            //  ws.DefaultRowHeight = 40;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 10;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 15;
            ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Kho";
            ws.Cells[2, 3].Value = "Vật tư";
            ws.Cells[2, 4].Value = "Đơn vị tính";
            ws.Cells[2, 5].Value = "Số lượng tồn"; ;
            var i = 3;
            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.WareHouseModel.Name;
                    ws.Cells[i, 3].Value = order.WareHouseItemModel.Name;
                    ws.Cells[i, 4].Value = order.UnitModel.UnitName;
                    ws.Cells[i, 5].Value = order.Quantity;
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

        public async Task<ActionResult> GetExcelReport(BeginningWareHouseSearchModel model)
        {
            var res = await ApiHelper.ExecuteAsync<List<BeginningWareHouseModel>>("/warehouse-beginning/get-list", model, Method.GET, ApiHosts.Warehouse);
            var data = res?.data;
            var stt = 1;
            var models = new List<BeginningExportModel>();
            if (data?.Count > 0)
            {
                foreach (var e in data)
                {
                    var m = new BeginningExportModel
                    {
                        STT = stt,
                        Item = e.WareHouseItemModel.Name,
                        Quantity = e.Quantity,
                        UnitName = e.UnitName,
                        WareHouse = e.WareHouseModel.Name
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Beginning");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo tồn kho đầu kì";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/Beginning_vi.xlsx");
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

            var fileDownloadName = "bao_cao_ton_kho_dau_ki.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }

        #endregion Export

        #region ImportExcel

        public IActionResult ImportExcel()
        {
            var bg = new BeginningWareHouseModel();
            return View(bg);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportExcel(IEnumerable<BeginningWareHouseModel> models)
        {
            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();
            var db = new List<BeginningWareHouseModel>();
            foreach (var item in models)
            {
                await GetAvailable();
                var check = (IEnumerable<SelectListItem>)ViewData["warehouses"];
                item.WareHouseId = GetReason(check, item.WareHouseId.Trim());

                var check1 = (IEnumerable<SelectListItem>)ViewData["WareHouseItem"];
                item.ItemId = GetReason(check1, item.ItemId.Trim());

                var check2 = (IEnumerable<SelectListItem>)ViewData["Unit"];
                item.UnitId = GetReason(check2, item.UnitId.Trim());
                var teamCreate = new List<BeginningWareHouseModel>();
                teamCreate.Add(item);
                countColumn++;
                var res = await ApiHelper.ExecuteAsync("/warehouse-beginning/create", teamCreate, Method.POST, ApiHosts.Warehouse);

                if (!res.success)
                {
                    var temError = "" + countColumn + ";" + res.message + "";
                    vs.Add(temError);
                    openWith.Add(countColumn, res.GetErrorsToHtml().Replace("</span>", "").Replace("<span>", "").Replace("<ul>", "").Replace("<li>", "").Replace("</li>", ",").Replace("</ul>", ""));
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

        public async Task<ActionResult> ExporBeginning()
        {
            var fileName = "ton-kho-dau-ky.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Tồn kho đầu kỳ");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 30;
            ws.Column(2).Width = 15;
            ws.Column(4).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Value = "Kho";
            ws.Cells[1, 1].AddComment("Nhập mã kho trên phần mềm vào hệ thống");

            ws.Cells[1, 2].Value = "Vật tư";
            ws.Cells[1, 2].AddComment("Nhập mã vật tư trên phần mềm vào hệ thống");

            ws.Cells[1, 3].Value = "Đơn vị tính";
            ws.Cells[1, 3].AddComment("Nhập đơn vị trên phần mềm vào hệ thống");

            ws.Cells[1, 4].Value = "Số lượng tồn";
            ws.Cells[1, 4].AddComment("Nhập số lượng tồn vào hệ thống");
            ws.Cells[2, 4, 10000, 4].Style.Numberformat.Format = "#";

            // set style name column
            using (var rng = ws.Cells["A1:D1"])
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

            var resWarehouseitem = await ApiHelper
                .ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get-available", null, Method.GET, ApiHosts.Warehouse);

            var resUnit = await ApiHelper
                .ExecuteAsync<List<UnitModel>>("/unit/get-available", null, Method.GET, ApiHosts.Warehouse);

            var categories = new List<SelectListItem>();
            var categories1 = new List<SelectListItem>();
            var categories2 = new List<SelectListItem>();
            var data = res.data;
            var data1 = resWarehouseitem.data;
            var data2 = resUnit.data;

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
                        Text = m.Code
                    };
                    categories1.Add(item);
                }
            }
            if (data2?.Count > 0)
            {
                foreach (var m in data2)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.UnitName
                    };
                    categories2.Add(item);
                }
            }
            categories.OrderBy(e => e.Text);
            categories1.OrderBy(e => e.Text);
            categories2.OrderBy(e => e.Text);
            ViewData["warehouses"] = categories;
            ViewData["wareHouseItem"] = categories1;
            ViewData["unit"] = categories2;
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
            infoRow["Title"] = "Danh sách nhập lỗi tồn đầu kỳ";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-beginning.xlsx");
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
        #endregion ImportExcel

    }
}