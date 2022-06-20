using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("station-level")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.StationLevel")]
    public class StationLevelController : AdminApiController
    {
        #region Fields

        private readonly IStationLevelService _stationLevelService;

        #endregion Fields

        #region Ctor

        public StationLevelController(
            IStationLevelService stationLevelService)
        {
            _stationLevelService = stationLevelService;
        }

        #endregion Ctor

        #region Utilities

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.StationLevel.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy danh sách dự án cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _stationLevelService.GetAll(showHidden);

            List<StationLevelModel> result = new List<StationLevelModel>();

            if (availableList?.Count > 0)
            {
                availableList.ToList().ForEach(x =>
                {
                    var model = x.ToModel();
                    result.Add(model);
                });
            }

            return Ok(new XBaseResult
            {
                data = result
            });
        }

        #endregion Utilities
    }
}