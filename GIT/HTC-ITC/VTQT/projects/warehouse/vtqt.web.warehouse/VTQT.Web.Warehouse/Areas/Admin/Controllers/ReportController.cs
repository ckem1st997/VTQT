using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;
using System;
using VTQT.Core;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.ComponentModel;
using VTQT.Core.Domain.Warehouse.Enum;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class ReportController : AdminMvcController
    {
        #region Fields
        private readonly Dictionary<string, string> reportRoute;
        #endregion

        #region Ctor
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ReportController()
        {
            reportRoute = new Dictionary<string, string>
            {
                { "Total", "/report/summary-warehouse" },
                { "Detail", "/report/ledger-warehouse" },
                { "InwardMisa", "/report/ledger-inward-misa" },
                { "OutwardMisa", "/report/ledger-out-misa" }
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Hàm Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var searchModel = new ReportValueSearchModel() { RouteKey = reportRoute["Detail"]};
            return View(searchModel);
        }
        #endregion

        #region List

        /// <summary>
        /// Gọi Api lấy cấu trúc cây báo cáo
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<ReportTreeModel>>("/report/get-report-list-by-appid", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            IList<ReportTreeModel> cg = new List<ReportTreeModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    if (item.key == reportRoute["Total"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["Detail"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["InwardMisa"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["OutwardMisa"])
                    {
                        cg.Add(item);
                    }
                }
            }

            return Ok(cg);
        }

        /// <summary>
        /// Lấy dữ liệu items
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetItems(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>(
                "warehouse-item/get-select", null, Method.GET, ApiHosts.Warehouse);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu kho
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetWarehouses(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<WareHouseModel>>(
                "warehouse/get-select-tree?showList=true", null, Method.GET, ApiHosts.Warehouse);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;            

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu người đề xuất
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetUsers(string search, int page)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);

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

        /// <summary>
        /// Lấy dữ liệu dự án
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProjects(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<ProjectModel>>(
                "project/get-available", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu phòng ban
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDepartments(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<OrganizationModel>>(
                "organization/get-available", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            ReportValueSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.WareHouseId) && searchModel.RouteKey == "/report/ledger-warehouse" || string.IsNullOrEmpty(searchModel.WareHouseId) && searchModel.RouteKey == "/report/summary-warehouse")
            {
                NotifyInfo(T("Notifies.NotWareHouseSelect"));
                return Ok(new XBaseResult { success = false });
            }
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<ReportValueModel>(),
                Total = 0
            };            
            
            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);       

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponse(searchModel);
            }            

            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo nhập kho Misa
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetInwardMisa(
            [DataSourceRequest] DataSourceRequest request,
            ReportInwaMisaSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<ReportInwardMisaModel>(),
                Total = 0
            };

            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponseInwardMisa(searchModel);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo xuất kho Misa
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetOutwardMisa(
            [DataSourceRequest] DataSourceRequest request,
            ReportOutwardMisaSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<ReportOutwardMisaModel>(),
                Total = 0
            };

            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponseOutwardMisa(searchModel);
            }
            return Ok(result);
        }

        /// <summary>
        /// Load partial view
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IActionResult> ListenTreeSelect(string key)
        {
            if (key == reportRoute["Total"])
            {                
                return PartialView("ReportTotal", new ReportValueSearchModel() { RouteKey = reportRoute["Total"]});
            }
            else if (key == reportRoute["Detail"])
            {
                ViewData["users"] = await GetAvailableUsers();
                return PartialView("ReportDetail", new ReportValueSearchModel() { RouteKey = reportRoute["Detail"]});
            }
            else if (key == reportRoute["InwardMisa"])
            {
                ViewData["users"] = await GetAvailableUsers();
                return PartialView("ReportInwardMisa", new ReportInwaMisaSearchModel() { RouteKey = reportRoute["InwardMisa"] });
            }
            else if (key == reportRoute["OutwardMisa"])
            {
                ViewData["users"] = await GetAvailableUsers();
                return PartialView("ReportOutwardMisa", new ReportOutwardMisaSearchModel() { RouteKey = reportRoute["OutwardMisa"] });
            }
            return View();
        }        
        #endregion

        #region Exports
        /// <summary>
        /// Xuất báo cáo excel
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetExcelReport(ReportValueSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
               .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                var res = await ApiHelper.ExecuteAsync<List<ReportValueModel>>(searchModel.RouteKey + "-excel", searchModel, Method.GET, ApiHosts.Master);
                var data = res?.data;                
                if (searchModel.RouteKey == reportRoute["Total"])
                {
                    if (data?.Count > 0)
                    {                        
                        var stt = 1;
                        var models = new List<ReportExcelTotalModel>();
                        if (data?.Count > 0)
                        {
                            foreach (var e in data)
                            {
                                var m = new ReportExcelTotalModel
                                {
                                    STT = stt,
                                    WareHouseItemCode = e.WareHouseItemCode,
                                    WareHouseItemName = e.WareHouseItemName,
                                    UnitName = e.UnitName,
                                    Beginning = e.Beginning,
                                    Import = e.Import,
                                    Export = e.Export,
                                    Balance = e.Balance
                                };
                                stt++;
                                models.Add(m);
                            }
                        }                        
                        var ds = new DataSet();
                        var dtInfo = new DataTable("Info");
                        dtInfo.Columns.Add("Title", typeof(string));
                        var infoRow = dtInfo.NewRow();
                        infoRow["Title"] = "Báo cáo tổng hợp";
                        dtInfo.Rows.Add(infoRow);
                        ds.Tables.Add(dtInfo);

                        var dtDataName = "Data";
                        var dtData = models.ToDataTable();
                        dtData.TableName = dtDataName;
                        ds.Tables.Add(dtData);
                        
                      //  var tmpPath = Path.GetFullPath("~/wwwroot/Templates/Reports/ReportTotal_vi.xlsx").Replace("~\\", "");
                        var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Reports/ReportTotal_vi.xlsx");

                        var wb = new Workbook(tmpPath);
                        var wd = new WorkbookDesigner(wb);
                        wd.SetDataSource(dataSet: ds);
                        wd.Process();
                        wd.Workbook.CalculateFormula();

                        var dstStream = new MemoryStream();
                        wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
                        dstStream.Seek(0, SeekOrigin.Begin);

                        dstStream.Position = 0;
                        return File(dstStream, "application/vnd.ms-excel", "bc_tonghop.xlsx");
                    }
                }
                else if (searchModel.RouteKey == reportRoute["Detail"])
                {
                    var stt = 1;
                    var models = new List<ReportExcelDetailModel>();
                    if (data?.Count > 0)
                    {
                        foreach (var e in data)
                        {
                            var m = new ReportExcelDetailModel
                            {
                                STT = stt,
                                WareHouseItemCode = e.WareHouseItemCode,
                                WareHouseItemName = e.WareHouseItemName,      
                                VoucherCodeImport = e.VoucherCodeImport,
                                VoucherCodeExport = e.VoucherCodeExport,
                                UnitName = e.UnitName,
                                Beginning = e.Beginning,
                                Import = e.Import,
                                Export = e.Export,
                                Balance = e.Balance,
                                Category = e.Category,
                                Purpose = e.Purpose,
                                Proposer = e.User,
                                DepartmentName = e.DepartmentName,
                                ProjectName = e.ProjectName,
                                Description = e.Description,
                                NoteRender = e.NoteRender,
                                Moment = e.Moment.ToString("dd/MM/yyyy")
                            };
                            stt++;
                            models.Add(m);
                        }
                    }                    
                    var ds = new DataSet();
                    var dtInfo = new DataTable("Info");
                    dtInfo.Columns.Add("Title", typeof(string));
                    var infoRow = dtInfo.NewRow();
                    infoRow["Title"] = "Báo cáo thẻ kho";
                    dtInfo.Rows.Add(infoRow);
                    ds.Tables.Add(dtInfo);

                    var dtDataName = "Data";
                    var dtData = models.ToDataTable();
                    dtData.TableName = dtDataName;
                    ds.Tables.Add(dtData);

                    // var tmpPath = Path.GetFullPath("~/wwwroot/Templates/Reports/ReportDetail_vi.xlsx").Replace("~\\", "");
                    var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Reports/ReportDetail_vi.xlsx");

                    var wb = new Workbook(tmpPath);
                    var wd = new WorkbookDesigner(wb);
                    wd.SetDataSource(dataSet: ds);
                    wd.Process();
                    wd.Workbook.CalculateFormula();

                    //var handler = Guid.NewGuid().ToString();

                    //var ms = new MemoryStream();
                    //wb.Save(ms, SaveFormat.Xlsx);
                    //ms.Seek(0, SeekOrigin.Begin);
                    //TempData[handler] = ms.ToArray();

                    //var fileDownloadName = "bc_thekho.xlsx";

                    //return Json(new { FileGuid = handler, FileName = fileDownloadName });
                    var dstStream = new MemoryStream();
                    wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
                    dstStream.Seek(0, SeekOrigin.Begin);

                    dstStream.Position = 0;
                    return File(dstStream, "application/vnd.ms-excel", "bc_thekho.xlsx");
                }
            }

            return null;
        }

        /// <summary>
        /// Xuất báo cáo excel nhập kho misa
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetExcelInwardMisaReport(ReportInwaMisaSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
               .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                var res = await ApiHelper.ExecuteAsync<List<ReportInwardMisaModel>>(searchModel.RouteKey + "-excel", searchModel, Method.GET, ApiHosts.Master);
                var data = res?.data;
                
                if (searchModel.RouteKey == reportRoute["InwardMisa"])
                {
                    var stt = 1;
                    var models = new List<ReportExcelInwardMisaDetailModel>();
                    if (data?.Count > 0)
                    {
                        foreach (var e in data)
                        {
                            var m = new ReportExcelInwardMisaDetailModel
                            {
                                STT = stt,
                                Moment=e.Moment,
                                NoteRender=e.NoteRender,
                                Voucher=string.IsNullOrEmpty(e.Voucher)?e.VoucherCode:e.Voucher,
                                VoucherDateTime = e.VoucherDateTime,
                                VoucherCode = e.VoucherCode,
                                UnitName = e.UnitName,
                                VendorCode = e.VendorCode,
                                ProjectId = e.ProjectId,
                                DepartmentName = e.DepartmentName,
                                Description = e.Description,
                                WareHouseItemCode = e.WareHouseItemCode,
                                WareHouseItemName = e.WareHouseItemName,
                                WareHouseItemId = e.WareHouseItemId,
                                AccountMore = e.AccountMore,
                                AccountYes = e.AccountYes,
                                Quantity = e.Quantity,
                                Price = e.Price,
                                Amount = e.Amount,
                            };
                            stt++;
                            models.Add(m);
                        }
                    }
                    var ds = new DataSet();
                    var dtInfo = new DataTable("Info");
                    dtInfo.Columns.Add("Title", typeof(string));
                    var infoRow = dtInfo.NewRow();
                    infoRow["Title"] = "Báo cáo nhập kho Misa";
                    dtInfo.Rows.Add(infoRow);
                    ds.Tables.Add(dtInfo);

                    var dtDataName = "Data";
                    var dtData = models.ToDataTable();
                    dtData.TableName = dtDataName;
                    ds.Tables.Add(dtData);

                    var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Reports/BC_MiSa_Nhap_kho.xls");

                    var wb = new Workbook(tmpPath);
                    var wd = new WorkbookDesigner(wb);
                    wd.SetDataSource(dataSet: ds);
                    wd.Process();
                    wd.Workbook.CalculateFormula();
                    
                    var dstStream = new MemoryStream();
                    wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
                    dstStream.Seek(0, SeekOrigin.Begin);

                    dstStream.Position = 0;
                    return File(dstStream, "application/vnd.ms-excel", "bc_nhapkhomisa.xlsx");
                }
            }

            return null;
        }

        /// <summary>
        /// Xuất báo cáo excel xuất kho misa
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetExcelOutwardMisaReport(ReportOutwardMisaSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?.ToUniversalTime()
               .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?.ToUniversalTime()
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                var res = await ApiHelper.ExecuteAsync<List<ReportOutwardMisaModel>>(searchModel.RouteKey + "-excel", searchModel, Method.GET, ApiHosts.Master);
                var data = res?.data;

                if (searchModel.RouteKey == reportRoute["OutwardMisa"])
                {
                    var stt = 1;
                    var models = new List<ReportExcelOutwardMisaDetailModel>();
                    if (data?.Count > 0)
                    {
                        foreach (var e in data)
                        {
                            e.Reason = GetEnumDescription((OutwardReason)e.Reason.ToInt());

                            var m = new ReportExcelOutwardMisaDetailModel
                            {
                                STT = stt,
                                ReceiverCode=e.ReceiverCode,
                                Receiver=e.Receiver,
                                Moment=e.Moment,
                                Reason=e.Reason,
                                VoucherDateTime = e.VoucherDateTime,
                                Voucher = string.IsNullOrEmpty(e.Voucher) ? e.VoucherCode : e.Voucher,
                                VoucherCode = e.VoucherCode,
                                UnitName = e.UnitName,
                                VendorCode = e.VendorCode,
                                ProjectId = e.ProjectId,
                                DepartmentName = e.DepartmentName,
                                Description = e.Description,
                                NoteRender=e.NoteRender,
                                WareHouseItemCode = e.WareHouseItemCode,
                                WareHouseItemName = e.WareHouseItemName,
                                WareHouseItemId = e.WareHouseItemId,
                                AccountMore = e.AccountMore,
                                AccountYes = e.AccountYes,
                                Quantity = e.Quantity,
                                Price = e.Price,
                                Amount = e.Amount,
                            };
                            stt++;
                            models.Add(m);
                        }
                    }
                    var ds = new DataSet();
                    var dtInfo = new DataTable("Info");
                    dtInfo.Columns.Add("Title", typeof(string));
                    var infoRow = dtInfo.NewRow();
                    infoRow["Title"] = "Báo cáo xuất kho Misa";
                    dtInfo.Rows.Add(infoRow);
                    ds.Tables.Add(dtInfo);

                    var dtDataName = "Data";
                    var dtData = models.ToDataTable();
                    dtData.TableName = dtDataName;
                    ds.Tables.Add(dtData);

                    var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Reports/BC_MiSa_Xuat_kho.xls");

                    var wb = new Workbook(tmpPath);
                    var wd = new WorkbookDesigner(wb);
                    wd.SetDataSource(dataSet: ds);
                    wd.Process();
                    wd.Workbook.CalculateFormula();

                    var dstStream = new MemoryStream();
                    wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
                    dstStream.Seek(0, SeekOrigin.Begin);

                    dstStream.Position = 0;
                    return File(dstStream, "application/vnd.ms-excel", "bc_xuatkhomisa.xlsx");
                }
            }

            return null;
        }

        /// <summary>
        /// Trả về file excel
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
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

        #region Utilities
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
        private async Task<DataSourceResult> GetDataResponse(ReportValueSearchModel searchModel)
        {
            var res = await ApiHelper
                        .ExecuteAsync<List<ReportValueModel>>
                         (searchModel.RouteKey, searchModel, Method.GET, ApiHosts.Master);
            var data = res.data;

            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }

        private async Task<DataSourceResult> GetDataResponseInwardMisa(ReportInwaMisaSearchModel searchModel)
        {
            var res = await ApiHelper
                        .ExecuteAsync<List<ReportInwardMisaModel>>
                         (searchModel.RouteKey, searchModel, Method.GET, ApiHosts.Master);
            var data = res.data;
            
            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }
        private async Task<DataSourceResult> GetDataResponseOutwardMisa(ReportOutwardMisaSearchModel searchModel)
        {
            var res = await ApiHelper
                        .ExecuteAsync<List<ReportOutwardMisaModel>>
                         (searchModel.RouteKey, searchModel, Method.GET, ApiHosts.Master);
            var data = res.data;
            foreach (var item in data)
            {
                item.Reason= GetEnumDescription((OutwardReason)item.Reason.ToInt());
            }
            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }
        private async Task<List<SelectListItem>> GetAvailableUsers()
        {
            var res = await ApiHelper
                .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            var users = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = $"{m.FullName} - {m.Email} ({m.UserName})"
                    };
                    users.Add(item);
                }
            }

            users.OrderBy(e => e.Text);
            if (users == null || users.Count == 0)
            {
                users = new List<SelectListItem>();
            }

            return users;
        }
        #endregion        
    }
}
