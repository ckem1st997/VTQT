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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.Utilities;
using VTQT.Web.Asset.Areas.Admin.Helper;
using VTQT.Web.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;


namespace VTQT.Web.Asset.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class AssetCategoryController : AdminMvcController
    {
        #region Fields

        #endregion

        #region Ctor
        public AssetCategoryController() { }
        #endregion

        #region Methods
        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            AssetCategorySearchModel searchModel = new AssetCategorySearchModel();
            await GetAvailableCategories();
            return View(searchModel);
        }

        /// <summary>
        /// Lấy chi tiết loại tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<AssetCategoryModel>("/asset-category/edit", new { id = id }, Method.GET, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            await GetAvailableCategories();

            model.SelectedDepreciationUnit = model.DepreciationUnit.ToString();
            model.SelectedWarrantyUnit = model.WarrantyUnit.ToString();

            return View(model);
        }

        /// <summary>
        /// Tạo mới loại tài sản
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<AssetCategoryModel>("/asset-category/create", null, Method.GET, ApiHosts.Asset);

            var model = res.data;
            await GetAvailableCategories();

            return View(model);
        }

        /// <summary>
        /// Gọi Api tạo mới loại tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            int.TryParse(model.SelectedDepreciationUnit, out int depreciationUnit);
            model.DepreciationUnit = depreciationUnit;
            int.TryParse(model.SelectedWarrantyUnit, out int warrantyUnit);
            model.WarrantyUnit = warrantyUnit;

            if (string.IsNullOrEmpty(model.Code))
            {
                var code = await ApiHelper.ExecuteAsync("/generate-code/get", new { tableName = nameof(AssetCategory) }, Method.GET, ApiHosts.Master);
                model.Code = code.data;
            }
            
            var res = await ApiHelper
                .ExecuteAsync("/asset-category/create", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy về loại tài sản cần chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AssetCategoryModel>("/asset-category/edit", new { id = id }, Method.GET, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            await GetAvailableCategories();

            model.SelectedDepreciationUnit = model.DepreciationUnit.ToString();
            model.SelectedWarrantyUnit = model.WarrantyUnit.ToString();

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa loại tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(AssetCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            int.TryParse(model.SelectedDepreciationUnit, out int depreciationUnit);
            model.DepreciationUnit = depreciationUnit;
            int.TryParse(model.SelectedWarrantyUnit, out int warrantyUnit);
            model.WarrantyUnit = warrantyUnit;

            var res = await ApiHelper
                .ExecuteAsync("/asset-category/edit", model, Method.POST, ApiHosts.Asset);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa loại tài sản
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
                .ExecuteAsync("/asset-category/deletes", ids, Method.POST, ApiHosts.Asset);
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
                .ExecuteAsync("/asset-category/activates", model, Method.POST, ApiHosts.Asset);
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
        /// Lấy danh sách loại tài sản phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             AssetCategorySearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<AssetCategoryModel>>("/asset-category/get", searchModel, Method.GET, ApiHosts.Asset);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }
        #endregion

        #region Utilities
        private async Task GetAvailableCategories()
        {
            var res = await ApiHelper
                .ExecuteAsync<List<AssetCategoryModel>>("/asset-category/get-available", null, Method.GET, ApiHosts.Asset);
            var categories = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    categories.Add(item);
                }
            }

            categories.OrderBy(e => e.Text);
            ViewData["categories"] = categories;

            var durations = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)Duration.Date).ToString(),
                    Text = Duration.Date.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)Duration.Month).ToString(),
                    Text = Duration.Month.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)Duration.Year).ToString(),
                    Text = Duration.Year.GetEnumDescription()
                }
            };
            ViewData["durations"] = durations;

            var assetTypes = new List<SelectListItem>
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
                }
            };
            ViewData["assetTypes"] = assetTypes;
        }
        #endregion

        #region Export
        public async Task<ActionResult> ExportOrder(AssetCategorySearchModel model)
        {
            var fileName = "danh-sach-loai-tai-san.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<AssetCategoryModel>>("/asset-category/get", model, Method.GET, ApiHosts.Asset);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách loại tài sản");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 10;
            ws.Column(2).Width = 15;

            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Mã loại tài sản";
            ws.Cells[2, 3].Value = "Tên loại tài sản";
            ws.Cells[2, 4].Value = "Thuộc loại";
            ws.Cells[2, 5].Value = "Mô tả";
            ws.Cells[2, 6].Value = "Trạng thái";
            var i = 3;
            if (orders != null)
            {
                var modelc = new AssetModel();
                await AssetHelper.GetAvailableAssetCategories(modelc);
                var checkName = modelc.AvailableCategories;
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.Code;
                    ws.Cells[i, 3].Value = order.Name;
                    ws.Cells[i, 4].Value = CheckNullString(order.ParentId, checkName);
                    ws.Cells[i, 5].Value = order.Description;
                    ws.Cells[i, 6].Value = order.Inactive ? "Ngừng kích hoạt" : "Đã kích hoạt";
                    i++;
                }
            }


            // set style title

            using (var rng = ws.Cells["C1:D1"])
            {
                rng.Value = "Danh sách loại tài sản";
                rng.Merge = true;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            }

            // set style name column
            using (var rng = ws.Cells["A2:F2"])
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

        #endregion
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

        public async Task<ActionResult> GetExcelReport(AssetCategorySearchModel model)
        {
            var res = await ApiHelper.ExecuteAsync<List<AssetCategoryModel>>("/asset-category/get", model, Method.GET, ApiHosts.Asset);
            var data = res?.data;
            var stt = 1;
            var models = new List<AssetCategoryExportModel>();
            if (data?.Count > 0)
            {
                var modelc = new AssetModel();
                await AssetHelper.GetAvailableAssetCategories(modelc);
                var checkName = modelc.AvailableCategories;
                foreach (var order in data)
                {
                    var m = new AssetCategoryExportModel
                    {
                        STT = stt,
                        Code = order.Code,
                        Name = order.Name,
                        Description = order.Description,
                        Inactive = order.Inactive ? "Chưa kích hoạt" : "Đã kích hoạt",
                        Type = CheckNullString(order.ParentId, checkName)
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("AssetCategory");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết loại tài sản";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/AssetCategory_vi.xlsx");
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

            var fileDownloadName = "bao_cao_chi_tiet_loai_tai_san.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }


        #region ImportExcel
        public IActionResult ImportExcel()
        {
            var asset = new AssetCategoryModel();
            return View(asset);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportExcel(IEnumerable<AssetCategoryModel> models)
        {
            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();
            var db = new List<AssetCategoryModel>();
            await GetAvailable();
            foreach (var item in models)
            {
                var check = (IEnumerable<SelectListItem>)ViewData["assetCategores"];
                if (!string.IsNullOrEmpty(item.ParentId))
                    item.ParentId = GetReasonWH(check, item.ParentId.Trim());

                if (!string.IsNullOrEmpty(item.SelectedDepreciationUnit))
                    item.DepreciationUnit= (int)GetValueFromDescription<Duration>(item.SelectedDepreciationUnit);

                if (!string.IsNullOrEmpty(item.SelectedWarrantyUnit))
                    item.WarrantyUnit = (int)GetValueFromDescription<Duration>(item.SelectedWarrantyUnit);
                countColumn++;
                var res = await ApiHelper.ExecuteAsync("/asset-category/create", item, Method.POST, ApiHosts.Asset);

                
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

        public async Task<ActionResult> ExporAssetCategory()
        {
            var fileName = "loai-tai-san.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách loại tài sản");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 30;
            ws.Column(2).Width = 15;
            ws.Column(4).Width = 30;
            ws.Column(8).Width = 30;
            ws.Column(6).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Value = "Mã loại tài sản";
            ws.Cells[1, 2].Value = "Tên loại tài sản";
            ws.Cells[1, 3].Value = "Thuộc loại";
            ws.Cells[1, 4].Value = "Mô tả";
            ws.Cells[1, 5].Value = "Thời gian khấu hao";
            ws.Cells[1, 6].Value = "Đơn vị của thời gian khấu hao";
            ws.Cells[1, 7].Value = "Thời gian bảo hành";
            ws.Cells[1, 8].Value = "Đơn vị của thời gian bảo hành";
            ws.Cells[1, 9].Value = "Áp dụng";

            // set style name column
            using (var rng = ws.Cells["A1:I1"])
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
                .ExecuteAsync<List<AssetCategoryModel>>("/asset-category/get-available", null, Method.GET, ApiHosts.Asset);
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
            ViewData["assetCategores"] = categories;
        }
        private static string GetReasonWH(IEnumerable<SelectListItem> vs, string Show)
        {
            if (Show is null)
                return "";
            var check = vs.FirstOrDefault(x => x.Text.Equals(Show));
            return check is null ? "" : check.Value;
        }
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
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
            var dtInfo = new DataTable("AssetCategory");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Danh sách nhập lỗi loại tài sản";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-assetcategory.xlsx");
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
