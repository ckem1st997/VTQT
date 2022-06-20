using System;
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

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("warehouse-item")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.WareHouseItem")]
    public class WareHouseItemController : AdminApiController
    {
        #region Fields

        private readonly IWareHouseItemService _itemService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;
        private readonly IUnitService _unitService;
        private readonly IWareHouseItemCategoryService _service;
        private readonly IWareHouseItemUnitService _wareHouseItemUnitService;

        #endregion

        #region Ctor

        public WareHouseItemController(
            IWareHouseItemService itemService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IAutoCodeService autoCodeService,
            IUnitService unitService,
            IWareHouseItemCategoryService service,
            IWareHouseItemUnitService wareHouseItemUnitService)
        {
            _itemService = itemService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
            _autoCodeService = autoCodeService;
            _unitService = unitService;
            _service = service;
            _wareHouseItemUnitService = wareHouseItemUnitService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItems.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Tạo mới vật tư rỗng có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItems.Create")]
        public async Task<IActionResult> Create(string id)
        {
            var model = new WareHouseItemModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Thêm mới vật tư
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItems.Create")]
        public async Task<IActionResult> Create(WareHouseItemModel model)
        {
            if (model?.Code == null || !model.Code.Any())
            {
                model.Code = await _autoCodeService.GenerateCode(nameof(WareHouseItem));
            }

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _itemService.ExistsAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Warehouse.WareHouseItems.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _itemService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseItem"))
            });
        }

        [Route("create-batch")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> CreateBatch(IEnumerable<WareHouseItemModel> models)
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
                var listUnit = new List<WareHouseItemUnit>();
                var e = s.ToEntity();
                e.Code = s.Code;
                var unit = new WareHouseItemUnit();
                unit.ConvertRate = 1;
                unit.IsPrimary = true;
                unit.ItemId = s.Id;
                unit.UnitId = s.UnitId;
                listUnit.Add(unit);
                e.WareHouseItemUnits = listUnit;
                return e;
            });
            //     await _itemService.InsertAsync(entities);
            var countDone = await _itemService.InsertAsync(entities);
            if (countDone > 0)
            {
                var listUnit = new List<WareHouseItemUnit>();
                foreach (var model in entities)
                {
                    var unit = new WareHouseItemUnit();
                    unit.ConvertRate = 1;
                    unit.IsPrimary = true;
                    unit.ItemId = model.Id;
                    unit.UnitId = model.UnitId;
                    listUnit.Add(unit);
                }

                if (listUnit.Count > 0)
                    await _wareHouseItemUnitService.InsertAsync(listUnit);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = countDone,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseItem"))
            });
        }

        /// <summary>
        /// Thêm mới vật tư bằng excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create-excel")]
        [HttpPost]
        public async Task<IActionResult> CreateExcel(WareHouseItemModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _itemService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.WareHouseItem"))
            });
        }

        /// <summary>
        /// Trả về vật tư cập nhật có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.WareHouseItems.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _itemService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales,
                (locale, languageId) => { locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false); });

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Cập nhật vật tư
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItems.Edit")]
        public async Task<IActionResult> Edit(WareHouseItemModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _itemService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            entity = model.ToEntity(entity);

            await _itemService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.WareHouseItem"))
            });
        }

        /// <summary>
        /// Xóa vật tư theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.WareHouseItems.Deletes")]
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

            await _itemService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.WareHouseItem"))
            });
        }

        /// <summary>
        /// Kích hoạt/ ngừng kích hoạt vật tư
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

            await _itemService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                success = true,
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.WareHouseItem"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.WareHouseItem"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu vật tư theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _itemService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy dữ liệu vật tư theo Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-code")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _itemService.GetByCodeAsync(code);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        #endregion

        #region Lists

        /// <summary>
        /// Lấy danh sách vật tư phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] WareHouseItemSearchModel searchModel)
        {
            var searchContext = new WareHouseItemSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<WareHouseItemModel>();
            var entities = _itemService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);
                m.WareHouseItemCategoryModel = e.WareHouseItemCategory == null
                    ? new WareHouseItemCategoryModel
                    {
                        Code = "",
                        Name = ""
                    }
                    : new WareHouseItemCategoryModel
                    {
                        Id = e.WareHouseItemCategory.Id,
                        Code = e.WareHouseItemCategory.Code,
                        Name = await e.WareHouseItemCategory.GetLocalizedAsync(x => x.Name, searchContext.LanguageId)
                    };
                m.UnitModel = e.Unit == null
                    ? new UnitModel
                    {
                        UnitName = ""
                    }
                    : new UnitModel
                    {
                        Id = e.Unit.Id,
                        UnitName = await e.Unit.GetLocalizedAsync(x => x.UnitName, searchContext.LanguageId)
                    };
                m.VendorModel = e.Vendor == null
                    ? new VendorModel
                    {
                        Code = "",
                        Name = ""
                    }
                    : new VendorModel
                    {
                        Id = e.Vendor.Id,
                        Code = e.Vendor.Code,
                        Name = await e.Vendor.GetLocalizedAsync(x => x.Name, searchContext.LanguageId)
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

        [Route("get-selectWareHouseItem")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelect()
        {
            var entity = _itemService.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
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
            var entity = _itemService.GetAll(true);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseItem"))
                });

            var models = new List<WareHouseItemModel>();
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
        /// Lấy danh sách vật tư cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _itemService.GetAll(showHidden);

            List<WareHouseItemModel> result = new List<WareHouseItemModel>();

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


        /// <summary>
        /// Lấy danh sách vật tư cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("get-available-excel")]
        [HttpGet]
        public async Task<IActionResult> GetAvailableListExcel(bool showHidden = false)
        {
            var availableList = _itemService.GetAll(showHidden);

            List<WareHouseItemModel> result = new List<WareHouseItemModel>();

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

        #endregion

        #region Utilities

        private async Task UpdateLocalesAsync(WareHouseItem entity, WareHouseItemModel model)
        {
            foreach (var local in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, local.Name,
                    local.LanguageId);
            }
        }

        private async Task<bool> ValidateImportDataAsync(List<WareHouseItemModel> models,
            Dictionary<string, IEnumerable<string>> errors)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));


            // nếu true: tồn tại code trùng=> trả về false, mà không cần check nữa
            var isDup = false;
            var isError = false;
            var idx = 0;

            // Check row excel trùng lặp bởi Code, xem trong excel, người dùng có nhập code trùng nhau không
            var dupModels = models
                .GroupBy(g => g.Code)
                .Where(w => w.Skip(1).Any())
                .SelectMany(s => s);
            // check xem có tồn tại list danh sách trùng không
            isDup = dupModels.Any();
            // nếu tồn tại danh sách trùng
            foreach (var dupModel in dupModels)
            {
                // lấy vị trí của hàng row excel bị trùng(+1 là để hiển thị: hàng 0=> show lỗi là hàng 1)
                var dupIdx = models.IndexOf(dupModel) + 1;
                // khởi tạo biến chứa thông tin lỗi
                // var error = new KeyValuePair<string, IEnumerable<string>>(dupIdx.ToString(),
                //     Enumerable.Empty<string>());
                var listError = new List<string>();
                // thêm lỗi vào biến
                //   error.Value.Append($"Mã \"{dupModel.Code}\" bị trùng lặp dữ liệu");
                listError.Add($"Mã \"{dupModel.Code}\" bị trùng lặp dữ liệu");
                // thêm lỗi vào danh sách lỗi
                //  errors.Append(error);
                errors.Add(dupIdx.ToString(), listError);
            }

            // check nếu trùng thì out luôn
            if (isDup)
                return false;

            // check unitId có tồn tại trong db


            // get list unitId
            var lstUnitId = _unitService.GetSelect().ToList().Select(x => x.Id);
            // nếu tồn tại danh sách trùng
            foreach (var dupModel in models)
            {
                if (!lstUnitId.Contains(dupModel.UnitId))
                {
                    // lấy vị trí của hàng row excel bị trùng(+1 là để hiển thị: hàng 0=> show lỗi là hàng 1)
                    var dupIdx = models.IndexOf(dupModel) + 1;
                    var listError = new List<string>();
                    listError.Add($"Đơn vị tính chưa có trong cơ sở dữ liệu");
                    // thêm lỗi vào danh sách lỗi
                    //  errors.Append(error);
                    errors.Add(dupIdx.ToString(), listError);
                    isDup = true;
                }
            }

            // check nếu trùng thì out luôn
            if (isDup)
                return false;


            // get list WhCategoryId
            var lstWhCategoryId = _service.GetAll().ToList().Select(x => x.Id);
            // check xem có tồn tại list danh sách trùng không
            // nếu tồn tại danh sách trùng
            foreach (var dupModel in models)
            {
                if (!lstWhCategoryId.Contains(dupModel.CategoryID))
                {
                    // lấy vị trí của hàng row excel bị trùng(+1 là để hiển thị: hàng 0=> show lỗi là hàng 1)
                    var dupIdx = models.IndexOf(dupModel) + 1;
                    var listError = new List<string>();
                    listError.Add($"Loại vật tư chưa có trong cơ sở dữ liệu");
                    // thêm lỗi vào danh sách lỗi
                    //  errors.Append(error);
                    errors.Add(dupIdx.ToString(), listError);
                    isDup = true;
                }
            }

            // check nếu trùng thì out luôn
            if (isDup)
                return false;


            // Validate data row excel
            // lấy danh sách Code từ model excel, .Distinct() để lấy không trùng nhau
            var lstCodes = models.Where(w => !string.IsNullOrWhiteSpace(w.Code)).Select(s => s.Code).Distinct();
            // check xem danh sách code có tồn tại trong db không
            var existCodes = await _itemService.ExistCodesAsync(lstCodes);

            foreach (var m in models)
            {
                idx++;
                // như trên
                var error = new KeyValuePair<string, IEnumerable<string>>(idx.ToString(), Enumerable.Empty<string>());
                var listError = new List<string>();
                if (string.IsNullOrWhiteSpace(m.Code))
                {
                    isError = true;
                    //  error.Value.Append($"Mã không được để trống");
                    listError.Add($"Mã \"{m.Code}\" không được để trống");
                }

                if (existCodes.Contains(m.Code))
                {
                    isError = true;
                    //   error.Value.Append($"Mã \"{m.Code}\" đã tồn tại");
                    listError.Add($"Mã \"{m.Code}\" đã tồn tại");
                }

                //...
                //  errors.Append(error);
                errors.Add(idx.ToString(), listError);
            }

            return !isError;
        }

        #endregion


        [Route("update-all-warehouse-item-unit")]
        [HttpGet]
        public async Task<IActionResult> UpdateAllWareHouseItemUnit()
        {
            var listEntity = _itemService.GetAll(true);
            var listE = _wareHouseItemUnitService.GetAll();
            var listEntityWareHouseItemUnit = new List<WareHouseItemUnit>();
            foreach (var item in listEntity)
            {
                if (listE.FirstOrDefault(x => x.ItemId.Equals(item.Id) && x.UnitId.Equals(item.UnitId)) == null)
                {
                    var tem = new WareHouseItemUnit()
                    {
                        ConvertRate = 1,
                        IsPrimary = true,
                        UnitId = item.UnitId,
                        ItemId = item.Id
                    };
                    listEntityWareHouseItemUnit.Add(tem);
                }
            }

            long res = 0;
            if (listEntityWareHouseItemUnit.Count > 0)
                res = await _wareHouseItemUnitService.InsertAsync(listEntityWareHouseItemUnit);
            return Ok(new XBaseResult
            {
                success = res > 0
            });
        }
    }
}