using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.Services.Asset.Helper;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("asset-category")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AssetCategory")]
    public class AssetCategoryController : AdminApiController
    {
        #region Fields
        private readonly IAssetCategoryService _assetCategoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;        
        #endregion

        #region Ctor
        public AssetCategoryController(
            IAssetCategoryService assetCategoryService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService)
        {
            _assetCategoryService = assetCategoryService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetCategories.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Tạo mới loại tài sản rỗng có đa ngôn ngữ
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetCategories.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new AssetCategoryModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Thêm mới loại tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Thêm mới thành công</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AssetCategories.Create")]
        public async Task<IActionResult> Create(AssetCategoryModel model)
        {
            model.Code = model.Code?.Trim();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _assetCategoryService.ExistAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Asset.AssetCategories.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _assetCategoryService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.AssetCategory"))
            });
        }

        /// <summary>
        /// Trả về loại tài sản cập nhật có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetCategories.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _assetCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetCategory"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Cập nhật loại tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Cập nhật thành công</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AssetCategories.Edit")]
        public async Task<IActionResult> Edit(AssetCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _assetCategoryService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetCategory"))
                });

            entity = model.ToEntity(entity);

            await _assetCategoryService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.AssetCategory"))
            });
        }

        /// <summary>
        /// Chi tiết loại tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetCategories.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _assetCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetCategory"))
                });

            var model = entity.ToModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Xóa loại tài sản theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Xóa thành công</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AssetCategories.Deletes")]
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

            await _assetCategoryService.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.AssetCategory"))
            });
        }

        /// <summary>
        /// Kích hoạt/ ngừng kích hoạt loại tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Thao tác thành công</response>
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

            await _assetCategoryService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                success = true,
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.AssetCategory"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.AssetCategory"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu loại tài sản theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Lấy dữ liệu thành công</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _assetCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetCategory"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách loại tài sản phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Lấy danh sách loại tài sản thành công</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] AssetCategorySearchModel searchModel)
        {
            var searchContext = new AssetCategorySearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<AssetCategoryModel>();
            var entities = _assetCategoryService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);                
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
        /// Lấy danh sách loại tài sản cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        /// <response code="200">Lấy danh sách loại tài sản thành công</response>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<ActionResult> GetAvailable(int assetType, bool showHidden = false)
        {
            var entities = _assetCategoryService.GetAll(showHidden, assetType);
            var models = new List<AssetCategoryModel>();

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = e.ToModel();                    
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {                
                data = models
            });
        }

        [Route("get-select")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Getall()
        {
            var entity = _assetCategoryService.GetAll(false);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetCategory"))
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
        #endregion

        #region Utilities
        private async Task UpdateLocalesAsync(AssetCategory entity, AssetCategoryModel model)
        {
            foreach (var local in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, local.Name, local.LanguageId);
            }
        }
        #endregion
    }
}
