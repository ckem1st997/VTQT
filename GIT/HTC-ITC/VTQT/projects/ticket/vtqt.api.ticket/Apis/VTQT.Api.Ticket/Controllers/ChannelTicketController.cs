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
    [Route("channel-ticket")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ChannelTicket")]
    public class ChannelTicketController : AdminApiController
    {
        #region Fields

        private readonly IChannelTicketService _channelTicketService;
        private readonly IChannelService _channelService;
        private readonly IChannelCategoryService _channelCategoryService;
        private readonly ICableService _cableService;
        private readonly IPhenomenaService _phenomenaService;

        #endregion

        #region Ctor

        public ChannelTicketController(
            IChannelService channelService,
            IChannelTicketService channelTicketService,
            IChannelCategoryService channelCategoryService,
            ICableService cableService,
            IPhenomenaService phenomenaService)
        {
            _channelService = channelService;
            _channelCategoryService = channelCategoryService;
            _channelTicketService = channelTicketService;
            _cableService = cableService;
            _phenomenaService = phenomenaService;
        }

        #endregion

        #region Methods

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.ChannelTicket.Deletes")]
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

            await _channelTicketService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ChannelTicket"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ChannelTicket.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ChannelTicket.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new ChannelTicketModel
            {
                AvailableCables = _cableService.GetMvcListItems(false),
                AvailableCategories = _channelCategoryService.GetMvcListItems(false),
                AvailableChannels = _channelService.GetMvcListItems(false),
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
        //[AppApiAction("Ticket.AppActions.ChannelTicket.Create")]
        public async Task<IActionResult> Create(ChannelTicketModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.StartDate = model.StartDate;
            entity.FinishDate = model.FinishDate;
            entity.ChannelName = _channelService.GetMvcListItems(false).ToList().Where(x => x.Value.Equals(entity.ChannelId)).FirstOrDefault().Text;

            await _channelTicketService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.NetworkLink"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ChannelTicket.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _channelTicketService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Channel"))
                });

            var model = entity.ToModel();
            model.AvailableCables = _cableService.GetMvcListItems(false);
            model.AvailableCategories = _channelCategoryService.GetMvcListItems(false);
            model.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
            model.AvailableChannels = _channelService.GetMvcListItems(false);
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.ChannelTicket.Index")]
        public async Task<IActionResult> Edit(ChannelTicketModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _channelTicketService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Channel"))
                });

            entity = model.ToEntity(entity);
            entity.StartDate = model.StartDate;
            entity.FinishDate = model.FinishDate;
            entity.ChannelName = _channelService.GetMvcListItems(false).ToList().Where(x => x.Value.Equals(entity.ChannelId)).FirstOrDefault().Text;

            await _channelTicketService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Channel"))
            });
        }
        #endregion

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ChannelTicketSearchModel searchModel)
        {
            var searchContext = new ChannelTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                TicketId = searchModel.TicketId
            };

            var models = new List<ChannelTicketModel>();
            var entities = _channelTicketService.Get(searchContext);
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
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] ChannelTicketSearchModel searchModel)
        {
            var searchContext = new ChannelTicketSearchContext
            {
                TicketId = searchModel.TicketId
            };

            var models = new List<ChannelTicketModel>();
            var entities = _channelTicketService.GetByChannelTicketId(searchContext);

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