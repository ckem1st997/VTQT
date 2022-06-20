using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Asset;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("asset-chart")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.Chart")]
    public class ChartController : AdminApiController
    {
        private readonly IChartService _chartService;

        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.Charts.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("get-chart-pie")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetChartPie(int type, string OrganizationId)
        {
            var result = _chartService.GetChartPie(type, OrganizationId);
            var models = new List<AssetModel>();
            foreach (var e in result)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        [Route("get-chart-column")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetChartColumn(int type, string OrganizationId)
        {
            var result = _chartService.GetChartCoulunm(type,OrganizationId);
            var models = new List<AssetModel>();
            foreach (var e in result)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }


        [Route("get-warranty-duration")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetWarrantyDurationAsync(AssetSearchModel model)
        {
            int number;
            var searchMode = new ChartSearchContext()
            {
                Keywords = model.Keywords,
                PageIndex = model.PageIndex - 1,
                PageSize = model.PageSize,
                Date= int.TryParse(model.OrganizationId, out number)?int.Parse(model.OrganizationId):0
            };
            var result =await _chartService.GetWarrantyDuration(searchMode);
            var models = new List<AssetModel>();
            foreach (var e in result)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = result.TotalCount
            });
        }


        [Route("get-project-base")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetProjectBaseAsync(AssetSearchModel model)
        {
            int number;
            var searchMode = new ChartSearchContext()
            {
                Keywords = model.Keywords,
                PageIndex = model.PageIndex - 1,
                PageSize = model.PageSize,
                OrganizationId  = model.OrganizationId

            };
            var result =await _chartService.GetProjectBase(searchMode);
            var models = new List<AssetModel>();
            foreach (var e in result)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = result.TotalCount
            });
        }
    }
}
