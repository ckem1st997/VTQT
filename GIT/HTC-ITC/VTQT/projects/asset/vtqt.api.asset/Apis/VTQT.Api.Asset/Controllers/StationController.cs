using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Asset;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("station")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.Station")]
    public class StationController : AdminApiController
    {
        #region Fields
        private readonly IStationService _stationService;
        #endregion

        #region Ctor
        public StationController(IStationService stationService)
        {
            _stationService = stationService;
        }
        #endregion

        #region Methods        
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.Stations.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Cập nhật bảng trạm từ QLSC
        /// </summary>
        /// <returns></returns>
        [Route("update-all")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult UpdateAll()
        {
            _stationService.UpdateAll();

            return Ok();
        }

        /// <summary>
        /// Lấy trạm theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByCode(string code)
        {
            if (!await _stationService.ExistsAsync(code))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Station"))
                });
            }

            var entity = await _stationService.GetByCodeAsync(code);

            return Ok(new XBaseResult
            {
                success = true,
                data = entity
            });
        }
        #endregion

        #region List

        #endregion
    }
}
