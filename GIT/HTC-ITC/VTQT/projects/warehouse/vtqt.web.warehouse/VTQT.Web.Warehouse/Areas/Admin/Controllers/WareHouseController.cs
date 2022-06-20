using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class WareHouseController : AdminMvcController
    {
        #region Ctor

        private readonly IWorkContext _workContext;

        public WareHouseController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion Ctor

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new WareHouseSearchModel();

            return View(searchModel);
        }

        public IActionResult SetListRole(string id)
        {
            var model = new WareHouseModel();
            model.Id = id;
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseModel>("/warehouse/details", new { id = id }, Method.GET,
                ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> ListRoleByUser()
        {
            var model = new WareHouseUserModel();
            return View(model);
        }

        public async Task<IActionResult> ListRoleByUserRead()
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseUserModel>>("/warehouse-user/get-role-by-user", null,
                Method.GET,
                ApiHosts.Warehouse);
            var model = res.data == null ? new List<WareHouseUserModel>() : res.data;
            var result = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };
            return Ok(result);
        }



        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<WareHouseModel>("/warehouse/create", null, Method.GET,
                ApiHosts.Warehouse);
            // var tree = await ApiHelper.ExecuteAsync<IEnumerable<WareHouseModel>>("/warehouse/get-select-tree", null,
            //     Method.GET,
            //     ApiHosts.Warehouse);
            var model = res.data;
            // var select = tree?.data;
            // if (select != null)
            //     model.AvailableWareHouses = select.Select(s => new SelectListItem
            //     {
            //         Value = s.Id,
            //         Text = s.Name
            //     }).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WareHouseModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/warehouse/create", model, Method.POST, ApiHosts.Warehouse);
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
            var res = await ApiHelper.ExecuteAsync<WareHouseModel>("/warehouse/edit", new { id = id }, Method.GET,
                ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WareHouseModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/warehouse/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/warehouse/deletes", ids, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/warehouse/activates", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> SetListRoleByUser(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/warehouse-user/setlist-role-by-user", model, Method.POST,
                ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }


        public async Task<IActionResult> CheckRole(string idUser, string idWareHouse)
        {
            if (string.IsNullOrEmpty(idUser) || string.IsNullOrEmpty(idWareHouse))
                return Ok(new XBaseResult { success = false });
            var res = await ApiHelper.ExecuteAsync($"warehouse-user/check?idUser={idUser}&idWareHouse={idWareHouse}",
                null, Method.GET, ApiHosts.Warehouse);
            return Ok(res);
        }

        #endregion Methods

        #region Lists

        // TODO-Remove
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
            WareHouseSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse/get", searchModel, Method.GET,
                ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        public IActionResult AccessDeniedPath()
        {
            return View();
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetUser([DataSourceRequest] DataSourceRequest request,
            WareHouseSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET,
                ApiHosts.Master);
            var listRole = await ApiHelper.ExecuteAsync<List<WareHouseUserModel>>(
                "/warehouse-user/get-role?idWareHouse=" + searchModel.Keywords + "", null, Method.GET,
                ApiHosts.Warehouse);
            var data = res?.data;
            if (data != null && listRole?.data != null)
                foreach (var item in data)
                {
                    item.Active = true;
                    foreach (var item1 in listRole?.data)
                    {
                        if (item1.UserId.Equals(item.Id))
                        {
                            item.Active = false;
                            break;
                        }
                    }
                }

            var result = new DataSourceResult
            {
                Data = data.OrderBy(x => x.Active),
                Total = data.Count()
            };
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> SetRole(string idUser, string idWareHouse)
        {
            var list = new List<WareHouseUserModel>();
            var model = new WareHouseUserModel()
            {
                UserId = idUser,
                WarehouseId = idWareHouse,
                CreatedBy = _workContext.UserId
            };
            list.Add(model);
            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/warehouse-user/create", list, Method.POST,
                ApiHosts.Warehouse);
            var data = res?.data;

            var result = new XBaseResult
            {
                data = data,
                success = res.success
            };
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> DeSetRole(string idUser, string idWareHouse)
        {
            var res = await ApiHelper.ExecuteAsync<List<UserModel>>(
                $"/warehouse-user/delete?idWareHouse={idWareHouse}&idUser={idUser}", null, Method.DELETE,
                ApiHosts.Warehouse);
            var data = res?.data;

            var result = new XBaseResult
            {
                data = data,
                success = res.success
            };
            return Ok(result);
        }

        #endregion Lists


        #region export

        public async Task<ActionResult> ExportOrder(WareHouseSearchModel model)
        {
            var fileName = "danh-sach-kho.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse/get", model, Method.GET,
                ApiHosts.Warehouse);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách kho");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 10;
            ws.Column(2).Width = 15;
            ws.Column(4).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Mã kho";
            ws.Cells[2, 3].Value = "Tên kho";
            ws.Cells[2, 4].Value = "Địa chỉ";
            ws.Cells[2, 5].Value = "Mô tả";
            ws.Cells[2, 6].Value = "Trạng thái";
            var i = 3;
            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.Code;
                    ws.Cells[i, 3].Value = order.Name;
                    ws.Cells[i, 4].Value = order.Address;
                    ws.Cells[i, 5].Value = order.Description;
                    ws.Cells[i, 6].Value = order.Inactive ? "Ngừng kích hoạt" : "Đã kích hoạt";
                    i++;
                }

            // set style title

            using (var rng = ws.Cells["D1"])
            {
                rng.Value = "Danh sách kho";
                rng.Merge = true;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White); //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            }

            // set style name column
            using (var rng = ws.Cells["A2:F2"])
            {
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129,
                    189)); //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
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

        public async Task<ActionResult> GetExcelReport(WareHouseSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse/get", searchModel, Method.GET,
                ApiHosts.Warehouse);
            var data = res?.data;

            var stt = 1;
            var models = new List<WareHouseExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new WareHouseExportModel
                    {
                        STT = stt,
                        Code = order.Code,
                        Name = order.Name,
                        Address = order.Address,
                        Description = order.Description,
                        Inactive = order.Inactive ? "Chưa kích hoạt" : "Đã kích hoạt"
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("WareHouse");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết kho";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/WareHouse_vi.xlsx");
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

            var fileDownloadName = "bao_cao_chi_tiet_kho.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }

        #endregion export

        #region ImportExcel

        public IActionResult ImportExcel()
        {
            var ware = new WareHouseModel();
            return View(ware);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public virtual async Task<IActionResult> ImportExcel([FromBody] IEnumerable<WareHouseModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));
            int countColumn = 0;
            List<string> vs = new List<string>();
            Dictionary<int, string> openWith = new Dictionary<int, string>();

            #region GetAvailable

            #endregion

            var res = await ApiHelper.ExecuteAsync("/warehouse/create-batch", models, Method.POST, ApiHosts.Warehouse);
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

        public async Task<ActionResult> ExportWareHouse()
        {
            var fileName = "danh-sach-kho.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách kho");
            ws.DefaultColWidth = 20;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            ws.Cells[1, 1].Value = "Mã kho";
            ws.Cells[1, 1].AddComment("Nhập mã kho vào hệ thống");

            ws.Cells[1, 2].Value = "Tên kho";
            ws.Cells[1, 2].AddComment("Nhập tên kho vào hệ thống");

            ws.Cells[1, 3].Value = "Địa chỉ";
            ws.Cells[1, 3].AddComment("Nhập địa chỉ vào hệ thống");

            ws.Cells[1, 4].Value = "Mô tả";
            ws.Cells[1, 4].AddComment("Nhập mô tả vào hệ thống");

            ws.Cells[1, 5].Value = "Áp dụng";
            ws.Cells[1, 5]
                .AddComment("Chú ý nhập áp dụng vào hệ thống mặc định là (Đã kích hoạt hoặc Ngừng Kích hoạt)");


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
            infoRow["Title"] = "Danh sách nhập kho";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);
            var tmpPath = CommonHelper.MapPath("/wwwroot/ImportExcel/err-warehouse.xlsx");
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