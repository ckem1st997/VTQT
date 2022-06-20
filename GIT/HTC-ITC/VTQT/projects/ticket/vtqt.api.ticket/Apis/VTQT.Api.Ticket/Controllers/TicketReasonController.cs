using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("ticket-reason")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.TicketReason")]
    public class TicketReasonController : AdminApiController
    {
        #region Fields
        private readonly ITicketReasonService _ticketReasonService;
        #endregion

        #region Ctor
        public TicketReasonController(ITicketReasonService ticketReasonService)
        {
            _ticketReasonService = ticketReasonService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.TicketReason.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
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

            var entity = await _ticketReasonService.GetByIdAsync(id);
            var model = new TicketReasonModel();

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
        #endregion

        #region List
        [Route("get-list-detail")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetDetailReasonsByReasonId(string reasonId)
        {
            var results = _ticketReasonService.GetDetailReasonsByReasonId(reasonId);

            return Ok(new XBaseResult
            {
                data = results
            });
        }

        /// <summary>
        /// Lấy danh sách reason cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden, string projectId)
        {
            var availableList = _ticketReasonService.GetAvailable(showHidden, projectId);

            List<TicketReasonModel> result = new List<TicketReasonModel>();

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
    }
}
