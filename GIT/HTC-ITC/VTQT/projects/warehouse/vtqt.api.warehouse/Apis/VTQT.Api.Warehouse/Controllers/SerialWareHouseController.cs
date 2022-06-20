using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Localization;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("warehouse-serial")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.SerialWareHouse")]
    public class SerialWareHouseController : AdminApiController
    { 
        #region Fields
        private readonly ISerialWareHouseService _serialWareHouseService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public SerialWareHouseController(ISerialWareHouseService vendorService, ILocalizationService localizationService)
        {
            _serialWareHouseService = vendorService;
            _localizationService = localizationService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.SerialWareHouses.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Insert a newly SerialWareHouse item
        /// </summary>
        /// <param name="model">SerialWareHouseModel - skip CustomProperties</param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>

        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        [AppApiAction("WareHouse.AppActions.SerialWareHouses.Create")]
        public async Task<IActionResult> Create([FromBody] SerialWareHouseModel model)
        {
            model.Serial= model.Serial?.Trim();
            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _serialWareHouseService.ExistsAsync(model.Serial))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Warehouse.SerialWareHouse.Fields.Code"))
                });
            var entity = model.ToEntity();
            await _serialWareHouseService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.SerialWareHouse"))
            });
        }

        #region Utilities

        #endregion


        /// <summary>
        /// Update a SerialWareHouse item
        /// </summary>
        /// <param name="id">PRIMARY KEY SerialWareHouse</param>
        /// <param name="model">SerialWareHouseModel - skip CustomProperties</param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>

        [Route("edit")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.SerialWareHouses.Edit")]
        public async Task<IActionResult> Edit([FromBody] SerialWareHouseModel model)
        {
            ModelState.Remove("Serial");
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var entity = await _serialWareHouseService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.SerialWareHouse"))
                });

            entity = model.ToEntity(entity);
            await _serialWareHouseService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.SerialWareHouse"))
            });
        }

        /// <summary>
        /// Delete a list SerialWareHouse by list id
        /// </summary>
        /// <param name="ids">list id PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>

        [Route("deletes")]
        [HttpDelete]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.SerialWareHouses.Deletes")]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _serialWareHouseService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.SerialWareHouse"))
            });
        }


        #endregion

        #region Lists
        /// <summary>
        /// Get a list SerialWareHouse
        /// </summary>
        /// <param name="searchModel">SerialWareHouseSearchModel</param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>

        [Route("get-list")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public IActionResult Get([FromQuery] SerialWareHouseSearchModel searchModel)
        {
            var searchContext = new SerialWareHouseSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<SerialWareHouseModel>();
            var entities = _serialWareHouseService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<SerialWareHouseModel>(models, searchContext.PageIndex, searchContext.PageSize, entities.TotalCount)
            });
        }

        /// <summary>
        /// Get a SerialWareHouse item by id
        /// </summary>
        /// <param name="id">PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>

        [Route("get-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _serialWareHouseService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.SerialWareHouse"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion
    }
}