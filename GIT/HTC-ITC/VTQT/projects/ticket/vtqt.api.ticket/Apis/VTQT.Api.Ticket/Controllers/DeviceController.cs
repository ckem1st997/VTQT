using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Services.Localization;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("device")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Device")]
    public class DeviceController : AdminApiController
    {
        #region Fields

        private readonly IDeviceService _deviceService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion Fields

        #region Ctor

        public DeviceController(
            IDeviceService deviceService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService)
        {
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _deviceService = deviceService;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Khởi tạo đối tượng thiết bị
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Device.Create")]
        public IActionResult Create()
        {
            var model = new DeviceModel();
            AddMvcLocales(_languageService, model.Locales);
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới thiết bị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Device.Create")]
        public async Task<IActionResult> Create(DeviceModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var entity = model.ToEntity();

            await _deviceService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Device"))
            });
        }

        /// <summary>
        /// Lấy dữ thiết bị cần update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Device.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _deviceService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Device"))
                });

            var model = entity.ToModel();
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
        /// Cập nhật thiết bị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Device.Edit")]
        public async Task<IActionResult> Edit(DeviceModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _deviceService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Device"))
                });

            entity = model.ToEntity(entity);

            await _deviceService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Device"))
            });
        }

        /// <summary>
        /// Lấy chi tiết thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Device.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _deviceService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Device"))
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
        /// Xóa danh sách thiết bị
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Device.Deletes")]
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

            await _deviceService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Device"))
            });
        }

        /// <summary>
        /// Kích hoạt trạng thái thiết bị
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

            await _deviceService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.Device"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.Device"))
            });
        }
        
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var entity = await _deviceService.GetByIdAsync(id);
            var model = new DeviceModel();

            if (entity != null)
            {
                model = entity.ToModel();
            }

            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }

        #endregion Methods

        #region List

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Device.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu danh sách thiết bị
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] DeviceSearchModel searchModel)
        {
            var searchContext = new DeviceSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<DeviceModel>();
            var entities = _deviceService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        #endregion List

        #region Utilities

        private async Task UpdateLocalesAsync(Device entity, DeviceModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        #endregion Utilities
    }
}