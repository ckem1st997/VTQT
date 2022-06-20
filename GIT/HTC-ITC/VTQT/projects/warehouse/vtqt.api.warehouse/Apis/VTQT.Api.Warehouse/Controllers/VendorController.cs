using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("vendor")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.Vendor")]
    public class VendorController : AdminApiController
    {
        #region Fields
        private readonly IVendorService _venderService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IAutoCodeService _autoCodeService;
        #endregion

        #region Ctor
        public VendorController(
            ILanguageService languageService,
            IVendorService vendorService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService)
        {
            _venderService = vendorService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
            _autoCodeService = autoCodeService;
        }
        #endregion

        #region Methods

        [Route("create-batch")]
        [HttpPost]
        public async Task<IActionResult> CreateBatch(IEnumerable<VendorModel> models)
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
            var countDone = await _venderService.InsertVendorAsync(entities);
            return Ok(new XBaseResult
            {
                success = true,
                data = countDone,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.Vendor"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Vendors.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Tạo mới nhà cung cấp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Vendors.Create")]
        public async Task<IActionResult> Create(VendorModel model)
        {
            if (model?.Code == null || !model.Code.Any())
            {
                model.Code = await _autoCodeService.GenerateCode(nameof(Vendor));
            }

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _venderService.ExistsAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Warehouse.Vendors.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _venderService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.Vendor"))
            });
        }

        /// <summary>
        /// Tạo mới nhà cung cấp excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create-excel")]
        [HttpPost]
        public async Task<IActionResult> CreateExcel(VendorModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _venderService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.Vendor"))
            });
        }

        /// <summary>
        /// Trả về nhà cung cấp mới rỗng có đa ngôn ngữ
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Vendors.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new VendorModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Trả về nhà cung cấp cập nhật có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Vendors.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _venderService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
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
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Vendors.Edit")]
        public async Task<IActionResult> Edit(VendorModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _venderService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Vendor"))
                });

            entity = model.ToEntity(entity);

            await _venderService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.Vendor"))
            });
        }

        /// <summary>
        /// Xóa nhà cung cấp theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("WareHouse.AppActions.Vendors.Deletes")]
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

            await _venderService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.Vendor"))
            });
        }

        /// <summary>
        /// Kích hoạt/ ngừng kích hoạt nhà cung cấp
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

            await _venderService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                success = true,
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.Vendor"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.Vendor"))
            });
        }

        #endregion

        #region Lists
        /// <summary>
        /// Lấy danh sách nhà cung cấp phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] VendorSearchModel searchModel)
        {
            var searchContext = new VendorSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.ActiveStatus,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<VendorModel>();
            var entities = _venderService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<VendorModel>(models, searchContext.PageIndex, searchContext.PageSize, entities.TotalCount)
            });
        }


        [Route("details")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Vendors.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _venderService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.WareHouse"))
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
        /// Lấy dữ liệu nhà cung cấp theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _venderService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Vendor"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy danh sách nhà cung cấp cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _venderService.GetAll(showHidden);

            List<VendorModel> result = new List<VendorModel>();

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
        private async Task UpdateLocalesAsync(Core.Domain.Warehouse.Vendor entity, VendorModel model)
        {
            foreach (var local in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, local.Name, local.LanguageId);
            }
        }

        private async Task<bool> ValidateImportDataAsync(List<VendorModel> models,
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
            var existCodes = await _venderService.ExistCodesAsync(lstCodes);

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
        #endregion
    }
}
