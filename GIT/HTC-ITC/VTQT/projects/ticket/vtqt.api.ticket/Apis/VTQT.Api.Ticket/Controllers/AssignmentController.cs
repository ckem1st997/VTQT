using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Ticket.Enum;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("assignment")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Assignment")]
    public class AssignmentController : AdminApiController
    {
        #region Fields

        private readonly IAssignmentService _assignmentService;
        private readonly ITicketService _ticketService;
        private readonly IUserModelHelper _userModelHelper;

        #endregion

        #region Ctor

        public AssignmentController(
            IAssignmentService assignmentService,
            ITicketService ticketService,
            IUserModelHelper userModelHelper)
        {
            _assignmentService = assignmentService;
            _ticketService = ticketService;
            _userModelHelper = userModelHelper;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Assignment.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Assignment.Create")]
        public async Task<IActionResult> Create()
        {
            var m = new AssignmentModel
            {
                AvailableReasons = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)AssignReason.Escalate).ToString(),
                        Text = AssignReason.Escalate.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)AssignReason.Assign).ToString(),
                        Text = AssignReason.Assign.GetEnumDescription()
                    }
                },
                AvailableUsers = _userModelHelper.GetMvcListItems()
            };

            return Ok(new XBaseResult
            {
                data = m
            });
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Assignment.Create")]
        public async Task<IActionResult> Create(AssignmentModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.AssignmentDate = DateTime.UtcNow;

            await _assignmentService.InsertAsync(entity);

            var ticket = await _ticketService.GetByIdAsync(model.TicketId);
            ticket.Assignee = entity.Assignee;
            ticket.ModifiedDate = DateTime.UtcNow;

            await _ticketService.UpdateAsync(ticket);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Assignment"))
            });
        }

        #endregion
    }
}
