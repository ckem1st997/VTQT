using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("report")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.Report")]
    public class ReportController : AdminApiController
    {
        #region Fields
        private readonly IReportService _reportService;
        #endregion

        #region Ctor
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Reports.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// báo cáo tổng hợp
        /// </summary>
        /// <param name="reportSearchModel"></param>
        /// <returns></returns>
        [HttpGet("get-report-general")]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetReport([FromQuery] ReportSearchModel reportSearchModel)
        {
            var search = new ReportSearchContext();
            search.ReportType = reportSearchModel.ReportType;
            search.PageIndex = reportSearchModel.PageIndex - 1;
            search.PageSize = reportSearchModel.PageSize;
            search.WareHouseId = reportSearchModel.WareHouseId;
            search.WareHouseItemId = reportSearchModel.WareHouseItemId;
            search.FromDate = reportSearchModel.FromDate;
            search.ToDate = reportSearchModel.ToDate;
            var models = new List<ReportResponseModel>();
            var entities = _reportService.GetReport(search);
            foreach (var e in entities)
            {
                var tem = new ReportResponseModel();
                tem.BalanceQuantity = e.BalanceQuantity;
                tem.WarehouseItemCode = e.WarehouseItemCode;
                tem.InwardQuantity = e.InwardQuantity;
                tem.WarehouseItemName = e.WarehouseItemName;
                tem.OutwardQuantity = e.OutwardQuantity;
                tem.TotalQuantity = e.TotalQuantity;
                tem.UnitName = e.UnitName;
                tem.Purpose = e.Purpose;
                tem.DepartmentName = e.DepartmentName;
                tem.Proposer = e.Proposer;
                tem.Description = e.Description;
                tem.Generic = e.Generic;
                tem.Note = "";
                tem.Date = e.Date.ToLocalTime();
                tem.ProjectName = e.ProjectName;
                tem.BeginningQuantity = e.BeginningQuantity;
                models.Add(tem);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<ReportResponseModel>(models, search.PageIndex, search.PageSize, entities.TotalCount)
            });
        }

        #endregion

        #region Utilities

        #endregion
    }
}
