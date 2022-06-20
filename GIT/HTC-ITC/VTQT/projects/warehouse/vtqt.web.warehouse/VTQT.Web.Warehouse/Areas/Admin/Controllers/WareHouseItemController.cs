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
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class WareHouseItemController : AdminMvcController
    {
        #region Ctor

        public WareHouseItemController(
            )
        {
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var searchModel = new WareHouseItemSearchModel();

            return View(searchModel);
        }

        /// <summary>
        /// Lấy chi tiết vật tư
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<WareHouseItemModel>("/warehouse-item/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        /// <summary>
        /// Tạo mới vật tư
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<WareHouseItemModel>("/warehouse-item/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            await GetDropDownList(model);

            return View(model);
        }

        public async Task<IActionResult> GetAllUnit()
        {
            var res = await ApiHelper
                .ExecuteAsync<WareHouseItemModel>("/warehouse-item/update-all-warehouse-item-unit", null, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError("Đồng bộ thất bại!!");
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess("Đồng bộ thành công... bạn có thể về nhà ăn tết !!");
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo danh sách dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task GetDropDownList(WareHouseItemModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<UnitModel>>("/unit/get-available", null, Method.GET, ApiHosts.Warehouse);
            var availableVendors = await ApiHelper
                .ExecuteAsync<List<VendorModel>>("/vendor/get-available", null, Method.GET, ApiHosts.Warehouse);
            var availableWhItemCategories = await ApiHelper
                .ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get-available", null, Method.GET, ApiHosts.Warehouse);

            if (availableUnits?.data?.Count > 0)
            {
                availableUnits.data.ForEach(item =>
                {
                    model.AvailableUnits
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.UnitName,
                        Value = item.Id
                    });
                });
            }

            if (availableVendors?.data?.Count > 0)
            {
                availableVendors.data.ForEach(item =>
                {
                    model.AvailableVendors
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }

            if (availableWhItemCategories?.data?.Count > 0)
            {
                availableWhItemCategories.data.ForEach(item =>
                {
                    model.AvailableWareHouseItemCategories
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }
        }

        /// <summary>
        /// Lấy về vật tư cần chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemModel>("/warehouse-item/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetDropDownList(model);

            if (model.AvailableUnits.Count > 0 &&
                !string.IsNullOrEmpty(model.UnitId))
            {
                var item = model.AvailableUnits
                    .FirstOrDefault(x => x.Value.Equals(model.UnitId));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (model.AvailableVendors.Count > 0 &&
                !string.IsNullOrEmpty(model.VendorID))
            {
                var item = model.AvailableVendors
                    .FirstOrDefault(x => x.Value.Equals(model.VendorID));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (model.AvailableWareHouseItemCategories.Count > 0 &&
                !string.IsNullOrEmpty(model.CategoryID))
            {
                var item = model.AvailableWareHouseItemCategories
                    .FirstOrDefault(x => x.Value.Equals(model.CategoryID));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa vật tư
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(WareHouseItemModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/warehouse-item/edit", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa vật tư
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
                .ExecuteAsync("/warehouse-item/deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api kích hoạt, ngừng kích hoạt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/warehouse-item/activates", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sách vật tư phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             WareHouseItemSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion List

        #region WarehouseItemUnit

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Read([DataSourceRequest] DataSourceRequest request, WareHouseItemUnitSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemUnitModel>>("/warehouse-item-unit/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count : res.totalCount
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSave(WareHouseItemModel model, IEnumerable<WareHouseItemUnitModel> modelDetails)
        {
            var addItem = await ApiHelper.ExecuteAsync("/warehouse-item/create", model, Method.POST, ApiHosts.Warehouse);
            if (!addItem.success)
            {
                NotifyError(addItem.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            foreach (var item in modelDetails)
            {
                var tem = new WareHouseItemUnitModel();
                tem.ItemId = model.Id;
                tem.UnitId = item.UnitId;
                tem.UnitName = item.UnitName;
                tem.ConvertRate = item.ConvertRate;
                var res = await ApiHelper.ExecuteAsync("/warehouse-item-unit/create", tem, Method.POST, ApiHosts.Warehouse);
                if (!res.success)
                {
                    NotifyError(res.GetErrorsToHtml());
                    return Ok(new XBaseResult { success = false });
                }
            }

            NotifySuccess(addItem.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> AddItem()
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemUnitModel>("/warehouse-item-unit/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> CreateItem(string itemId)
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemUnitModel>("/warehouse-item-unit/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.ItemId = itemId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(WareHouseItemUnitModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/warehouse-item-unit/create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemNoMessage(WareHouseItemUnitModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/warehouse-item-unit/create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                return Ok(new XBaseResult { success = false });
            }
            return Ok(new XBaseResult() { success = true });
        }

        public async Task<IActionResult> EditItem(string id)
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseItemUnitModel>("/warehouse-item-unit/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(WareHouseItemUnitModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/warehouse-item-unit/edit", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> EditSave(WareHouseItemUnitModel model)
        {
            var res = await ApiHelper.ExecuteAsync("/warehouse-item/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/warehouse-item-unit/deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion WarehouseItemUnit

        #region Check  by API

        [HttpGet]
        public async Task<IActionResult> CheckUnitExits([FromQuery] string ItemId, string UnitId)
        {
            if (string.IsNullOrEmpty(ItemId) || string.IsNullOrEmpty(UnitId))
                return Ok(new XBaseResult() { success = false });
            string url = $"/warehouse-item-unit/check-unit-by-item-id?ItemId={ItemId}&UnitId={UnitId}";
            var res = await ApiHelper.ExecuteAsync(url, null, Method.GET, ApiHosts.Warehouse);
            return Ok(new XBaseResult() { success = res.success });
        }

        #endregion Check  by API

        #region export

        public async Task<ActionResult> ExportOrder(WareHouseItemSearchModel model)
        {
            var fileName = "danh-sach-vat-tu.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get", model, Method.GET, ApiHosts.Warehouse);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách vật tư");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 10;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 25;
            ws.Column(8).Width = 15;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Mã vật tư";
            ws.Cells[2, 3].Value = "Tên vật tư";
            ws.Cells[2, 4].Value = "Loại vật tư";
            ws.Cells[2, 5].Value = "Mô tả";
            ws.Cells[2, 6].Value = "Nhà cung cấp";
            ws.Cells[2, 7].Value = "Quốc gia";
            ws.Cells[2, 8].Value = "Áp dụng";
            var i = 3;
            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.Code;
                    ws.Cells[i, 3].Value = order.Name;
                    ws.Cells[i, 4].Value = order.WareHouseItemCategoryModel.Name;
                    ws.Cells[i, 5].Value = order.Description;
                    ws.Cells[i, 6].Value = order.VendorModel.Name;
                    ws.Cells[i, 7].Value = order.Country;
                    ws.Cells[i, 8].Value = order.Inactive ? "Ngừng kích hoạt" : "Đã kích hoạt";
                    i++;
                }

            // set style title

            using (var rng = ws.Cells["D1"])
            {
                rng.Value = "Danh sách vật tư";
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

        public async Task<ActionResult> GetExcelReport(WareHouseItemSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get", searchModel, Method.GET, ApiHosts.Warehouse); var data = res?.data;
            var stt = 1;
            var models = new List<WareHouseItemExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new WareHouseItemExportModel
                    {
                        STT = stt,
                        Code = order.Code,
                        Name = order.Name,
                        ItemType = order.WareHouseItemCategoryModel.Name,
                        Country = order.Country,
                        Description = order.Description,
                        VendorName = order.VendorModel.Name,
                        Inactive = order.Inactive ? "Chưa kích hoạt" : "Đã kích hoạt"
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("WareHouseItem");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết loại danh mục";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/WareHouseItem_vi.xlsx");
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

            var fileDownloadName = "bao_cao_chi_tiet_loai_danh_muc.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }

        #endregion export

        #region ImportExcel

        public IActionResult ImportExcel()
        {
            var ware = new WareHouseItemModel();
            return View(ware);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public virtual async Task<IActionResult> ImportExcel([FromBody] IEnumerable<WareHouseItemModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();
            var db = new List<WareHouseItemModel>();

            #region GetAvailable
            // lấy tất danh sách chuẩn bị để lọc dữ liệu 

            var resWhCats = await ApiHelper
                .ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get-available", null, Method.GET, ApiHosts.Warehouse);

            var resVendors = await ApiHelper
                .ExecuteAsync<List<VendorModel>>("/vendor/get-available", null, Method.GET, ApiHosts.Warehouse);

            var resUnits = await ApiHelper
                .ExecuteAsync<List<UnitModel>>("/unit/get-available", null, Method.GET, ApiHosts.Warehouse);

            var selWhCats = new List<SelectListItem>();
            var selVendors = new List<SelectListItem>();
            var selUnits = new List<SelectListItem>();

            var whCats = resWhCats.data;
            var vendors = resVendors.data;
            var units = resUnits.data;


            if (whCats?.Count > 0)
            {
                foreach (var m in whCats)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.Code
                    };
                    selWhCats.Add(item);
                }
            }
            if (vendors?.Count > 0)
            {
                foreach (var m in vendors)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.Code
                    };
                    selVendors.Add(item);
                }
            }
            if (units?.Count > 0)
            {
                foreach (var m in units)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.UnitName
                    };
                    selUnits.Add(item);
                }
            }
            selWhCats.OrderBy(e => e.Text);
            selVendors.OrderBy(e => e.Text);
            selUnits.OrderBy(e => e.Text);
            #endregion

            // tiến hành lọc lấy ID từ tên trong excel đối với danh mục yêu cầu nhập tên
            foreach (var item in models)
            {
                if (!string.IsNullOrEmpty(item.CategoryID))
                    item.CategoryID = GetReason(selWhCats, item.CategoryID.Trim());

                if (!string.IsNullOrEmpty(item.VendorID))
                    item.VendorID = GetReason(selVendors, item.VendorID.Trim());

                if (!string.IsNullOrEmpty(item.UnitId))
                    item.UnitId = GetReason(selUnits, item.UnitId.Trim());
            }

            var res = await ApiHelper.ExecuteAsync("/warehouse-item/create-batch", models, Method.POST, ApiHosts.Warehouse);
            var model = res.data;
            if (!res.success)
            {
                // thêm lỗi vào để hiển thị
                foreach (var error in res.errors)
                {
                    vs.Add($"[{error.Key}] {error.Value.StrJoin(";")}");
                    openWith.Add(int.Parse(error.Key), error.Value.StrJoin(";"));
                }
            }

            TempData["DowloadFileToError"] = await GetExcelReportError(openWith);
            return Ok(new { totalErr = vs.Count(), data= model });
        }

        public IActionResult TotalImportExcel(int sum = 0, int err = 0, long countDone=0)
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

        public async Task<ActionResult> ExportWareHouseItem()
        {
            var fileName = "danh-sach-vat-tu.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách vật tư");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[1, 1].Value = "Mã vật tư";
            ws.Cells[1, 1].AddComment("Nhập mã vật tư vào hệ thống");

            ws.Cells[1, 2].Value = "Tên vật tư";
            ws.Cells[1, 2].AddComment("Nhập tên vật tư vào hệ thống");

            ws.Cells[1, 3].Value = "Đơn vị";
            ws.Cells[1, 3].AddComment("Nhập đơn vị của phần mềm vào hệ thống");

            ws.Cells[1, 4].Value = "Loại vật tư";
            ws.Cells[1, 4].AddComment("Nhập mã loại vật tư của phần mềm vào hệ thống");

            ws.Cells[1, 5].Value = "Mô tả";
            ws.Cells[1, 5].AddComment("Nhập mô tả vào hệ thống");

            ws.Cells[1, 6].Value = "Nhà cung cấp";
            ws.Cells[1, 6].AddComment("Nhập nhà cung cấp của phần mềm vào hệ thống");

            ws.Cells[1, 7].Value = "Quốc gia";
            ws.Cells[1, 7].AddComment("Nhập quốc gia vào hệ thống");

            ws.Cells[1, 8].Value = "Áp dụng";
            ws.Cells[1, 8].AddComment("Chú ý nhập áp dụng vào hệ thống mặc định là (Đã kích hoạt hoặc Ngừng Kích hoạt)");

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

        private async Task GetAvailable()
        {
            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get-available", null, Method.GET, ApiHosts.Warehouse);

            var resvendor = await ApiHelper
                .ExecuteAsync<List<VendorModel>>("/vendor/get-available", null, Method.GET, ApiHosts.Warehouse);

            var resUnit = await ApiHelper
                .ExecuteAsync<List<UnitModel>>("/unit/get-available", null, Method.GET, ApiHosts.Warehouse);

            var categories = new List<SelectListItem>();
            var categories1 = new List<SelectListItem>();
            var categories2 = new List<SelectListItem>();

            var data = res.data;
            var data1 = resvendor.data;
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
            ViewData["warehouseItemCategores"] = categories;
            ViewData["vendor"] = categories1;
            ViewData["unit"] = categories2;
        }

        private static string GetReason(IEnumerable<SelectListItem> vs, string Show)
        {
            if (Show is null)
                return "";
            var check = vs.FirstOrDefault(x => x.Text.Equals(Show, StringComparison.OrdinalIgnoreCase));
            return check is null ? "" : check.Value;
        }

        /// <summary>
        /// Get Equals two string
        /// </summary>
        /// <param name="s1">params 1</param>
        /// <param name="s2">params 2</param>
        /// <returns></returns>
        public static bool CheckStringToIgnoreCare(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;
            return s1.Equals(s2, StringComparison.OrdinalIgnoreCase);
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
            infoRow["Title"] = "Danh sách nhập lỗi vật tư";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-warehouseitem.xlsx");
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