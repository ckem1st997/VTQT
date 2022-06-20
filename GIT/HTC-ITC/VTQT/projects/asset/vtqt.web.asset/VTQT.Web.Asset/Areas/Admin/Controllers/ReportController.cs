using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Asset.Areas.Admin.Helper;
using VTQT.Web.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Asset.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class ReportController : AdminMvcController
    {
        #region Fields

        private const string AssetInfrastructor = "AssetInfrastructor";
        private const string AssetProject = "AssetProject";
        private const string AssetOffice = "AssetOffice";
        private const string AssetMaintenance = "AssetMaintenance";
        private const string AssetInfrastructorLink = "/report/infrastructor-asset";
        private const string AssetProjectLink = "/report/project-asset";
        private const string AssetOfficeLink = "/report/office-asset";
        private const string AssetMaintenanceLink = "/report/asset-maintenance";
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
                { AssetInfrastructor, AssetInfrastructorLink },
                { AssetProject, AssetProjectLink },
                { AssetOffice, AssetOfficeLink },
                { AssetMaintenance, AssetMaintenanceLink }
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
            var view = new ReportAssetValueSearchModel() { RouteKey = reportRoute[AssetOffice] };
            return View(view);
        }

        #endregion

        #region ListTree

        /// <summary>
        /// Gọi Api lấy cấu trúc cây báo cáo
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<ReportTreeModel>>(
                "/report/get-report-list-by-appid?appid=6&inactive=false", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            IList<ReportTreeModel> cg = new List<ReportTreeModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    if (item.key == reportRoute[AssetProject])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute[AssetInfrastructor])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute[AssetOffice])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute[AssetMaintenance])
                    {
                        cg.Add(item);
                    }
                }
            }

            return Ok(cg);
        }

        /// <summary>
        /// Load partial view
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IActionResult> ListenTreeSelect(string key)
        {
            if (key == reportRoute[AssetInfrastructor])
            {
                return PartialView(AssetInfrastructor,
                    new ReportAssetValueSearchModel() { RouteKey = reportRoute[AssetInfrastructor] });
            }
            else if (key == reportRoute[AssetProject])
            {
                return PartialView(AssetProject,
                    new ReportAssetValueSearchModel() { RouteKey = reportRoute[AssetProject] });
            }
            else if (key == reportRoute[AssetOffice])
            {
                return PartialView(AssetOffice,
                    new ReportAssetValueSearchModel() { RouteKey = reportRoute[AssetOffice] });
            }
            else if (key == reportRoute[AssetMaintenance])
            {
                var res = await ApiHelper
                    .ExecuteAsync<AssetModel>("/asset/create", null, Method.GET, ApiHosts.Asset);
                ViewData["assetStatus"] = res.data?.AvailableAssetStatus;
                return PartialView(AssetMaintenance,
                    new AssetMaintenanceReportSearchModel()
                    {
                        RouteKey = reportRoute[AssetMaintenance],
                        StrAssetType = ((int)AssetType.Infrastructure).ToString()
                    });
            }

            return View();
        }

        #endregion

        #region ListReport

        /// <summary>
        /// Lấy dữ liệu báo cáo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetAssetInfrastructor(
            [DataSourceRequest] DataSourceRequest request,
            ReportAssetValueSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<ReportAssetInfrastructorModel>(),
                Total = 0
            };
            if (string.IsNullOrEmpty(searchModel.OrganizationUnitId))
                return Ok(result);
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponseAssetInfrastructor(searchModel);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetAssetProject(
            [DataSourceRequest] DataSourceRequest request,
            ReportAssetValueSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<AssetModel>(),
                Total = 0
            };
            if (string.IsNullOrEmpty(searchModel.OrganizationUnitId))
                return Ok(result);
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponseAssetProject(searchModel);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetAssetOffice(
            [DataSourceRequest] DataSourceRequest request,
            ReportAssetValueSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<AssetModel>(),
                Total = 0
            };
            if (string.IsNullOrEmpty(searchModel.OrganizationUnitId))
                return Ok(result);
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponseAssetOffice(searchModel);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetReportAssetMaintenance(
            [DataSourceRequest] DataSourceRequest request,
            AssetMaintenanceReportSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var result = new DataSourceResult
            {
                Data = new List<AssetMaintenanceReportModel>(),
                Total = 0
            };

            if (!string.IsNullOrEmpty(searchModel.StrAssetType))
            {
                int.TryParse(searchModel.StrAssetType, out int assetType);
                searchModel.AssetType = assetType;
            }
            else
            {
                searchModel.AssetType = 20;
            }

            if (string.IsNullOrEmpty(searchModel.OrganizationUnitId))
                return Ok(result);
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                result = await GetDataResponseReportAssetMaintenance(searchModel);
            }

            return Ok(result);
        }

        #endregion

        #region Utilities

        private async Task<DataSourceResult> GetDataResponseReportAssetMaintenance(
            AssetMaintenanceReportSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<AssetMaintenanceReportModel>>(searchModel.RouteKey, searchModel,
                Method.GET, ApiHosts.Master);
            var data = res.data;

            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }

        private async Task<DataSourceResult> GetDataResponseAssetInfrastructor(ReportAssetValueSearchModel searchModel)
        {
            var res = ApiHelper.Execute<List<ReportAssetInfrastructorModel>>(searchModel.RouteKey, searchModel,
                Method.GET, ApiHosts.Master);
            var data = res.data;
            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }

        private async Task<DataSourceResult> GetDataResponseAssetProject(ReportAssetValueSearchModel searchModel)
        {
            var res = ApiHelper.Execute<List<AssetModel>>(searchModel.RouteKey, searchModel, Method.GET,
                ApiHosts.Master);
            var data = res.data;
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.BalanceQuantity = item.OriginQuantity - item.RecallQuantity - item.SoldQuantity -
                                           item.BrokenQuantity;
                }
            }

            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }

        private async Task<DataSourceResult> GetDataResponseAssetOffice(ReportAssetValueSearchModel searchModel)
        {
            var res = ApiHelper.Execute<List<AssetModel>>(searchModel.RouteKey, searchModel, Method.GET,
                ApiHosts.Master);
            var data = res.data;
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.BalanceQuantity = item.OriginQuantity - item.RecallQuantity - item.SoldQuantity -
                                           item.BrokenQuantity;
                }
            }

            return new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
        }

        public async Task<IActionResult> GetOrganizationUnitId(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<OrganizationModel>>(
                "/organization/get-available", null, Method.GET, ApiHosts.Master);
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

        public async Task<IActionResult> GetAssetItem(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<AssetModel>>(
                "/asset/get-available-short", null, Method.GET, ApiHosts.Asset);
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

        [HttpGet]
        public async Task<IActionResult> GetEmployee(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET,
                ApiHosts.Master);
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
        /// Lấy dữ liệu trạm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetStations(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<StationModel>>(
                "station/get-available", null, Method.GET, ApiHosts.Master);

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

        public async Task<IActionResult> GetStationAsync(string search, int page)
        {
            var res = await ApiHelper.ExecuteAsync<List<StationModel>>("/station/get-available", null, Method.GET,
                ApiHosts.Master);
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

        public async Task<IActionResult> GetProjectAsync(string search, int page)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<ProjectModel>>("/project/get-available", null, Method.GET, ApiHosts.Master);
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


        public async Task<IActionResult> GetCategory(string search, int page)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<AssetCategoryModel>>($"/asset-category/get-available?assetType=0", null, Method.GET,
                    ApiHosts.Asset);
            var categories = new List<SelectListItem>();
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
        /// Lấy dữ liệu kiểu tài sản
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAssetTypes(string search, int page)
        {
            var data = new List<SelectListItem>
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
                },
            };

            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Text.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Khởi tạo dữ liệu kiểu tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetAssetType(string id)
        {
            var assetTypes = Enum.GetValues(typeof(AssetType));
            SelectListItem assetType = new SelectListItem();

            foreach (AssetType ast in assetTypes)
            {
                if (id == ((int)ast).ToString())
                {
                    assetType.Value = id;
                    assetType.Text = ast.GetEnumDescription();
                }
            }

            return Ok(assetType);
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
        /// Lấy dữ liệu phòng ban
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <param name="strAssetType"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAssets(string search, int page, string strAssetType)
        {
            int.TryParse(strAssetType, out int assetType);
            var res = await ApiHelper.ExecuteAsync<List<SelectListItem>>(
                "asset/get-available", new { assetType = assetType }, Method.GET, ApiHosts.Asset);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Text.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        #endregion

        #region export excel

        public async Task<ActionResult> GetExcelOfficeReport(ReportAssetValueSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.PageIndex = 1;
            searchModel.PageSize = 1000;
            var res = await ApiHelper.ExecuteAsync<List<AssetModel>>("/report/office-asset-export", searchModel,
                Method.GET, ApiHosts.Master);
            var data = res?.data;
            var stt = 1;
            var models = new List<ReportAssetExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new ReportAssetExportModel
                    {
                        STT = stt,
                        AllocationDate = order.AllocationDate.ToString().Split(" ")[0],
                        Code = order.Code,
                        Name = order.Name,
                        CategoryId = order.CategoryId,
                        OriginQuantity = order.OriginQuantity,
                        RecallQuantity = order.RecallQuantity,
                        SoldQuantity = order.SoldQuantity,
                        BrokenQuantity = order.BrokenQuantity,
                        BalanceQuantity = order.OriginQuantity - order.RecallQuantity - order.BrokenQuantity -
                                          order.SoldQuantity,
                        UnitName = order.UnitName,
                        StationName = order.StationName,
                        MaintenancedDate = order.MaintenancedDate,
                        CurrentUsageStatus = order.CurrentUsageStatus
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("AssetOffice");
            dtInfo.Columns.Add("Title", typeof(string));
            //  dtInfo.Columns.Add("AllocationDate", typeof(DateTime));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo tài sản hành chính";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/ReportOfficeAsset_vi.xlsx");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();

            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);
            var fileDownloadName = "bao_cao_tai_san_hanh_chinh.xlsx";

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", fileDownloadName);
        }  
        
        public async Task<ActionResult> GetExcelProjectReport(ReportAssetValueSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.PageIndex = 1;
            searchModel.PageSize = 1000;
            var res = await ApiHelper.ExecuteAsync<List<AssetModel>>("/report/project-asset-export", searchModel,
                Method.GET, ApiHosts.Master);
            var data = res?.data;
            var stt = 1;
            var models = new List<ReportAssetExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new ReportAssetExportModel
                    {
                        STT = stt,
                        AllocationDate = order.AllocationDate.ToString().Split(" ")[0],
                        Code = order.Code,
                        Name = order.Name,
                        CategoryId = order.CategoryId,
                        OriginQuantity = order.OriginQuantity,
                        RecallQuantity = order.RecallQuantity,
                        SoldQuantity = order.SoldQuantity,
                        BrokenQuantity = order.BrokenQuantity,
                        BalanceQuantity = order.OriginQuantity - order.RecallQuantity - order.BrokenQuantity -
                                          order.SoldQuantity,
                        UnitName = order.UnitName,
                        StationName = order.StationName,
                        MaintenancedDate = order.MaintenancedDate,
                        CurrentUsageStatus = order.CurrentUsageStatus,
                        ProjectCode = order.ProjectCode,
                        ProjectName = order.ProjectName,
                        CustomerName = order.CustomerName,
                        CustomerCode = order.CustomerCode
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("AssetProject");
            dtInfo.Columns.Add("Title", typeof(string));
            //  dtInfo.Columns.Add("AllocationDate", typeof(DateTime));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo tài sản dự án";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/ReportProjectAsset_vi.xlsx");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();

            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);
            var fileDownloadName = "bao_cao_tai_san_du_an.xlsx";

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", fileDownloadName);
        }


        public async Task<ActionResult> GetExcelInfrastructorReport(ReportAssetValueSearchModel searchModel)
        {
           searchModel.StrFromDate = searchModel.FromDate?
              .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.PageIndex = 1;
            searchModel.PageSize = 1000;
            var res = await ApiHelper.ExecuteAsync<List<ReportAssetInfrastructorModel>>("/report/infrastructor-asset-export", searchModel, Method.GET, ApiHosts.Master);
            var data = res?.data;
            var stt = 1;
            var models = new List<ReportAssetExportModel>();
            if (data?.Count > 0)
            {
                foreach (var order in data)
                {
                    var m = new ReportAssetExportModel
                    {
                        STT = stt,
                        Area = order.Area,
                        LongItude = order.LongItude,
                        LatItude = order.LatItude,
                        Name = order.Name,
                        CurrentUsageStatus = order.CurrentUsageStatus,
                        Note = order.Note,
                        Quantity = order.Quantity,
                        StationCode = order.StationCode,
                        StationName = order.StationName,
                        UnitName = order.UnitName
                    };
                    stt++;
                    models.Add(m);
                }
            }
            var ds = new DataSet();
            var dtInfo = new DataTable("Asset");
            dtInfo.Columns.Add("Title", typeof(string));
            var infoRow = dtInfo.NewRow();
            infoRow["Title"] = "Báo cáo tài sản hạ tầng";
            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/ReportInfrastructorAsset_vi.xlsx");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();

            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);
            var fileDownloadName = "bao_cao_tai_san_ha_tang.xlsx";

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", fileDownloadName);
        }

        /// <summary>
        /// Xuất báo cáo excel bảo dưỡng tài sản
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetReportExcelAssetMaintenance(AssetMaintenanceReportSearchModel searchModel)
        {
            searchModel.StrFromDate = searchModel.FromDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrToDate = searchModel.ToDate?
                .ToString("s", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(searchModel.StrAssetType))
            {
                int.TryParse(searchModel.StrAssetType, out int assetType);
                searchModel.AssetType = assetType;
            }
            else
            {
                searchModel.AssetType = 20;
            }

            if (!string.IsNullOrEmpty(searchModel.RouteKey))
            {
                var res = await ApiHelper.ExecuteAsync<List<AssetMaintenanceReportModel>>(
                    searchModel.RouteKey + "-excel", searchModel, Method.GET, ApiHosts.Master);
                var data = res?.data;

                if (data?.Count > 0)
                {
                    var assetMaintenance = new DataTable();
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.Stt), typeof(int));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.OrganizationUnitName),
                        typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.MaintenanceLocation),
                        typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.AssetName), typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.Serial), typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.CurrentUsageStatus),
                        typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.MaintenanceDate),
                        typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.ReasonDescription),
                        typeof(string));
                    assetMaintenance.Columns.Add(nameof(AssetMaintenanceReportExcelModel.Content), typeof(string));

                    var info = new DataTable();
                    info.Columns.Add("title", typeof(string));

                    info.Rows.Add("Báo cáo bảo dưỡng/ sửa chữa");
                    info.TableName = "info";
                    var stt = 1;
                    data.ForEach(x =>
                    {
                        var m = new AssetMaintenanceReportExcelModel
                        {
                            Stt = stt++,
                            OrganizationUnitName = x.OrganizationUnitName,
                            MaintenanceLocation = x.MaintenanceLocation,
                            AssetName = x.AssetName,
                            Serial = x.Serial,
                            ReasonDescription = x.ReasonDescription,
                            Content = x.Content,
                            MaintenanceDate = x.MaintenanceDate.ToString("dd/MM/yyyy")
                        };

                        if (!string.IsNullOrEmpty(x.CurrentUsageStatus))
                        {
                            var getUsageStatus = ApiHelper.ExecuteAsync<UsageStatusModel>("usage-status/get-by-id",
                                new { id = x.CurrentUsageStatus }, Method.GET, ApiHosts.Asset);
                            var usage = getUsageStatus.Result.data;

                            m.CurrentUsageStatus = usage?.Description;
                        }

                        assetMaintenance.Rows.Add(m.Stt, m.OrganizationUnitName, m.MaintenanceLocation,
                            m.AssetName, m.Serial, m.CurrentUsageStatus, m.MaintenanceDate,
                            m.ReasonDescription, m.Content);
                    });
                    assetMaintenance.TableName = "assetMaintenance";
                    var ds = new DataSet();
                    ds.Tables.Add(assetMaintenance);
                    ds.Tables.Add(info);
                    var tmpPath = CommonHelper.MapPath("/wwwroot/Teamplate/Excel/AssetMaintenanceReport.xlsx");
                    var bytes = TemplateExcel.FillReport("AssetMaintenanceReport.xlsx", tmpPath, ds,
                        new string[] { "{", "}" });

                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "bc_baoduong.xlsx");
                }
            }

            return Ok();
        }

        #endregion
    }
}