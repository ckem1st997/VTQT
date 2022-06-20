using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Api.Warehouse.Helper;
using VTQT.Caching;
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
    [Route("warehouse")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("WareHouse.Controllers.WareHouse")]
    public class WareHouseController : AdminApiController
    {
        #region Fields

        private readonly IWareHouseService _wareHouseService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IWareHouseModelHelper _wareHouseModelHelper;
        private readonly IAutoCodeService _autoCodeService;
        private readonly IXBaseCacheManager _cacheManager;

        #endregion Fields

        #region Ctor

        public WareHouseController(
            IWareHouseService wareHouseService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IWareHouseModelHelper wareHouseModelHelper,
            IAutoCodeService autoCodeService,
            IXBaseCacheManager cacheManager)
        {
            _wareHouseService = wareHouseService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _wareHouseModelHelper = wareHouseModelHelper;
            _autoCodeService = autoCodeService;
            _cacheManager = cacheManager;
        }

        #endregion Ctor

        #region Methods

        [Route("create-batch")]
        [HttpPost]
        public async Task<IActionResult> CreateBatch(IEnumerable<WareHouseModel> models)
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
            var countDone = await _wareHouseService.InsertWHAsync(entities);
            return Ok(new XBaseResult
            {
                success = true,
                data = countDone,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouse"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouses.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("details")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouses.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _wareHouseService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
                });

            var model = entity.ToModel();
            var list = await _wareHouseModelHelper.GetWareHouseDropdownTreeAsync();
            model.AvailableWareHouses = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
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

        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouses.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new WareHouseModel();
            var list = await _wareHouseModelHelper.GetWareHouseDropdownTreeAsync();
            model.AvailableWareHouses = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Insert a newly Warehouse item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouses.Create")]
        public async Task<IActionResult> Create(WareHouseModel model)
        {
            if (model?.Code == null || !model.Code.Any())
            {
                model.Code = await _autoCodeService.GenerateCode(nameof(WareHouse));
            }

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _wareHouseService.ExistsAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Warehouse.WareHouses.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _wareHouseService.InsertAsync(entity);
            _wareHouseService.UpdatePath(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.WareHouse"))
            });
        }

        /// <summary>
        /// Insert a newly Warehouse item excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create-excel")]
        [HttpPost]
        public async Task<IActionResult> CreateExcel(WareHouseModel model)
        {

            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _wareHouseService.InsertAsync(entity);
            _wareHouseService.UpdatePath(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.WareHouse"))
            });
        }

        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouses.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _wareHouseService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
                });

            var model = entity.ToModel();
            var list = await _wareHouseModelHelper.GetWareHouseDropdownTreeAsync();
            model.AvailableWareHouses = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();

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
        /// Update a Warehouse item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouses.Edit")]
        public async Task<IActionResult> Edit(WareHouseModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _wareHouseService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
                });

            entity = model.ToEntity(entity);

            await _wareHouseService.UpdateAsync(entity);
            _wareHouseService.UpdatePath(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.WareHouse"))
            });
        }

        /// <summary>
        /// Delete a list Warehouses
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouses.Deletes")]
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

            await _wareHouseService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.WareHouse"))
            });
        }

        /// <summary>
        /// Active a list Warehouses
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

            await _wareHouseService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.WareHouse"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.WareHouse"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu kho theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _wareHouseService.GetByCodeAsync(code);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
                });
            }

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Lấy giá trị lựa chọn kho cuối cùng
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        [Route("get-last-selected")]
        [HttpGet]
        public async Task<IActionResult> GetLastSelectedDepartment(
            string appId,
            string userId,
            string path,
            string warehouseId)
        {
            if (string.IsNullOrEmpty(appId) ||
                string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(path))
            {
                return Ok(new XBaseResult
                {
                    data = null
                });
            }

            var result = _wareHouseService.GetLastSelectedNodeTree(appId, userId, path, warehouseId);

            return Ok(new XBaseResult
            {
                success = true,
                data = result
            });
        }

        /// <summary>
        /// Cập nhật cache key chọn kho
        /// </summary>        
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        [Route("update-last-selected")]
        [HttpPost]
        public async Task<IActionResult> UpdateLastSelectedDepartment(
            string appId,
            string userId,
            string path,
            string warehouseId)
        {
            if (string.IsNullOrEmpty(appId) ||
                string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(path))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            await _cacheManager.HybridProvider.RemoveAsync(string.Format(ModelCacheKeys.WarehousesTreeModelCacheKey, appId, userId, path));

            var result = _wareHouseService.GetLastSelectedNodeTree(appId, userId, path, warehouseId);

            return Ok(new XBaseResult
            {
                success = true,
                data = result
            });
        }
        #endregion Methods

        #region Lists

        /// <summary>
        /// Get a list Warehouses
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] WareHouseSearchModel searchModel)
        {
            var searchContext = new WareHouseSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<WareHouseModel>();
            var entities = _wareHouseService.Get(searchContext);
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

        [Route("get-tree")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetTree(bool showHidden = false)
        {
            var entity = await _wareHouseModelHelper.GetWareHouseTree(2, !showHidden);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
                });

            return Ok(new XBaseResult
            {
                data = entity
            });
        }

        [Route("get-selectWareHouse")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelect()
        {
            var entity = _wareHouseService.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouse"))
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

        [Route("get-select")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Getall()
        {
            var entity = _wareHouseService.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            var models = new List<WareHouseModel>();
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

        [Route("get-select-tree")]
        [HttpGet]
        // [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelectTree(bool showList = false)
        {
            var entity =await _wareHouseModelHelper.GetWareHouseDropdownTreeAsync(false,showList);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });
            return Ok(new XBaseResult
            {
                success = true,
                data = entity
            });
        }

        /// <summary>
        /// Get a Warehouse item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _wareHouseService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Lấy danh sách kho cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _wareHouseService.GetAll(showHidden);

            List<WareHouseModel> result = new List<WareHouseModel>();

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

        #endregion Lists

        #region Utilities

        private async Task UpdateLocalesAsync(WareHouse entity, WareHouseModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        private async Task<bool> ValidateImportDataAsync(List<WareHouseModel> models,
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
            var existCodes = await _wareHouseService.ExistCodesAsync(lstCodes);

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