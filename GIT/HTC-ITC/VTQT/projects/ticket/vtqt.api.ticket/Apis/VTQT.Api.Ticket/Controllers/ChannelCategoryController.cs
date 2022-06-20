using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
    [Route("channel-category")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ChannelCategory")]
    public class ChannelCategoryController : AdminApiController
    {
        #region Fields

        private readonly IChannelCategoryService _channelCategoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;

        #endregion

        #region Ctor

        public ChannelCategoryController(
            IChannelCategoryService channelCategoryService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService
            )
        {
            _channelCategoryService = channelCategoryService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _autoCodeService = autoCodeService;
        }

        #endregion

        #region Methods

        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ChannelCategory.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _channelCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ChannelCategory"))
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
        [AppApiAction("Ticket.AppActions.ChannelCategory.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new ChannelCategoryModel();

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Insert a newly ChannelCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Created successfully</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.ChannelCategory.Create")]
        public async Task<IActionResult> Create(ChannelCategoryModel model)
        {
            if (model?.Code == null || !model.Code.Any())
            {
                model.Code = await _autoCodeService.GenerateCode(nameof(ChannelCategory));
            }

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _channelCategoryService.ExistsAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.ChannelCategory.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _channelCategoryService.InsertAsync(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.ChannelCategory"))
            });
        }

        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ChannelCategory.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _channelCategoryService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ChannelCategory"))
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
        /// Update a ChannelCategory item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.ChannelCategory.Edit")]
        public async Task<IActionResult> Edit(ChannelCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _channelCategoryService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ChannelCategory"))
                });

            entity = model.ToEntity(entity);

            await _channelCategoryService.UpdateAsync(entity);
            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.ChannelCategory"))
            });
        }

        /// <summary>
        /// Delete a list ChannelCategory
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.ChannelCategory.Deletes")]
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

            await _channelCategoryService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ChannelCategory"))
            });
        }

        /// <summary>
        /// Active a list ChannelCategory
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

            await _channelCategoryService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.ChannelCategory"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.ChannelCategory"))
            });
        }

        #endregion Methods

        #region Lists

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ChannelCategory.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Get a list ChannelCategory
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ChannelCategorySearchModel searchModel)
        {
            var searchContext = new ChannelCategorySearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<ChannelCategoryModel>();
            var entities = _channelCategoryService.Get(searchContext);
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
        /// Lấy danh sách ChannelCategory cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _channelCategoryService.GetAll(showHidden);

            List<ChannelCategoryModel> result = new List<ChannelCategoryModel>();

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

        private async Task UpdateLocalesAsync(ChannelCategory entity, ChannelCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }
        #endregion
    }
}
