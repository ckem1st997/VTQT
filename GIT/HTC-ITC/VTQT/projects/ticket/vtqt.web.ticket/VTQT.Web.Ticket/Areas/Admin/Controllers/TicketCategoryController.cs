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
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;
using VTQT.Web.Ticket.Models;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class TicketCategoryController : AdminMvcController
    {
        #region Fields
        
        #endregion

        #region Ctor
        public TicketCategoryController()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Trang Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var searchModel = new TicketCategorySearchModel();
            ViewData["projects"] = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "test",
                    Text = "Test"
                }
            };
            return View(searchModel);
        }

        /// <summary>
        /// Khởi tạo loại ticket
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<TicketCategoryModel>("ticket-category/create", null, Method.GET, ApiHosts.Ticket);

            var model = res.data;
            ViewData["projects"] = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "test",
                    Text = "Test"
                }
            };

            return View(model);
        }

        /// <summary>
        /// Thêm mới loại ticket
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("ticket-category/create", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu loại ticket cần chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<TicketCategoryModel>("ticket-category/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            ViewData["projects"] = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "test",
                    Text = "Test"
                }
            };

            return View(model);
        }

        /// <summary>
        /// Chỉnh sửa loại ticket
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(TicketCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("ticket-category/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Xóa danh sách loại ticket
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
                .ExecuteAsync("ticket-category/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Kích hoạt, ngưng kích hoạt loại ticket
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
                .ExecuteAsync("ticket-category/activates", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy chi tiết loại ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<TicketCategoryModel>("ticket-category/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            ViewData["projects"] = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "test",
                    Text = "Test"
                }
            };

            return View(model);
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách loại ticket
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             TicketCategorySearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<TicketCategoryModel>>("ticket-category/get", searchModel, Method.GET, ApiHosts.Ticket);
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

        #endregion

        #region Import

        #endregion

        #region Export

        public async Task<ActionResult> ExportOrder(TicketCategorySearchModel model)
        {
            var fileName = "danh-sach-loại-ticket.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<TicketCategoryModel>>("/ticket-category/get", model, Method.GET, ApiHosts.Ticket);
            var orders = res.data;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách loại Ticket");
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
            ws.Cells[2, 2].Value = "Mã loại Ticket";
            ws.Cells[2, 3].Value = "Tên loại Ticket";
            ws.Cells[2, 4].Value = "Dự án";
            ws.Cells[2, 5].Value = "Trạng thái";
            var i = 3;

            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.Code;
                    ws.Cells[i, 3].Value = order.Name;
                    ws.Cells[i, 4].Value = order.Project.Name;
                    ws.Cells[i, 5].Value = order.Inactive ? "Ngừng kích hoạt" : "Đã kích hoạt";
                    i++;
                }


            // set style title

            using (var rng = ws.Cells["D1:E1"])
            {
                rng.Value = "Danh sách loại Ticket";
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

        public async Task<ActionResult> GetExcelReport(TicketCategorySearchModel model)
        {
            var res = await ApiHelper.ExecuteAsync<List<TicketCategoryModel>>("/ticket-category/get", model, Method.GET, ApiHosts.Ticket);
            var data = res?.data;
            var stt = 1;
            var models = new List<TicketCategoryExportModel>();

            if (data?.Count > 0)
            {
                foreach (var e in data)
                {
                    var m = new TicketCategoryExportModel
                    {
                        STT = stt,
                        Code = e.Code,
                        Name = e.Name,
                        ProjectId = e.ProjectId,
                        Inactive = e.Inactive ? "Chưa kích hoạt" : "Đã kích hoạt"
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("TicketCategory");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo loại Ticket";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/TicketCategory_vi.xlsx");
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

            var fileDownloadName = "bao_cao_loại_ticket.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
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
        #endregion
    }
}
