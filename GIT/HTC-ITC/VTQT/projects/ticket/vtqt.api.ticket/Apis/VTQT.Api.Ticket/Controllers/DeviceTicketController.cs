using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("device-ticket")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.DeviceTicket")]
    public class DeviceTicketController : AdminApiController
    {
        #region Fields
        private readonly IDeviceTicketService _deviceTicketService;
        private readonly IDeviceService _deviceService;
        private readonly IPhenomenaService _phenomenaService;
        private readonly IDeviceCategoryService _deviceCategoryService;

        #endregion

        #region Ctor

        public DeviceTicketController(
            IDeviceTicketService deviceTicketService,
            IDeviceService deviceService,
            IPhenomenaService phenomenaService,
            IDeviceCategoryService deviceCategoryService)
        {
            _deviceTicketService = deviceTicketService;
            _deviceService = deviceService;
            _phenomenaService = phenomenaService;
            _deviceCategoryService = deviceCategoryService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.DeviceTicket.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.DeviceTicket.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new DeviceTicketModel
            {
                AvailableCategories = _deviceCategoryService.GetMvcListItems(false),
                AvailableDevices = _deviceService.GetMvcListItems(false),
                AvailablePhenomenas = _phenomenaService.GetMvcListItems(false),
                StartDate = DateTime.Now
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.DeviceTicket.Create")]
        public async Task<IActionResult> Create(DeviceTicketModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.StartDate = model.StartDate;
            entity.FinishDate = model.FinishDate;
            entity.DeviceName = _deviceService.GetMvcListItems(false).ToList().Where(x => x.Value.Equals(entity.DeviceId)).FirstOrDefault().Text;

            await _deviceTicketService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.DeviceTicket"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.DeviceTicket.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _deviceTicketService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Device"))
                });

            var model = entity.ToModel();
            model.AvailableCategories = _deviceCategoryService.GetMvcListItems(false);
            model.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
            model.AvailableDevices = _deviceService.GetMvcListItems(false);
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.DeviceTicket.Index")]
        public async Task<IActionResult> Edit(DeviceTicketModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _deviceTicketService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Device"))
                });

            entity = model.ToEntity(entity);
            entity.StartDate = model.StartDate;
            entity.FinishDate = model.FinishDate;
            entity.DeviceName = _deviceService.GetMvcListItems(false).ToList().Where(x => x.Value.Equals(entity.DeviceId)).FirstOrDefault().Text;

            await _deviceTicketService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Device"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.DeviceTicket.Deletes")]
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

            await _deviceTicketService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.DeviceTicket"))
            });
        }

        #endregion

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] DeviceTicketSearchModel searchModel)
        {
            var searchContext = new DeviceTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                TicketId = searchModel.TicketId
            };

            var models = new List<DeviceTicketModel>();
            var entities = _deviceTicketService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.FinishDate = e.FinishDate.ToLocalTime();
                m.StartDate = e.StartDate.ToLocalTime();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] DeviceTicketSearchModel searchModel)
        {
            var searchContext = new DeviceTicketSearchContext
            {
                TicketId = searchModel.TicketId
            };

            var models = new List<DeviceTicketModel>();
            var entities = _deviceTicketService.GetByDeviceTicketId(searchContext);

            foreach (var e in entities)
            {
                var m = e.ToModel();

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        #endregion

        #region Utilities



        #endregion
    }
}