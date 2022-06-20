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
    [Route("warehouse-limit")]
    [ApiController]
  //  [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.WareHouseLimit")]
    public class WareHouseLimitController : AdminApiController
    {
        #region Fields
        private readonly IWareHouseLimitService _wareHouseLimitService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public WareHouseLimitController(
            IWareHouseLimitService wareHouseLimitService,
            ILocalizationService localizationService)
        {
            _wareHouseLimitService = wareHouseLimitService;
            _localizationService = localizationService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseLimits.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// thêm mới định mức tồn kho
        /// </summary>
        /// <param name="model">WareHouseLimitModel - skip CustomProperties</param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status201Created)]
        [AppApiAction("WareHouse.AppActions.WareHouseLimits.Create")]
        public async Task<IActionResult> Create(IEnumerable<WareHouseLimitModel> models)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entities = new List<WareHouseLimit>();
            var modelsExist = new List<WareHouseLimitModel>();
            foreach (var model in models)
            {
                if (await _wareHouseLimitService.ExistAsync(model.WareHouseId, model.ItemId) == false)
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
                await _wareHouseLimitService.InsertRangeAsync(entities);

                return Ok(new XBaseResult
                {
                    data = null,
                    success = true,
                    message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseLimit"))
                });
            }

            return Ok(new XBaseResult
            {
                success = false,
                data = modelsExist,
                message = string.Format(
                    T("Common.Notify.NotItem"),
                    T("Common.WareHouseLimit"))
            });
        }

        #region Utilities

        #endregion
        /// <summary>
        /// cập nhật theo danh sách định mức tồn kho
        /// </summary>
        /// <param name="models">WareHouseLimitModel - skip CustomProperties</param>
        /// <returns>Updated successfully</returns>
        /// 
        [Route("edit")]
        [HttpPost]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.WareHouseLimits.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<WareHouseLimitModel> models)
        {
            ModelState.Remove("WareHouseModel.Code");
            ModelState.Remove("WareHouseItemModel.UnitId");
            ModelState.Remove("WareHouseItemModel.Code");
            string error = "";
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var entities = new List<WareHouseLimit>();
            int i = 0;
            foreach (var model in models)
            {
                var entity = await _wareHouseLimitService.GetByIdAsync(model.Id);
                if (entity is null)
                {
                    error = error + string.Format($"Vị trí {i + 1}",
                            T("Common.Notify.DoesNotExist"),
                            T("Common.WareHouseLimit"));
                }

                entity = model.ToEntity(entity);
                entity.ModifiedDate = DateTime.UtcNow;
                entities.Add(entity);
                i++;
            }

            await _wareHouseLimitService.UpdateRangeAsync(entities);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.WareHouseLimit"))
            });

        }
        /// <summary>
        /// xóa định mức tồn kho theo danh sách
        /// </summary>
        /// <param name="ids">list id PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpDelete]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [AppApiAction("WareHouse.AppActions.WareHouseLimits.Deletes")]
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

            await _wareHouseLimitService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.WareHouseLimit"))
            });
        }

        #endregion

        #region Lists

        [Route("get-list-to-home")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
       // [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetToHomeAsync([FromQuery] WareHouseLimitSearchModel searchModel)
        {
            var searchContext = new WareHouseLimitSearchContext
            {
                Keywords = searchModel.Keywords,
                DateSoft = searchModel.DateSoft,
                FromDate = searchModel.FromDate,
                ToDate = searchModel.ToDate,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                WareHouesId = searchModel.WareHouesId
            };

            var models = new List<WareHouseLimitModel>();
            var entities = await _wareHouseLimitService.GetToHome(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Quantity = string.IsNullOrEmpty(e.UnitName) ? "0" : e.UnitName;
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
        /// lấy danh sách định mức tồn kho phân trang
        /// </summary>
        /// <param name="searchModel"> WareHouseLimitSearchModel</param>
        /// <returns></returns>
        /// <response code="200">Got successfully - list WareHouseLimitModel - PageIndex - PageSize - TotalCount </response>

        [Route("get-list")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] WareHouseLimitSearchModel searchModel)
        {
            var searchContext = new WareHouseLimitSearchContext
            {
                Keywords = searchModel.Keywords,
                DateSoft = searchModel.DateSoft,
                FromDate = searchModel.FromDate,
                ToDate = searchModel.ToDate,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                WareHouesId = searchModel.WareHouesId
            };

            var models = new List<WareHouseLimitModel>();
            var entities =await _wareHouseLimitService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.WareHouseId = e.WareHouseId;
                m.UnitName = e.Unit is null ? "" : e.Unit.UnitName;
                m.WareHouseModel = new WareHouseModel
                {
                    Id = e.WareHouse.Id,
                    Name = e.WareHouse.Name
                };
                m.WareHouseItemModel = new WareHouseItemModel
                {
                    Id = e.WareHouseItem.Id,
                    Name = e.WareHouseItem.Name
                };
                m.UnitModel = new UnitModel
                {
                    Id = e.Unit != null ? e.Unit.Id : "",
                    UnitName = e.Unit != null ? e.Unit.UnitName : "",
                };
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.Count
            });
        }
        /// <summary>
        /// Lấy dữ liệu định mức tồn kho theo Id
        /// </summary>
        /// <param name="id">PRIMARY KEY</param>
        /// <returns></returns>
        /// <response code="200">Got successfully - WareHouseLimitModel </response>
        [Route("get-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _wareHouseLimitService.GetByIdAsync(id);
            entity.CreatedDate = entity.CreatedDate.ToLocalTime();
            entity.ModifiedDate = entity.ModifiedDate.ToLocalTime();
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseLimit"))
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

        [Route("get-select")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelect()
        {
            var entity = _wareHouseLimitService.GetSelect();
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseLimit"))
                });

            var models = new List<WareHouseLimitModel>();
            foreach (var e in entity)
            {
                var m = e.ToModel();
                models.Add(m);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }
        #endregion
    }
}