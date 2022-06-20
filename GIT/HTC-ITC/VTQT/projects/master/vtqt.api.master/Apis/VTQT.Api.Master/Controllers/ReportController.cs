using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Master;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("report")]
    [ApiController]
    ///[XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.Report")]
    public class ReportController : AdminApiController
    {
        #region Fields
        private readonly IReportService _reportService;
        private readonly Dictionary<string, string> reportRoute;
        #endregion

        #region Ctor
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="reportService"></param>
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
            reportRoute = new Dictionary<string, string>
            {
                {"Total", "/report/summary-warehouse" },
                {"Detail", "/report/ledger-warehouse" },
                {"InwardMisa", "/report/inward-misa" },
                {"OutwardMisa", "/report/outward-misa" },
                //{"FTTH", "/report/ftth" },
                //{"Channel", "/report/channel" },
                //{"Ticket", "/report/ticket" },
                //{"CR", "/report/cr" },
                //{"ChannelDatetime", "/report/channel-datetime" },
                {AssetStatic.AssetInfrastructor, AssetStatic.AssetInfrastructorLink },
                { AssetStatic.AssetProject,AssetStatic.AssetProjectLink},
                { AssetStatic.AssetOffice,AssetStatic.AssetOfficeLink},
                //{ AssetStatic.FTTH,AssetStatic.FTTHLink},
                //{ AssetStatic.Channel,AssetStatic.ChannelLink},
                //{ AssetStatic.Ticket,AssetStatic.TicketLink},
                //{ AssetStatic.CR,AssetStatic.CRLink},
                //{ AssetStatic.ChannelDatetime,AssetStatic.ChannelDatetimeLink},
            };
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Reports.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        #endregion

        #region List    
        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("summary-warehouse")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> SummaryWarehouseAsync([FromQuery] ReportValueSearchModel searchModel)
        {
            var searchContext = new ReportValueSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models =await _reportService.GetReport(searchContext, reportRoute["Total"]);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp excel
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("summary-warehouse-excel")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> SummaryExcelWarehouseAsync([FromQuery] ReportValueSearchModel searchModel)
        {
            var searchContext = new ReportValueSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models =await _reportService.GetReportExcel(searchContext, reportRoute["Total"]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo thẻ kho
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("ledger-warehouse")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> LedgerWarehouseAsync([FromQuery] ReportValueSearchModel searchModel)
        {
            var searchContext = new ReportValueSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                ProjectId = searchModel.ProjectId,
                DepartmentId = searchModel.DepartmentId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                Proposer = searchModel.Proposer
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models =await _reportService.GetReport(searchContext, reportRoute["Detail"]);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo nhập kho misa
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("ledger-inward-misa")]
        [HttpGet]
        public async Task<IActionResult> LedgerInwardMisaAsync([FromQuery] ReportInwaMisaSearchModel searchModel)
        {
            var searchContext = new ReportInwardMisaSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                ProjectId = searchModel.ProjectId,
                DepartmentId = searchModel.DepartmentId.ToInt(),
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                Proposer = searchModel.Proposer
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportInwardMisa(searchContext, reportRoute["InwardMisa"]);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo xuất kho misa
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("ledger-out-misa")]
        [HttpGet]
        public async Task<IActionResult> LedgerOutMisaAsync([FromQuery] ReportOutwardMisaSearchModel searchModel)
        {
            var searchContext = new ReportOutwardMisaSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                ProjectId = searchModel.ProjectId,
                DepartmentId = searchModel.DepartmentId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                Proposer = searchModel.Proposer
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportOutwardMisa(searchContext, reportRoute["OutwardMisa"]);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo thẻ kho
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("infrastructor-asset")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> AssetInfrastructorAsync([FromQuery] ReportAssetValueSearchModel searchModel)
        {
            var searchContext = new ReportAssetInfrastructorSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                ItemCode = searchModel.ItemCode,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                KeyWords=searchModel.Keywords
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportAssetInfrastructorTreeAsync(searchContext, reportRoute[AssetStatic.AssetInfrastructor]);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }


         /// <summary>
        /// Lấy dữ liệu báo cáo thẻ kho
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("project-asset")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> AssetProjectAsync([FromQuery] ReportAssetValueSearchModel searchModel)
        {
            var searchContext = new ReportAssetInfrastructorSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                ItemCode = searchModel.ItemCode,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                KeyWords=searchModel.Keywords
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportAssetProjectTreeAsync(searchContext, reportRoute[AssetStatic.AssetProject]);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }


        /// <summary>
        /// Lấy dữ liệu báo cáo thẻ kho
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("office-asset")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> AssetOfficeAsync([FromQuery] ReportAssetValueSearchModel searchModel)
        {
            var searchContext = new ReportAssetInfrastructorSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                ItemCode = searchModel.ItemCode,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                KeyWords = searchModel.Keywords
                
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportAssetOfficeTreeAsync(searchContext, reportRoute[AssetStatic.AssetOffice]);
            // var list = new List<AssetModel>();
            // foreach (var item in models)
            // {
            //     var tem = item.ToModel();
            //     list.Add(tem);
            // }
            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }



        [Route("infrastructor-asset-export")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> AssetInfrastructorExportAsync([FromQuery] ReportAssetValueSearchModel searchModel)
        {
            var searchContext = new ReportAssetInfrastructorSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                ItemCode = searchModel.ItemCode,
                KeyWords = searchModel.Keywords

            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetExportAssetInfrastructorTreeAsync(searchContext, reportRoute[AssetStatic.AssetInfrastructor]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        
        
        [Route("office-asset-export")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> AssetOfficeExportAsync([FromQuery] ReportAssetValueSearchModel searchModel)
        {
            var searchContext = new ReportAssetInfrastructorSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                ItemCode = searchModel.ItemCode,
                KeyWords = searchModel.Keywords
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetExportAssetOfficeTreeAsync(searchContext, reportRoute[AssetStatic.AssetOffice]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }        
        
        
        [Route("project-asset-export")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> AssetProjectExportAsync([FromQuery] ReportAssetValueSearchModel searchModel)
        {
            var searchContext = new ReportAssetInfrastructorSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                ItemCode = searchModel.ItemCode,
                KeyWords = searchModel.Keywords
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetExportAssetProjectTreeAsync(searchContext, reportRoute[AssetStatic.AssetProject]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo thẻ kho excel
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("ledger-warehouse-excel")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> LedgerExcelWarehouseAsync([FromQuery] ReportValueSearchModel searchModel)
        {
            var searchContext = new ReportValueSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                ProjectId = searchModel.ProjectId,
                DepartmentId = searchModel.DepartmentId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                Proposer = searchModel.Proposer
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models =await _reportService.GetReportExcel(searchContext, reportRoute["Detail"]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo nhập kho misa excel
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("ledger-inward-misa-excel")]
        [HttpGet]
        public async Task<IActionResult> LedgerExcelInwardMisaAsync([FromQuery] ReportInwaMisaSearchModel searchModel)
        {
            var searchContext = new ReportInwardMisaSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                ProjectId = searchModel.ProjectId,
                DepartmentId = searchModel.DepartmentId.ToInt(),
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                Proposer = searchModel.Proposer
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportInwardMisaExcel(searchContext, reportRoute["InwardMisa"]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo xuất kho misa excel
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("ledger-out-misa-excel")]
        [HttpGet]
        public async Task<IActionResult> LedgerExcelOutwardMisaAsync([FromQuery] ReportOutwardMisaSearchModel searchModel)
        {
            var searchContext = new ReportOutwardMisaSearchContext
            {
                WareHouseId = searchModel.WareHouseId,
                WareHouseItemId = searchModel.WareHouseItemId,
                ProjectId = searchModel.ProjectId,
                DepartmentId = searchModel.DepartmentId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                Proposer = searchModel.Proposer
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = await _reportService.GetReportOutwardMisaExcel(searchContext, reportRoute["OutwardMisa"]);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo bảo dưỡng/ sửa chữa tài sản
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("asset-maintenance")]
        [HttpGet]
        public async Task<IActionResult> AssetMaintenanceReport([FromQuery] AssetMaintenanceReportSearchModel searchModel)
        {
            var searchContext = new AssetMaintenanceReportSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                AssetId = searchModel.AssetId,
                AssetType = searchModel.AssetType,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                StationCode = searchModel.StationCode
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = _reportService.GetReportAssetMaintenance(searchContext);            

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = models == null ? 0 : models.TotalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo excel bảo dưỡng/ sửa chữa tài sản
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("asset-maintenance-excel")]
        [HttpGet]
        public async Task<IActionResult> AssetMaintenanceReportExcel([FromQuery] AssetMaintenanceReportSearchModel searchModel)
        {
            var searchContext = new AssetMaintenanceReportSearchContext
            {
                OrganizationUnitId = searchModel.OrganizationUnitId,
                AssetId = searchModel.AssetId,
                AssetType = searchModel.AssetType,
                StationCode = searchModel.StationCode
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.AdjustToUniversal);
            }

            var models = _reportService.GetReportExcelAssetMaintenance(searchContext);

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        /// <summary>
        /// Lấy danh sách cây báo cáo
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="inactive"></param>
        /// <returns></returns>
        [Route("get-report-list-by-appid")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetReportListByAppid(string appid, bool inactive = false)
        {
            if (appid is null)
                appid = "";
            var results = _reportService.GetReportListByAppId(appid, inactive);

            return Ok(new XBaseResult
            {
                success = true,
                data = results,
                totalCount = results.Count()
            });
        }
        #endregion
    }
}
