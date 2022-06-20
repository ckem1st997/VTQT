using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Localization;
using VTQT.Services.Warehouse;
using VTQT.Services.Warehouse.Helper;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("unit")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.Unit")]
    public class UnitController : AdminApiController
    {
        #region Fields
        private readonly IUnitService _unitService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        #endregion

        #region Ctor
        public UnitController(IUnitService unitService, ILocalizationService localizationService, ILocalizedEntityService localizedEntityService, ILanguageService languageService)
        {
            _unitService = unitService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
        }
        #endregion

        #region Method

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Units.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Trả về đơn vị mới rỗng có đa ngôn ngữ
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Units.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new UnitModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Tạo mới đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Inserted successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Units.Create")]
        public async Task<IActionResult> Create(UnitModel model)
        {
            model.UnitName = model?.UnitName?.Trim();
            if (!ModelState.IsValid)
            {
                return InvalidModelResult();
            }

            if (await _unitService.ExistsAsync(model.UnitName))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("WareHouse.Units.Fields.UnitName"))
                });
            }

            var entity = model.ToEntity();
            await _unitService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.Unit"))
            });
        }


        /// <summary>
        /// Trả về đơn vị cập nhật theo Id có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Units.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _unitService.GetByIdAsync(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Unit"))
                });
            }

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.UnitName = entity.GetLocalized(x => x.UnitName, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Cập nhật đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> Update(UnitModel model)
        {
            model.UnitName = model?.UnitName?.Trim();
            if (!ModelState.IsValid)
            {
                return InvalidModelResult();
            }

            var entity = await _unitService.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Unit"))
                });
            }

            entity = model.ToEntity(entity);

            await _unitService.UpdateAsync(entity);

            //Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.Unit"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu đơn vị theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _unitService.GetByIdAsync(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Unit"))
                });
            }

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy danh sách đơn vị phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] UnitSearchModel searchModel)
        {
            var searchContext = new UnitSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.ActiveStatus,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<UnitModel>();
            var entities = _unitService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.UnitName = await e.GetLocalizedAsync(x => x.UnitName, searchContext.LanguageId);

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }


        [Route("get-selectUnit")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAll()
        {
            var entity = _unitService.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Unit"))
                });

            var models = new List<SelectItem>();
            foreach (var e in entity)
            {
                var tem = new SelectItem();
                tem.id = e.Id;
                tem.text = e.UnitName;
                models.Add(tem);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }

        [Route("get-select")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelect()
        {
            var entity = _unitService.GetSelect();
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            var models = new List<UnitModel>();
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

        /// <summary>
        /// Xóa đơn vị theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Units.Deletes")]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.NoItemsSelected"))
                });
            }

            await _unitService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.Unit"))
            });
        }

        /// <summary>
        /// Kích hoạt/ ngừng kích hoạt đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Activated successfully</response>
        [Route("activates")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.NoItemsSelected"))
                });
            }

            await _unitService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                success = true,
                message = model.Active ?
                string.Format(T("Common.Notify.Activated"), T("Common.Unit")) :
                string.Format(T("Common.Notify.Deactivated"), T("Common.Unit"))
            });
        }

        /// <summary>
        /// Lấy danh sách đơn vị cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _unitService.GetAll(showHidden);

            List<UnitModel> result = new List<UnitModel>();

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


        [Route("get-unitname-by-code-item")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetUnitNameByWareHouseItemCode(string code)
        {
            var available = _unitService.GetUnitNameByWareHouseItemCode(code);

            return Ok(new XBaseResult
            {
                data = available
            });
        }

        [Route("get-unit-by-code-item")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetUnitByWareHouseItemCode(string code)
        {
            var available = _unitService.GetUnitByWareHouseItemCode(code);
            var check = available.ToModel();
            return Ok(new XBaseResult
            {
                data = check
            });
        }
        #endregion

        #region Utilities
        private async Task UpdateLocalesAsync(Core.Domain.Warehouse.Unit entity, UnitModel model)
        {
            foreach (var locale in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.UnitName, locale.UnitName, locale.LanguageId);
            }
        }
        #endregion
    }
}
