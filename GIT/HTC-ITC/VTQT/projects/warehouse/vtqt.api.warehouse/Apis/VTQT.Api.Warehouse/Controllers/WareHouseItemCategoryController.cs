using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Warehouse;
using VTQT.Services.Warehouse.Helper;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;
using System;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("warehouse-item-category")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.WareHouseItemCategory")]
    public class WareHouseItemCategoryController : AdminApiController
    {
        #region Fields

        private readonly IWareHouseItemCategoryService _service;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;
        #endregion Fields

        #region Ctor

        public WareHouseItemCategoryController(
            IWareHouseItemCategoryService service,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IAutoCodeService autoCodeService)
        {
            _service = service;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
            _autoCodeService = autoCodeService;
        }

        #endregion Ctor

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy chi tiết loại vật tư có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouseItemCategory"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Tạo mới loại vật tư rỗng có đa ngôn ngữ
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new WareHouseItemCategoryModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới loại vật tư
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Create")]
        public async Task<IActionResult> Create(WareHouseItemCategoryModel model)
        {
            if (model?.Code == null || !model.Code.Any())
            {
                model.Code = await _autoCodeService.GenerateCode(nameof(WareHouseItemCategory));
            }

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _service.ExistsAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Warehouse.WareHouseItemCategories.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _service.InsertAsync(entity);

            _service.UpdatePath(entity);
            //_service.UpdateAllPath();

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseItemCategory"))
            });
        }

        [Route("create-batch")]
        [HttpPost]
        public async Task<IActionResult> CreateBatch(IEnumerable<WareHouseItemCategoryModel> models)
        {
            var errors = new Dictionary<string, IEnumerable<string>>();
            if (!await ValidateImportDataAsync(models.ToList(), errors))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    errors = errors,
                    data = 0
                });
            }

            var entities = models.Select(s =>
            {
                var e = s.ToEntity();
                e.Code = s.Code;

                return e;
            });
            var countDone = await _service.InsertWHitemCategoryAsync(entities);
            return Ok(new XBaseResult
            {
                success = true,
                data = countDone,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseItemCategory"))
            });
        }

        /// <summary>
        /// Thêm mới loại vật tư excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create-excel")]
        [HttpPost]
        public async Task<IActionResult> CreateExcel(WareHouseItemCategoryModel model)
        {

            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _service.InsertAsync(entity);

            _service.UpdatePath(entity);
            //_service.UpdateAllPath();

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseItemCategory"))
            });
        }

        /// <summary>
        /// Trả về loại vật tư cập nhật có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouseItemCategory"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật loại vật tư
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Edit")]
        public async Task<IActionResult> Edit(WareHouseItemCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _service.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItemCategory"))
                });

            entity = model.ToEntity(entity);

            await _service.UpdateAsync(entity);

            _service.UpdatePath(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.WareHouseItemCategory"))
            });
        }

        /// <summary>
        /// Xóa loại vật tư theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.WareHouseItemCategories.Deletes")]
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

            await _service.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.WareHouseItemCategory"))
            });
        }

        /// <summary>
        /// Kích hoạt/ ngừng kích hoạt loại vật tư
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
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _service.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                success = true,
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.WareHouseItemCategory"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.WareHouseItemCategory"))
            });
        }
        
        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sách loại vật tư phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] WareHouseItemCategorySearchModel searchModel)
        {
            var searchContext = new WareHouseItemCategorySearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<WareHouseItemCategoryModel>();
            var entities = _service.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// Lấy dữ liệu loại vật tư theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItemCategory"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy danh sách loại vật tư cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _service.GetAll(showHidden);

            List<WareHouseItemCategoryModel> result = new List<WareHouseItemCategoryModel>();

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

        [Route("get-select")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Getall()
        {
            var entity = _service.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItemCategory"))
                });

            var models = new List<SelectItem>();
            foreach (var e in entity)
            {
                var tem = new SelectItem();
                tem.id = e.Id;
                tem.text = e.Name;
                models.Add(tem);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }



        [Route("get-select-excel")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetallExcel()
        {
            var entity = _service.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItemCategory"))
                });

            var models = new List<SelectItem>();
            foreach (var e in entity)
            {
                var tem = new SelectItem();
                tem.id = e.Id;
                tem.text = e.Name;
                models.Add(tem);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }

        #endregion List

        #region Utilities

        private async Task UpdateLocalesAsync(WareHouseItemCategory entity, WareHouseItemCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        private async Task<bool> ValidateImportDataAsync(List<WareHouseItemCategoryModel> models,
    Dictionary<string, IEnumerable<string>> errors)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));
            var isDup = false;
            var isError = false;
            var idx = 0;
            var dupModels = models
                .GroupBy(g => g.Code)
                .Where(w => w.Skip(1).Any())
                .SelectMany(s => s);
            isDup = dupModels.Any();
            foreach (var dupModel in dupModels)
            {
                var dupIdx = models.IndexOf(dupModel) + 1;
                var listError = new List<string>();
                listError.Add($"Mã \"{dupModel.Code}\" bị trùng lặp dữ liệu");
                errors.Add(dupIdx.ToString(), listError);
            }
            if (isDup)
                return false;

            var lstCodes = models.Where(w => !string.IsNullOrWhiteSpace(w.Code)).Select(s => s.Code).Distinct();
            var existCodes = await _service.ExistCodesAsync(lstCodes);

            foreach (var m in models)
            {
                idx++;
                var error = new KeyValuePair<string, IEnumerable<string>>(idx.ToString(), Enumerable.Empty<string>());
                var listError = new List<string>();
                if (string.IsNullOrWhiteSpace(m.Code))
                {
                    isError = true;
                    listError.Add($"Mã \"{m.Code}\" không được để trống");
                }

                if (existCodes.Contains(m.Code))
                {
                    isError = true;
                    listError.Add($"Mã \"{m.Code}\" đã tồn tại");
                }
                errors.Add(idx.ToString(), listError);
            }

            return !isError;
        }

        #endregion Utilities
    }
}
