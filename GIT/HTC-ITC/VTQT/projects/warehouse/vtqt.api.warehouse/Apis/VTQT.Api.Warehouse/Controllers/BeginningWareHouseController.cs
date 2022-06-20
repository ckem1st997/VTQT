using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Localization;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{

    [Route("warehouse-beginning")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.BeginningWareHouse")]
    public class BeginningWareHouseController : AdminApiController
    {
        #region Fields
        private readonly IBeginningWareHouseService _beginningWareHouseService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public BeginningWareHouseController(
            IBeginningWareHouseService vendorService,
            ILocalizationService localizationService)
        {
            _beginningWareHouseService = vendorService;
            _localizationService = localizationService;
        }
        #endregion
        /// <summary>
        /// Insert a newly BeginningWareHouse item
        /// </summary>
        /// <param name="model">BeginningWareHouseModel - skip CustomProperties</param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.BeginningWareHouses.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        [AppApiAction("WareHouse.AppActions.BeginningWareHouses.Create")]
        public async Task<IActionResult> Create(IEnumerable<BeginningWareHouseModel> models)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entities = new List<BeginningWareHouse>();
            var modelsExist = new List<BeginningWareHouseModel>();
            foreach (var model in models)
            {
                if (await _beginningWareHouseService.ExistAsync(model.WareHouseId, model.ItemId) == false)
                {
                    var entity = model.ToEntity();
                    entity.Id = Guid.NewGuid().ToString();
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.ModifiedDate = DateTime.UtcNow;
                    entities.Add(entity);
                }
                else
                {
                    modelsExist.Add(model);
                }
            }

            if (entities?.Count > 0 && modelsExist.Count <= 0)
            {
                await _beginningWareHouseService.InsertRangeAsync(entities);                

                return Ok(new XBaseResult
                {
                    data = null,
                    success = true,
                    message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.BeginningWareHouse"))
                });
            }

            return Ok(new XBaseResult
            {
                success = false,
                data = modelsExist,
                message = string.Format(
                    T("Common.Notify.Duplicate"))
            });
        }

        #region Utilities

        #endregion
        /// <summary>
        /// Update a BeginningWareHouse item
        /// </summary>
        /// <param name="id">PRIMARY KEY BeginningWareHouse</param>
        /// <param name="model"> BeginningWareHouseModel - skip CustomProperties</param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.BeginningWareHouses.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<BeginningWareHouseModel> models)
        {
            ModelState.Remove("WareHouseModel.Code");
            ModelState.Remove("WareHouseItemModel.UnitId");
            ModelState.Remove("WareHouseItemModel.Code");
            string error = "";
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var entities = new List<BeginningWareHouse>();
            int i = 0;
            foreach (var model in models)
            {
                var entity = await _beginningWareHouseService.GetByIdAsync(model.Id);
                if (entity is null)
                {
                    error = error + string.Format($"Vị trí {i + 1}",
                            T("Common.Notify.DoesNotExist"),
                            T("Common.BeginningWareHouse"));
                }

                entity = model.ToEntity(entity);
                entity.ModifiedDate = DateTime.UtcNow;
                entities.Add(entity);
                i++;
            }

            await _beginningWareHouseService.UpdateRangeAsync(entities);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.BeginningWareHouse"))
            });

        }
        /// <summary>
        /// Delete a list BeginningWareHouse by list id
        /// </summary>
        /// <param name="ids">list id PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpDelete]
        [AppApiAction("WareHouse.AppActions.BeginningWareHouses.Deletes")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
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

            await _beginningWareHouseService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.BeginningWareHouse"))
            });
        }

        #endregion

        #region Lists
        /// <summary>
        /// Get a list BeginningWareHouse
        /// </summary>
        /// <param name="searchModel"> BeginningWareHouseSearchModel</param>
        /// <returns></returns>
        /// <response code="200">Got successfully - list BeginningWareHouseModel - PageIndex - PageSize - TotalCount </response>

        [Route("get-list")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] BeginningWareHouseSearchModel searchModel)
        {
            var searchContext = new BeginningWareHouseSearchContext
            {
                Keywords = searchModel.Keywords,
                DateSoft = searchModel.DateSoft,
                FromDate = searchModel.FromDate,
                ToDate = searchModel.ToDate,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                WareHouesId = searchModel.WareHouesId
            };

            var models = new List<BeginningWareHouseModel>();
            var entities =await _beginningWareHouseService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.WareHouseId = e.WareHouseId;
                m.UnitName = e.Unit is null ? "" : e.Unit.UnitName;
                m.UnitModel = e.Unit == null ? new UnitModel
                {
                    UnitName = ""
                } : new UnitModel
                {
                    Id = e.Unit.Id,
                    UnitName = e.Unit.UnitName
                };
                m.WareHouseModel = e.WareHouse == null ? new WareHouseModel
                {
                    Name = ""
                } : new WareHouseModel
                {
                    Id = e.WareHouse.Id,
                    Name = e.WareHouse.Name
                };
                m.WareHouseItemModel = e.WareHouseItem == null ? new WareHouseItemModel
                {
                    Name = ""
                } : new WareHouseItemModel
                {
                    Id = e.WareHouseItem.Id,
                    Name = e.WareHouseItem.Name
                };
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
        /// Check id idWareHouse and idItem by BeginningWareHouse
        /// </summary>
        /// <param name="idWareHouse"></param>
        /// <param name="idItem"></param>
        /// <returns>successfully</returns>
        [Route("check-ware-item")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Check(string idWareHouse,string idItem)
        {
           
            return Ok(new XBaseResult
            {
                success = await _beginningWareHouseService.ExistAsync(idWareHouse, idItem),
                message = string.Format(
                        T("Common.Notify.Exist"),
                        T("Common.BeginningWareHouse"))
            });
        }


        [Route("get-ware-item-by-warehouse-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByWareHouseId(string idWareHouse)
        {
            var list = await _beginningWareHouseService.GetByWareHouseIdAsync(idWareHouse);
            var model = new List<WareHouseItemModel>();
            foreach (var item in list)
            {
                model.Add(item.ToModel());
            }

            return Ok(new XBaseResult
            {
                data = model,
                message = string.Format(
                        T("Common.Notify.Exist"),
                        T("Common.BeginningWareHouse"))
            });
        }


        /// <summary>
        /// Get a BeginningWareHouse item by id
        /// </summary>
        /// <param name="id">PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Got successfully - BeginningWareHouseModel </response>
        [Route("get-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _beginningWareHouseService.GetByIdAsync(id);
            entity.CreatedDate = entity.CreatedDate.ToLocalTime();
            entity.ModifiedDate = entity.ModifiedDate.ToLocalTime();
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.BeginningWareHouse"))
                });

            var model = entity.ToModel();
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion
    }
}
