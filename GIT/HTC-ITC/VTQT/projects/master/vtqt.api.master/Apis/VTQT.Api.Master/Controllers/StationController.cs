using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("station")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.Station")]
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
        [AppApiAction("Master.AppActions.Stations.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu trạm theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _stationService.GetByIdAsync(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Station"))
                });
            }

            var model = new StationModel
            {
                Id = entity.id,
                Code = entity.ma_tram,
                Name = entity.ten_tram
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy dữ liệu trạm theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _stationService.GetByCodeAsync(code);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Station"))
                });
            }

            var model = new StationModel
            {
                Id = entity.id,
                Code = entity.ma_tram,
                Name = entity.ten_tram
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Láy danh sách trạm phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] StationSearchModel searchModel)
        {
            var searchContext = new StationSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<StationModel>();
            var entities = _stationService.Get(searchContext);

            if (entities?.Count > 0)
            {
                foreach (var e in entities)
                {
                    var m = new StationModel
                    {
                        Id = e.id,
                        Code = e.ma_tram,
                        Name = e.ten_tram
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// Lấy danh sách trạm cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = _stationService.GetAvailable();
            var models = new List<StationModel>();

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = new StationModel
                    {
                        Id = e.id,
                        Code = e.ma_tram,
                        Name = e.ten_tram
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }
        #endregion
    }
}
