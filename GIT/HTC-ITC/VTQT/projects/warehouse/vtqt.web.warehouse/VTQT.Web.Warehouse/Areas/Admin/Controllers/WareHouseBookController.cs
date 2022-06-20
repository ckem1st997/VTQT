using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core.Domain.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.SharedMvc.Warehouse.Models.WareHouse;
using VTQT.Web.Framework.Controllers;
using VTQT.Core.Domain.Warehouse.Enum;
using System.Globalization;
using VTQT.Core;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.Utilities;
using VTQT.Web.Framework.Security;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Reflection;
using System.ComponentModel;
using System;
using System.IO;
using System.Net;
using System.Data;
using Aspose.Words;
using Microsoft.AspNetCore.Hosting;
using VTQT.Web.Warehouse.Models;
using Aspose.Cells;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class WareHouseBookController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public WareHouseBookController(IWorkContext workContext)
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
            var checkRole = await ApiHelper.ExecuteAsync<List<WareHouseModel>>("/warehouse-user/check-role-user?idUser=" + _workContext.UserId + "", null, Method.GET, ApiHosts.Warehouse);

            if (!checkRole.success)
                return RedirectToAction("AccessDeniedPath", "WareHouse");
            var searchModel = new VoucherWareHouseSearchModel();
            GetAvailableData();

            var resLastSelected = await ApiHelper.
                ExecuteAsync<string>($"/warehouse/get-last-selected/?appId=5&userId={_workContext.UserId}&path=/Admin/WareHouseBook", null, Method.GET, ApiHosts.Warehouse);

            if (!string.IsNullOrEmpty(resLastSelected.data))
            {
                searchModel.WareHouseId = resLastSelected.data;
            }

            return View(searchModel);
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

        /// <summary>
        /// Chuyển tới trang chỉnh sửa phiếu nhập/ xuất tham số Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<string> Edit(string id)
        {
            const string failed = "0";
            const string redirectToInward = "1";
            const string redirectToOutward = "2";
            if (string.IsNullOrEmpty(id))
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return failed;
            }

            var resIn = await ApiHelper.ExecuteAsync<InwardModel>("/inward/details", new { id = id }, Method.GET, ApiHosts.Warehouse);

            return resIn.data != null ? redirectToInward : redirectToOutward;
        }

        /// <summary>
        /// Gọi Api xóa phiếu nhập/ xuất
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
                .ExecuteAsync("/warehouse-book/deletes", ids, Method.POST, ApiHosts.Warehouse);
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
        /// Lấy danh sách phiếu nhập/ xuất kho phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            VoucherWareHouseSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var res = await ApiHelper
                .ExecuteAsync<List<VoucherWareHouseModel>>
                    ("/warehouse-book/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var resLastSelected = await ApiHelper
                    .ExecuteAsync<string>($"/warehouse/update-last-selected/?appId=5&userId={_workContext.UserId}&path=/Admin/WareHouseBook&warehouseId={searchModel.WareHouseId}", null, Method.POST, ApiHosts.Warehouse);

            if (data?.Count > 0)
            {
                var listUser = await ApiHelper.ExecuteAsync<InwardModel>("/inward/create", null, Method.GET, ApiHosts.Warehouse);

                data.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedBy))
                    {
                        var userName = listUser.data.AvailableCreatedBy.FirstOrDefault(z => z.Value.Equals(x.CreatedBy)).Text;
                        x.CreatedBy = userName.Split("-").Length>0? userName.Split("-")[0]:string.Empty;
                    }
                    if (!string.IsNullOrEmpty(x.VoucherType))
                    {
                        if (x.VoucherType.Equals(nameof(Inward)))
                        {
                            x.VoucherType = string.Format(T("Common.ImportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate.ToLocalTime());
                            x.SelectedInwardReason = x.Reason.ToString();
                        }
                        else if (x.VoucherType.Equals(nameof(Outward)))
                        {
                            x.VoucherType = string.Format(T("Common.ExportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate.ToLocalTime());
                            x.SelectedOutwardReason = x.Reason.ToString();
                        }
                    }
                    //ModifiedBy
                    if (!string.IsNullOrEmpty(x.ModifiedBy))
                    {
                        var userName1 = listUser.data.AvailableCreatedBy.FirstOrDefault(z => z.Value.Equals(x.ModifiedBy)).Text;
                        x.ModifiedBy = userName1.Split("-").Length > 0 ? userName1.Split("-")[0] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(x.VoucherType))
                    {
                        if (x.VoucherType.Equals(nameof(Inward)))
                        {
                            x.VoucherType = string.Format(T("Common.ImportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate.ToLocalTime());
                            x.SelectedInwardReason = x.Reason.ToString();
                        }
                        else if (x.VoucherType.Equals(nameof(Outward)))
                        {
                            x.VoucherType = string.Format(T("Common.ExportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate.ToLocalTime());
                            x.SelectedOutwardReason = x.Reason.ToString();
                        }
                    }
                });
            }

            GetAvailableData();
            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }
        #endregion

        #region Utilities
        private void GetAvailableData()
        {
            var vouchers = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)VoucherType.Inward).ToString(),
                    Text = VoucherType.Inward.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)VoucherType.Outward).ToString(),
                    Text = VoucherType.Outward.GetEnumDescription()
                }
            };
            ViewData["vouchers"] = vouchers;

            var inwardReason = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)InwardReason.Purchase).ToString(),
                    Text = InwardReason.Purchase.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)InwardReason.Revoke).ToString(),
                    Text = InwardReason.Revoke.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)InwardReason.Return).ToString(),
                    Text = InwardReason.Return.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)InwardReason.Other).ToString(),
                    Text = InwardReason.Other.GetEnumDescription()
                }
            };
            ViewData["inwardReason"] = inwardReason;

            var outwardReason = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Office).ToString(),
                    Text = OutwardReason.Office.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Station).ToString(),
                    Text = OutwardReason.Station.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Project).ToString(),
                    Text = OutwardReason.Project.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Customer).ToString(),
                    Text = OutwardReason.Customer.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Items).ToString(),
                    Text = OutwardReason.Items.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Tool).ToString(),
                    Text = OutwardReason.Tool.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.ChangeWarehouse).ToString(),
                    Text = OutwardReason.ChangeWarehouse.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)OutwardReason.Other).ToString(),
                    Text = OutwardReason.Other.GetEnumDescription()
                }
            };
            ViewData["outwardReason"] = outwardReason;
        }
        #endregion

        #region export
        public async Task<ActionResult> ExportOrder(VoucherWareHouseSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
               .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);
            var fileName = "danh-sach-so-kho.xlsx";
            var res = await ApiHelper.ExecuteAsync<List<VoucherWareHouseModel>>("/warehouse-book/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var orders = res.data;

            if (orders?.Count > 0)
            {
                orders.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.VoucherType))
                    {
                        if (x.VoucherType.Equals(nameof(Inward)))
                        {
                            x.VoucherType = string.Format(T("Common.ImportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate);
                            x.SelectedInwardReason = x.Reason.ToString();
                        }
                        else if (x.VoucherType.Equals(nameof(Outward)))
                        {
                            x.VoucherType = string.Format(T("Common.ExportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate);
                            x.SelectedOutwardReason = x.Reason.ToString();
                        }
                    }
                });
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            //Create the worksheet
            var ws = pck.Workbook.Worksheets.Add("Danh sách sổ kho");
            ws.DefaultColWidth = 20;
            ws.DefaultRowHeight = 15;
            ws.Cells.Style.WrapText = true;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Column(1).Width = 10;
            ws.Column(2).Width = 15;
            ws.Column(4).Width = 15;
            ws.Column(9).Width = 30;
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            ws.Cells[2, 1].Value = "STT";
            ws.Cells[2, 2].Value = "Loại phiếu";
            ws.Cells[2, 3].Value = "Số phiếu";
            ws.Cells[2, 4].Value = "Ngày";
            ws.Cells[2, 5].Value = "Người tạo";
            ws.Cells[2, 6].Value = "Bên giao";
            ws.Cells[2, 7].Value = "Bên nhận";
            ws.Cells[2, 8].Value = "Lý do nhập";
            ws.Cells[2, 9].Value = "Lý do xuất";
            var i = 3;
            int team;
            if (orders != null)
                foreach (var order in orders)
                {
                    ws.Cells[i, 1].Value = i - 2;
                    ws.Cells[i, 2].Value = order.VoucherType;
                    ws.Cells[i, 3].Value = order.VoucherCode;
                    ws.Cells[i, 4].Value = order.StrVoucherDate;
                    ws.Cells[i, 5].Value = order.CreatedBy;
                    ws.Cells[i, 6].Value = order.Deliver;
                    ws.Cells[i, 7].Value = order.Receiver;
                    ws.Cells[i, 8].Value = GetReason(out team, order.SelectedInwardReason, GetEnumDescription((InwardReason)order.Reason));
                    ws.Cells[i, 9].Value = GetReason(out team, order.SelectedOutwardReason, GetEnumDescription((OutwardReason)order.Reason));
                    i++;
                }

            // set style title

            using (var rng = ws.Cells["D1:E1"])
            {
                rng.Value = "Danh sách sổ kho";
                rng.Merge = true;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            }

            // set style name column
            using (var rng = ws.Cells["A2:I2"])
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

        private static string GetReason(out int team, string Type, string Show)
        {
            return int.TryParse(Type, out team) ? Show : "";
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

        public async Task<ActionResult> GetExcelReport(VoucherWareHouseSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
              .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);
            var res = await ApiHelper.ExecuteAsync<List<VoucherWareHouseModel>>("/warehouse-book/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res?.data;

            var listUser = await ApiHelper.ExecuteAsync<InwardModel>("/inward/create", null, Method.GET, ApiHosts.Warehouse);


            if (data?.Count > 0)
            {
                data.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.CreatedBy))
                    {
                        var userName = listUser.data.AvailableCreatedBy.FirstOrDefault(z => z.Value.Equals(x.CreatedBy)).Text;
                        x.CreatedBy = userName.Split("-").Length > 0 ? userName.Split("-")[0] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(x.VoucherType))
                    {
                        if (x.VoucherType.Equals(nameof(Inward)))
                        {
                            x.VoucherType = string.Format(T("Common.ImportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate);
                            x.SelectedInwardReason = x.Reason.ToString();
                        }
                        else if (x.VoucherType.Equals(nameof(Outward)))
                        {
                            x.VoucherType = string.Format(T("Common.ExportVoucher"));
                            x.StrVoucherDate = string.Format("{0:dd/MM/yyyy}", x.VoucherDate);
                            x.SelectedOutwardReason = x.Reason.ToString();
                        }
                    }
                });
            }
            GetAvailableData();
            var getInWard = (IEnumerable<SelectListItem>)ViewData["inwardReason"];
            var getOutWard = (IEnumerable<SelectListItem>)ViewData["outwardReason"];
            var stt = 1;
            var models = new List<WareHouseBookExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new WareHouseBookExportModel
                    {
                        STT = stt,
                        VoucherType = order.VoucherType,
                        VoucherCode = order.VoucherCode,
                        StrVoucherDate = order.StrVoucherDate,
                        CreatedBy = order.CreatedBy,
                        Deliver = order.Deliver,
                        Receiver = order.Receiver,
                        InwardReason = GetReason(getInWard, order.SelectedInwardReason),
                        OutwardReason = GetReason(getOutWard, order.SelectedOutwardReason)
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("WareHouseBook");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo chi tiết sổ kho";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/WareHouseBook_vi.xlsx");
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

            var fileDownloadName = "bao_cao_chi_tiet_so_kho.xlsx";

            return Json(new { FileGuid = handler, FileName = fileDownloadName });
        }



        #endregion


        #region ExportWhSaigon
        public async Task<ActionResult> ExportInwardWHsaigon(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<InwardModel>("/print/get-by-id-to-in?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<InwardDetailModel>>($"/inward/detail-get?InwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion

            #region Order information

            var exportDate = DateTime.Now;
            var inDay = exportDate.Day;
            var inMouth = exportDate.Month;
            var inYear = exportDate.Year;
            var vDay = entity.CreatedDate.Day;
            var vMouth = entity.CreatedDate.Month;
            var vYear = entity.CreatedDate.Year - 2000 < 10 ? "0+" + (entity.CreatedDate.Year - 2000) + "" : "" + (entity.CreatedDate.Year - 2000) + "";
            var voucherCode = entity.VoucherCode;
            var description = entity.Description;
            var voucher = string.IsNullOrEmpty(entity.Voucher) ? entity.VoucherCode : entity.Voucher;
            var wareHouse = entity.WareHouse is null ? "" : entity.WareHouse.Name;
            var address = entity.WareHouse is null ? "" : entity.WareHouse.Address;
            var sumNone = 0;
            var priceString = "0";
            var sumUIPrice = 0m;
            var sumAmount = 0m;
            var sumUIQuantity = 0m;
            var Deliver = entity.Deliver;

            #endregion
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            #region Invoice details

            DataTable dt = new DataTable { TableName = "Inward" };
            dt.Columns.Add("S", typeof(string));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("QNone", typeof(string));
            dt.Columns.Add("UIQ", typeof(string));
            dt.Columns.Add("UIP", typeof(string));
            dt.Columns.Add("Am", typeof(string));

            int i = 0;
            foreach (var item in result.data)
            {
                i++;

                DataRow dr = dt.NewRow();
                dr["S"] = i;
                dr["ItemName"] = item.ItemName;
                dr["Code"] = item.WareHouseItem?.Code;
                dr["Unit"] = item.UnitName;
                dr["QNone"] = 0;
                dr["UIQ"] = item.UIQuantity;
                dr["UIP"] = FormatNumbe(item.UIPrice);
                dr["Am"] = FormatNumbe(item.Amount);

                sumAmount = sumAmount + item.Amount;
                sumUIPrice = sumUIPrice + item.UIPrice;
                sumUIQuantity = sumUIQuantity + item.UIQuantity;
                dt.Rows.Add(dr);
                if (i > 20)
                    break;
            }
            if (sumAmount > 0)
                priceString = NumberToText((double)sumAmount);
            #endregion

            #region Export word

            var bcStream = new MemoryStream();
            string path = CommonHelper.MapPath("/wwwroot/Word/inphieukhosaigon.doc");
            var doc = new Aspose.Words.Document(Path.GetFullPath(path).Replace("~\\", ""));

            doc.MailMerge.Execute(new[]
            {
                "ExportDate","inDay" ,"inMouth","inYear" ,"vDay" ,"vMouth" ,"vYear" ,"voucherCode","description","voucher" ,"wareHouse","address" , "sumN","sumUIP" ,"sumAm","sumUIQ","PriceString","Deliver"
            },
            new object[]
            {
                    exportDate,
                    inDay,
                    inMouth,
                    inYear,
                    vDay,
                    vMouth,
                    vYear,
                    voucherCode,
                    description,
                    voucher,
                    wareHouse,
                    address,
                    sumNone,
                    sumUIPrice,
                    sumAmount,
                    sumUIQuantity,
                    priceString,
                    Deliver
            });

            doc.MailMerge.ExecuteWithRegions(dt);
            doc.MailMerge.DeleteFields();

            var dstStream = new MemoryStream();
            var docSave = doc.Save(dstStream, Aspose.Words.SaveFormat.Pdf);
            dstStream.Position = 0;

            return File(dstStream, docSave.ContentType, "phieu-nhap-" + entity.VoucherCode + ".pdf");

            #endregion

        }
        #endregion

        public async Task<ActionResult> ExportInward(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<InwardModel>("/print/get-by-id-to-in?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<InwardDetailModel>>($"/inward/detail-get?InwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion

            #region Order information

            var exportDate = DateTime.Now;
            var inDay = entity.VoucherDate.Day;
            var inMouth = entity.VoucherDate.Month;
            var inYear = entity.VoucherDate.Year;
            var vDay = entity.CreatedDate.Day;
            var vMouth = entity.CreatedDate.Month;
            var vYear = entity.CreatedDate.Year - 2000 < 10 ? "0+" + (entity.CreatedDate.Year - 2000) + "" : "" + (entity.CreatedDate.Year - 2000) + "";
            var voucherCode = entity.VoucherCode;
            var voucher = string.IsNullOrEmpty(entity.Voucher) ? entity.VoucherCode : entity.Voucher;
            var description = entity.Description;
            var wareHouse = entity.WareHouse is null ? "" : entity.WareHouse.Name;
            var address = entity.WareHouse is null ? "" : entity.WareHouse.Address;
            var sumNone = 0;
            var priceString = "0";
            var sumUIPrice = 0m;
            var sumAmount = 0m;
            var sumUIQuantity = 0m;
            var Deliver = entity.Deliver;

            #endregion
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            #region Invoice details

            DataTable dt = new DataTable { TableName = "Inward" };
            dt.Columns.Add("S", typeof(string));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("QNone", typeof(string));
            dt.Columns.Add("UIQ", typeof(string));
            dt.Columns.Add("UIP", typeof(string));
            dt.Columns.Add("Am", typeof(string));

            int i = 0;
            foreach (var item in result.data)
            {
                i++;

                DataRow dr = dt.NewRow();
                dr["S"] = i;
                dr["ItemName"] = item.ItemName;
                dr["Code"] =item.WareHouseItem?.Code;
                dr["Unit"] = item.UnitName;
                dr["QNone"] = 0;
                dr["UIQ"] = item.UIQuantity;
                dr["UIP"] = FormatNumbe(item.UIPrice);
                dr["Am"] = FormatNumbe(item.Amount);

                sumAmount = sumAmount + item.Amount;
                sumUIPrice = sumUIPrice + item.UIPrice;
                sumUIQuantity = sumUIQuantity + item.UIQuantity;
                dt.Rows.Add(dr);
                if (i > 20)
                    break;
            }
            if (sumAmount > 0)
                priceString = NumberToText((double)sumAmount);
            #endregion

            #region Export word

            var bcStream = new MemoryStream();
            string path = CommonHelper.MapPath("/wwwroot/Word/inphieu1.docx");
            var doc = new Aspose.Words.Document(Path.GetFullPath(path).Replace("~\\", ""));

            doc.MailMerge.Execute(new[]
            {
                "ExportDate","inDay" ,"inMouth","inYear" ,"vDay" ,"vMouth" ,"vYear" ,"voucherCode","voucher", "description" ,"wareHouse","address" , "sumN","sumUIP" ,"sumAm","sumUIQ","PriceString","Deliver"
            },
            new object[]
            {
                    exportDate,
                    inDay,
                    inMouth,
                    inYear,
                    vDay,
                    vMouth,
                    vYear,
                    voucherCode,
                    voucher,
                    description,
                    wareHouse,
                    address,
                    sumNone,
                    sumUIPrice,
                    sumAmount,
                    sumUIQuantity,
                    priceString,
                    Deliver
            });

            doc.MailMerge.ExecuteWithRegions(dt);
            doc.MailMerge.DeleteFields();

            var dstStream = new MemoryStream();
            var docSave = doc.Save(dstStream, Aspose.Words.SaveFormat.Pdf);
            dstStream.Position = 0;

            return File(dstStream, docSave.ContentType, "phieu-nhap-" + entity.VoucherCode + ".pdf");

            #endregion

        }

        #region Inphieu Test

        public async Task<ActionResult> ExportInwardTest(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<InwardModel>("/print/get-by-id-to-in?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<InwardDetailModel>>($"/inward/detail-get?InwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion

            #region Order information

            var exportDate = DateTime.Now;
            var inDay = entity.VoucherDate.Day;
            var inMouth = entity.VoucherDate.Month;
            var inYear = entity.VoucherDate.Year;
            var vDay = entity.CreatedDate.Day;
            var vMouth = entity.CreatedDate.Month;
            var vYear = entity.CreatedDate.Year - 2000 < 10 ? "0+" + (entity.CreatedDate.Year - 2000) + "" : "" + (entity.CreatedDate.Year - 2000) + "";
            var voucherCode = entity.VoucherCode;
            var voucher = string.IsNullOrEmpty(entity.Voucher) ? entity.VoucherCode : entity.Voucher;
            var description = entity.Description;
            var wareHouse = entity.WareHouse is null ? "" : entity.WareHouse.Name;
            var address = entity.WareHouse is null ? "" : entity.WareHouse.Address;
            var sumNone = 0;
            var priceString = "0";
            var sumUIPrice = 0m;
            var sumAmount = 0m;
            var sumUIQuantity = 0m;
            var Deliver = entity.Deliver;

            #endregion
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            #region Invoice details

            DataTable dt = new DataTable { TableName = "Inward" };
            dt.Columns.Add("S", typeof(string));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("QNone", typeof(string));
            dt.Columns.Add("UIQ", typeof(string));
            dt.Columns.Add("UIP", typeof(string));
            dt.Columns.Add("Am", typeof(string));

            int i = 0;
            foreach (var item in result.data)
            {
                i++;

                DataRow dr = dt.NewRow();
                dr["S"] = i;
                dr["ItemName"] = item.ItemName;
                dr["Code"] = item.WareHouseItem?.Code;
                dr["Unit"] = item.UnitName;
                dr["QNone"] = 0;
                dr["UIQ"] = item.UIQuantity;
                dr["UIP"] = FormatNumbe(item.UIPrice);
                dr["Am"] = FormatNumbe(item.Amount);

                sumAmount = sumAmount + item.Amount;
                sumUIPrice = sumUIPrice + item.UIPrice;
                sumUIQuantity = sumUIQuantity + item.UIQuantity;
                dt.Rows.Add(dr);
                if (i > 20)
                    break;
            }
            if (sumAmount > 0)
                priceString = NumberToText((double)sumAmount);
            #endregion

            #region Export word

            var bcStream = new MemoryStream();
            string path = CommonHelper.MapPath("/wwwroot/Word/inphieutest.doc");
            var doc = new Aspose.Words.Document(Path.GetFullPath(path).Replace("~\\", ""));

            doc.MailMerge.Execute(new[]
            {
                "ExportDate","inDay" ,"inMouth","inYear" ,"vDay" ,"vMouth" ,"vYear" ,"voucherCode","voucher", "description" ,"wareHouse","address" , "sumN","sumUIP" ,"sumAm","sumUIQ","PriceString","Deliver"
            },
            new object[]
            {
                    exportDate,
                    inDay,
                    inMouth,
                    inYear,
                    vDay,
                    vMouth,
                    vYear,
                    voucherCode,
                    voucher,
                    description,
                    wareHouse,
                    address,
                    sumNone,
                    sumUIPrice,
                    sumAmount,
                    sumUIQuantity,
                    priceString,
                    Deliver
            });

            doc.MailMerge.ExecuteWithRegions(dt);
            doc.MailMerge.DeleteFields();

            var dstStream = new MemoryStream();
            var docSave = doc.Save(dstStream, Aspose.Words.SaveFormat.Pdf);
            dstStream.Position = 0;

            return File(dstStream, docSave.ContentType, "phieu-nhap-" + entity.VoucherCode + ".pdf");

            #endregion

        }

        #endregion


        #region ExportWHSaigon
        public async Task<ActionResult> ExportOutwardWHSaigon(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/print/get-by-id-to-out?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<OutwardDetailModel>>($"/outward/detail-get?OutwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion

            #region Order information

            var exportDate = DateTime.Now;
            var inDay = exportDate.Day;
            var inMouth = exportDate.Month;
            var inYear = exportDate.Year;
            var vDay = entity.CreatedDate.Day;
            var vMouth = entity.CreatedDate.Month;
            var vYear = entity.CreatedDate.Year - 2000 < 10 ? "0+" + (entity.CreatedDate.Year - 2000) + "" : "" + (entity.CreatedDate.Year - 2000) + "";
            var voucherCode = entity.VoucherCode;
            var voucherCodeReality = string.IsNullOrEmpty(entity.VoucherCodeReality) ? entity.VoucherCode : entity.VoucherCodeReality;
            var description = entity.Description;
            var wareHouse = entity.WareHouse is null ? "" : entity.WareHouse.Name;
            var address = entity.WareHouse is null ? "" : entity.WareHouse.Address;
            var receiver = entity.Receiver;
            var sumNone = 0;
            var priceString = "0";
            var sumUIPrice = 0m;
            var sumAmount = 0m;
            var sumUIQuantity = 0m;


            #endregion
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            #region Invoice details

            DataTable dt = new DataTable { TableName = "Outward" };
            dt.Columns.Add("S", typeof(string));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("QNone", typeof(string));
            dt.Columns.Add("UIQ", typeof(string));
            dt.Columns.Add("UIPrice", typeof(string));
            dt.Columns.Add("Amount", typeof(string));

            int i = 0;
            foreach (var item in result.data)
            {
                i++;

                DataRow dr = dt.NewRow();
                dr["S"] = i;
                dr["ItemName"] = item.ItemName;
                dr["Code"] = item.WareHouseItem?.Code;
                dr["Unit"] = item.UnitName;
                dr["QNone"] = 0;
                dr["UIQ"] = item.UIQuantity;
                dr["UIPrice"] = FormatNumbe(item.UIPrice);
                dr["Amount"] = FormatNumbe(item.Amount);

                sumAmount = sumAmount + item.Amount;
                sumUIPrice = sumUIPrice + item.UIPrice;
                sumUIQuantity = sumUIQuantity + item.UIQuantity;
                dt.Rows.Add(dr);
                if (i > 20)
                    break;
            }


            #endregion
            if (sumAmount > 0)
                priceString = NumberToText((double)sumAmount);
            #region Export word

            var bcStream = new MemoryStream();
            string path = CommonHelper.MapPath("/wwwroot/Word/xuatphieukhosaigon.doc");
            var doc = new Aspose.Words.Document(Path.GetFullPath(path).Replace("~\\", ""));

            doc.MailMerge.Execute(new[]
            {
                "ExportDate","inDay" ,"inMouth","inYear" ,"vDay" ,"vMouth" ,"vYear" ,"voucherCode","receiver","description","voucherCodeReality" ,"wareHouse","address" , "sumNone","sumUIPrice" ,"sumAmount","sumUIQuantity","PriceString"
            },
            new object[]
            {
                    exportDate,
                    inDay,
                    inMouth,
                    inYear,
                    vDay,
                    vMouth,
                    vYear,
                    voucherCode,
                    receiver,
                    description,
                    voucherCodeReality,
                    wareHouse,
                    address,
                    sumNone,
                    sumUIPrice,
                    sumAmount,
                    sumUIQuantity,
                    priceString
            });

            doc.MailMerge.ExecuteWithRegions(dt);
            doc.MailMerge.DeleteFields();

            var dstStream = new MemoryStream();
            var docSave = doc.Save(dstStream, Aspose.Words.SaveFormat.Pdf);
            dstStream.Position = 0;

            return File(dstStream, docSave.ContentType, "phieu-xuat-" + entity.VoucherCode + ".pdf");

            #endregion

        }
        #endregion

        public async Task<ActionResult> ExportOutward(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/print/get-by-id-to-out?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<OutwardDetailModel>>($"/outward/detail-get?OutwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion

            #region Order information

            var exportDate = DateTime.Now;
            var inDay = entity.VoucherDate.Day;
            var inMouth = entity.VoucherDate.Month;
            var inYear = entity.VoucherDate.Year;
            var vDay = entity.CreatedDate.Day;
            var vMouth = entity.CreatedDate.Month;
            var vYear = entity.CreatedDate.Year - 2000 < 10 ? "0+" + (entity.CreatedDate.Year - 2000) + "" : "" + (entity.CreatedDate.Year - 2000) + "";
            //var voucherCode = entity.VoucherCode;
            var voucherCodeReality = string.IsNullOrEmpty(entity.VoucherCodeReality) ? entity.VoucherCode : entity.VoucherCodeReality;
            var receiver = entity.Receiver;
            var description = entity.Description;
            var wareHouse = entity.WareHouse is null ? "" : entity.WareHouse.Name;
            var address = entity.WareHouse is null ? "" : entity.WareHouse.Address;
            var sumNone = 0;
            var priceString = "0";
            var sumUIPrice = 0m;
            var sumAmount = 0m;
            var sumUIQuantity = 0m;


            #endregion
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            #region Invoice details

            DataTable dt = new DataTable { TableName = "Outward" };
            dt.Columns.Add("S", typeof(string));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("QNone", typeof(string));
            dt.Columns.Add("UIQ", typeof(string));
            dt.Columns.Add("UIPrice", typeof(string));
            dt.Columns.Add("Amount", typeof(string));

            int i = 0;
            foreach (var item in result.data)
            {
                i++;

                DataRow dr = dt.NewRow();
                dr["S"] = i;
                dr["ItemName"] = item.ItemName;
                dr["Code"] = item.WareHouseItem?.Code;
                dr["Unit"] = item.UnitName;
                dr["QNone"] = 0;
                dr["UIQ"] = item.UIQuantity;
                dr["UIPrice"] = FormatNumbe(item.UIPrice);
                dr["Amount"] = FormatNumbe(item.Amount);

                sumAmount = sumAmount + item.Amount;
                sumUIPrice = sumUIPrice + item.UIPrice;
                sumUIQuantity = sumUIQuantity + item.UIQuantity;
                dt.Rows.Add(dr);
                if (i > 20)
                    break;
            }


            #endregion
            if (sumAmount > 0)
                priceString = NumberToText((double)sumAmount);
            #region Export word

            var bcStream = new MemoryStream();
            string path = CommonHelper.MapPath("/wwwroot/Word/xuatphieu.doc");
            var doc = new Aspose.Words.Document(Path.GetFullPath(path).Replace("~\\", ""));

            doc.MailMerge.Execute(new[]
            {
                "exportDate","receiver","inDay" ,"inMouth","inYear" ,"vDay" ,"vMouth" ,"vYear" ,/*"voucherCode",*/"voucherCodeReality","description" ,"wareHouse","address" , "sumNone","sumUIPrice" ,"sumAmount","sumUIQuantity","PriceString"
            },
            new object[]
            {
                    exportDate,
                    receiver,
                    inDay,
                    inMouth,
                    inYear,
                    vDay,
                    vMouth,
                    vYear,
                    //voucherCode,
                    voucherCodeReality,
                    description,
                    wareHouse,
                    address,
                    sumNone,
                    sumUIPrice,
                    sumAmount,
                    sumUIQuantity,
                    priceString
            });

            doc.MailMerge.ExecuteWithRegions(dt);
            doc.MailMerge.DeleteFields();

            var dstStream = new MemoryStream();
            var docSave = doc.Save(dstStream, Aspose.Words.SaveFormat.Pdf);
            dstStream.Position = 0;

            return File(dstStream, docSave.ContentType, "phieu-xuat-" + entity.VoucherCode + ".pdf");

            #endregion

        }



        public async Task<ActionResult> ExportRecall(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<InwardModel>("/print/get-by-id-to-in?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<InwardDetailModel>>($"/inward/detail-get?InwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            #endregion
            var stt = 1;
            var models = new List<RecallModel>();
            if (result.data?.Count > 0)
            {
                foreach (var order in result.data)
                {
                    var m = new RecallModel()
                    {
                        STT = stt,
                        Name = order.ItemName,
                        UnitName = order.UnitName,
                        Quantity = order.UIQuantity,
                        Status = order.Status,
                        CodeItem = order.CodeItem
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("Inward");
            dtInfo.Columns.Add("Deliver", typeof(string));
            dtInfo.Columns.Add("Receiver", typeof(string));


            dtInfo.Columns.Add("DeliverDepartment", typeof(string));
            dtInfo.Columns.Add("DeliverAddress", typeof(string));
            dtInfo.Columns.Add("DeliverPhone", typeof(string));

            dtInfo.Columns.Add("ReceiverAddress", typeof(string));
            dtInfo.Columns.Add("ReceiverPhone", typeof(string));
            dtInfo.Columns.Add("ReceiverDepartment", typeof(string));

            dtInfo.Columns.Add("Description", typeof(string));
            dtInfo.Columns.Add("Reason", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Deliver"] = res.data.Deliver;
            infoRow["Receiver"] = res.data.Receiver;

            infoRow["DeliverDepartment"] = res.data.DeliverDepartment;
            infoRow["DeliverAddress"] = res.data.DeliverAddress;
            infoRow["DeliverPhone"] = res.data.DeliverPhone;

            infoRow["ReceiverAddress"] = res.data.ReceiverAddress;
            infoRow["ReceiverPhone"] = res.data.ReceiverPhone;
            infoRow["ReceiverDepartment"] = res.data.ReceiverDepartment;

            infoRow["Description"] = models.Any() ? res.data.Description : string.Empty;
            infoRow["Reason"] = GetEnumDescription(res.data.Reason);
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/BM10VTQTVTBBTHVT.xls");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();
            if (stt > 1)
            {
                Worksheet worksheet = wb.Worksheets[1];

                // Create a Cells object ot fetch all the cells.
                Cells cells = worksheet.Cells;

                // Merge some Cells (C6:E7) into a single C6 Cell.
                cells.Merge(28, 8, stt - 1, 1);

            }

            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", "bien_ban_thu_hoi_hang_hoa-" + res.data.VoucherCode + ".xlsx");

        } 
        
        
        public async Task<ActionResult> ExportDone(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/print/get-by-id-to-out?id=" + id + "", null, Method.GET, ApiHosts.Warehouse);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<OutwardDetailModel>>($"/outward/detail-get?OutwardId={id}", null, Method.GET, ApiHosts.Warehouse);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            #endregion
            var stt = 1;
            var models = new List<RecallModel>();
            if (result.data?.Count > 0)
            {
                foreach (var order in result.data)
                {
                    var m = new RecallModel()
                    {
                        STT = stt,
                        Name = order.ItemName,
                        UnitName = order.UnitName,
                        Quantity = order.UIQuantity,
                        Status = order.Status,
                        CodeItem = order.CodeItem,
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("Outward");
            dtInfo.Columns.Add("Deliver", typeof(string));
            dtInfo.Columns.Add("Receiver", typeof(string));

            dtInfo.Columns.Add("DeliverDepartment", typeof(string));
            dtInfo.Columns.Add("DeliverAddress", typeof(string));
            dtInfo.Columns.Add("DeliverPhone", typeof(string));

            dtInfo.Columns.Add("ReceiverAddress", typeof(string));
            dtInfo.Columns.Add("ReceiverPhone", typeof(string));


            dtInfo.Columns.Add("Description", typeof(string));
            dtInfo.Columns.Add("Reason", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Deliver"] = res.data.Deliver;
            infoRow["Receiver"] = res.data.Receiver;


            infoRow["DeliverDepartment"] = res.data.DeliverDepartment;
            infoRow["DeliverAddress"] = res.data.DeliverAddress;
            infoRow["DeliverPhone"] = res.data.DeliverPhone;

            infoRow["ReceiverAddress"] = res.data.ReceiverAddress;
            infoRow["ReceiverPhone"] = res.data.ReceiverPhone;



            infoRow["Description"] = models.Any() ? res.data.Description : string.Empty;
            infoRow["Reason"] = GetEnumDescription(res.data.Reason);
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/BM.03.VTQT.VT.02_BBBG.xls");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();
            if (stt > 1)
            {
                Worksheet worksheet = wb.Worksheets[0];

                // Create a Cells object ot fetch all the cells.
                Cells cells = worksheet.Cells;

                // Merge some Cells (C6:E7) into a single C6 Cell.
                cells.Merge(28, 8, stt - 1, 1);

            }

            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", "bien_ban_xac_nhan_hang_hoa-"+res.data.VoucherCode+".xlsx");

        }

        private static string FormatNumbe(decimal item)
        {
            return item.ToString("n4", CultureInfo.CurrentCulture) ?? 0.ToString("n4", CultureInfo.CurrentCulture);
        }





        public static string NumberToText(double inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng chẵn" : "");
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
    }
}
