using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("network-linkCategory")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.NetworkLinkCategory")]
    public class NetworkLinkCategoryController : AdminApiController
    {
        #region Fields

        private readonly INetworkLinkCategoryService _networkLinkCategoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;

        #endregion

        #region Ctor

        public NetworkLinkCategoryController(
            INetworkLinkCategoryService networkLinkCategoryService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService
            )
        {
            _networkLinkCategoryService = networkLinkCategoryService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _autoCodeService = autoCodeService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _networkLinkCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLinkCategory"))
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

        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new NetworkLinkCategoryModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Insert a newly NetworkLinkCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Create")]
        public async Task<IActionResult> Create(NetworkLinkCategoryModel model)
        {
            if (model?.Code == null || !model.Code.Any())
            {
                model.Code = await _autoCodeService.GenerateCode(nameof(NetworkLinkCategory));
            }

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _networkLinkCategoryService.ExistsAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.NetworkLinkCategory.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _networkLinkCategoryService.InsertAsync(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.NetworkLinkCategory"))
            });
        }

        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _networkLinkCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLinkCategory"))
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
        /// Update a NetworkLinkCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Edit")]
        public async Task<IActionResult> Edit(NetworkLinkCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _networkLinkCategoryService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLinkCategory"))
                });

            entity = model.ToEntity(entity);

            await _networkLinkCategoryService.UpdateAsync(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.NetworkLinkCategory"))
            });
        }

        /// <summary>
        /// Delete a list NetworkLinkCategory
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.NetworkLinkCategory.Deletes")]
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

            await _networkLinkCategoryService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.NetworkLinkCategory"))
            });
        }

        /// <summary>
        /// Active a list NetworkLinkCategory
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

            await _networkLinkCategoryService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.NetworkLinkCategory"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.NetworkLinkCategory"))
            });
        }

        #endregion Methods

        #region Lists

        /// <summary>
        /// Get a list NetworkLinkCategory
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] NetworkLinkCategorySearchModel searchModel)
        {
            var searchContext = new NetworkLinkCategorySearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<NetworkLinkCategoryModel>();
            var entities = _networkLinkCategoryService.Get(searchContext);
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
        /// Lấy danh sách loại link mạng cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _networkLinkCategoryService.GetAll(showHidden);

            List<NetworkLinkCategoryModel> result = new List<NetworkLinkCategoryModel>();

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

        private async Task UpdateLocalesAsync(NetworkLinkCategory entity, NetworkLinkCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }
        #endregion
    }
}