using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
    [Route("warehouse-balance")]
    [ApiController]
   // [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.WarehouseBalance")]
    public class WarehouseBalanceController : AdminApiController
    {
        #region Fields
        private readonly IWarehouseBalanceService _warehouseBalanceService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public WarehouseBalanceController(IWarehouseBalanceService vendorService, ILocalizationService localizationService)
        {
            _warehouseBalanceService = vendorService;
            _localizationService = localizationService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Insert a newly BeginningWareHouse item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>

        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        [AppApiAction("WareHouse.AppActions.WarehouseBalances.Create")]
        public async Task<IActionResult> Create([FromBody] WarehouseBalanceModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            await _warehouseBalanceService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WarehouseBalance"))
            });
        }

        #region Utilities

        #endregion

        /// <summary>
        /// Update a BeginningWareHouse item
        /// </summary>
        /// <param name="model">BeginningWareHouseModel</param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>

        [Route("edit")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.WarehouseBalances.Edit")]
        public async Task<IActionResult> Edit([FromBody] WarehouseBalanceModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _warehouseBalanceService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WarehouseBalance"))
                });

            entity = model.ToEntity(entity);
            await _warehouseBalanceService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.WarehouseBalance"))
            });
        }

        /// <summary>
        /// Delete a list BeginningWareHouse by list id
        /// </summary>
        /// <param name="ids">list PRIMARY KEY BeginningWareHouse</param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>

        [Route("deletes")]
        [HttpDelete]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.WarehouseBalances.Deletes")]
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

            await _warehouseBalanceService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.WarehouseBalance"))
            });
        }


        #endregion

        #region Lists


        [Route("get-list-to-home")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
      //  [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetToHomeAsync([FromQuery] WarehouseBalanceSearchModel searchModel)
        {
            var searchContext = new WarehouseBalanceSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<WarehouseBalanceModel>();
            var entities = await _warehouseBalanceService.GetTableToHome(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.UIQuantity = m.Amount;
                models.Add(m);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount

            });
        }


        /// <summary>
        /// Get a list BeginningWareHouse
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully - list BeginningWareHouseModel - PageIndex - PageSize - TotalCount</response>

        [Route("get-list")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public IActionResult Get([FromQuery] WarehouseBalanceSearchModel searchModel)
        {
            var searchContext = new WarehouseBalanceSearchContext
            {
                Keywords = searchModel.Keywords,
                fromAmount = searchModel.fromAmount,
                fromQuantity = searchModel.fromQuantity,
                toAmount = searchModel.toAmount,
                toQuantity = searchModel.toQuantity,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<WarehouseBalanceModel>();
            var entities = _warehouseBalanceService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<WarehouseBalanceModel>(models, searchContext.PageIndex, searchContext.PageSize, entities.TotalCount)
            });
        }


        /// <summary>
        /// Get a BeginningWareHouse item by id
        /// </summary>
        /// <param name="id">PRIMARY KEY BeginningWareHouse</param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>

        [Route("get-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _warehouseBalanceService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WarehouseBalance"))
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
