using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using System.Linq;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("network-link-ticket")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.NetworkLinkTicket")]
    public class NetworkLinkTicketController : AdminApiController
    {
        #region Fields

        private readonly INetworkLinkTicketService _networkLinkTicketService;
        private readonly INetworkLinkService _networkLinkService;
        private readonly INetworkLinkCategoryService _networkLinkCategoryService;
        private readonly ICableService _cableService;
        private readonly IPhenomenaService _phenomenaService;

        #endregion

        #region Ctor

        public NetworkLinkTicketController(
            INetworkLinkTicketService networkLinkTicketService,
            INetworkLinkService networkLinkService,
            ICableService cableService,
            INetworkLinkCategoryService networkLinkCategoryService,
            IPhenomenaService phenomenaService)
        {
            _networkLinkTicketService = networkLinkTicketService;
            _networkLinkService = networkLinkService;
            _networkLinkCategoryService = networkLinkCategoryService;
            _cableService = cableService;
            _phenomenaService = phenomenaService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLinkTicket.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.NetworkLinkTicket.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new NetworkLinkTicketModel
            {
                AvailableCables = _cableService.GetMvcListItems(false),
                AvailableCategories = _networkLinkCategoryService.GetMvcListItems(false),
                AvailablePhenomenas = _phenomenaService.GetMvcListItems(false),
                AvailableNetworkLinks = _networkLinkService.GetMvcListItems(false),
                StartDate = DateTime.Now
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.NetworkLinkTicket.Create")]
        public async Task<IActionResult> Create(NetworkLinkTicketModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.StartDate = model.StartDate;
            entity.FinishDate = model.FinishDate;
            entity.NetworkLinkName = _networkLinkService.GetMvcListItems(false).ToList()
                .Where(x => x.Value.Equals(entity.NetworkLinkId)).FirstOrDefault().Text;

            await _networkLinkTicketService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                data = entity,
                message = string.Format(T("Common.Notify.Added"), T("Common.NetworkLink"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.NetworkLinkTicket.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _networkLinkTicketService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLink"))
                });

            var model = entity.ToModel();
            model.AvailableCables = _cableService.GetMvcListItems(false);
            model.AvailableCategories = _networkLinkCategoryService.GetMvcListItems(false);
            model.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
            model.AvailableNetworkLinks = _networkLinkService.GetMvcListItems(false);
            model.StartDate = entity.StartDate.ToLocalTime();
            model.FinishDate = entity.FinishDate.ToLocalTime();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.NetworkLinkTicket.Index")]
        public async Task<IActionResult> Edit(NetworkLinkTicketModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _networkLinkTicketService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLink"))
                });

            entity = model.ToEntity(entity);
            entity.StartDate = model.StartDate;
            entity.FinishDate = model.FinishDate;
            entity.NetworkLinkName = _networkLinkService.GetMvcListItems(false).ToList()
                .Where(x => x.Value.Equals(entity.NetworkLinkId)).FirstOrDefault().Text;

            await _networkLinkTicketService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                data = entity,
                message = string.Format(T("Common.Notify.Updated"), T("Common.NetworkLink"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.NetworkLinkTicket.Deletes")]
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

            await _networkLinkTicketService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.NetworkLinkTicket"))
            });
        }

        #endregion

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] NetworkLinkTicketSearchModel searchModel)
        {
            var searchContext = new NetworkLinkTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                TicketId = searchModel.TicketId
            };

            var models = new List<NetworkLinkTicketModel>();
            var entities = _networkLinkTicketService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.FinishDate = e.FinishDate.ToLocalTime();
                m.StartDate = e.StartDate.ToLocalTime();
                m.StartDateToString = m.StartDate.ToString(CultureInfo.InvariantCulture);
                m.FinishDateToString =m.FinishDate==null?"": m.FinishDate.ToString();
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
        public async Task<IActionResult> DetailGet([FromQuery] NetworkLinkTicketSearchModel searchModel)
        {
            var searchContext = new NetworkLinkTicketSearchContext
            {
                TicketId = searchModel.TicketId
            };

            var models = new List<NetworkLinkTicketModel>();
            var entities = _networkLinkTicketService.GetByNetworkLinkTicketId(searchContext);

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