using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain.Ticket.Enum;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("approval-ticket")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ApprovalTicket")]
    public class ApprovalTicketController : AdminApiController
    {
        #region Fields

        private readonly IApprovalTicketService _approvalTicketService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion

        #region Ctor

        public ApprovalTicketController(
            IApprovalTicketService approvalTicketService,
            IApprovalProgressService approvalProgressService)
        {
            _approvalTicketService = approvalTicketService;
            _approvalProgressService = approvalProgressService;
        }

        #endregion

        #region Methods

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.ApprovalTicket.Deletes")]
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

            await _approvalTicketService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ApprovalTicket"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ApprovalTicket.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ApprovalTicket.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new ApprovalTicketModel
            {
                AvailableApprovers = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ApproverTicket.DeputyGeneralManagerTechnical).ToString(),
                        Text = ApproverTicket.DeputyGeneralManagerTechnical.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ApproverTicket.DeputyGeneralManagerNetworkInfrastructor).ToString(),
                        Text = ApproverTicket.DeputyGeneralManagerNetworkInfrastructor.GetEnumDescription()
                    }
                },
                AvailableProgress = _approvalProgressService.GetMvcListItems(false)
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.ApprovalTicket.Create")]
        public async Task<IActionResult> Create(ApprovalTicketModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _approvalTicketService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.ApprovalTicket"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ApprovalTicket.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _approvalTicketService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ApprovalTicket"))
                });

            var model = entity.ToModel();
            model.AvailableProgress = _approvalProgressService.GetMvcListItems(false);
            model.AvailableApprovers = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)ApproverTicket.DeputyGeneralManagerTechnical).ToString(),
                    Text = ApproverTicket.DeputyGeneralManagerTechnical.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)ApproverTicket.DeputyGeneralManagerNetworkInfrastructor).ToString(),
                    Text = ApproverTicket.DeputyGeneralManagerNetworkInfrastructor.GetEnumDescription()
                }
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.ApprovalTicket.Index")]
        public async Task<IActionResult> Edit(ApprovalTicketModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _approvalTicketService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ApprovalTicket"))
                });

            entity = model.ToEntity(entity);

            await _approvalTicketService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.ApprovalTicket"))
            });
        }
        #endregion

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ApprovalTicketSearchModel searchModel)
        {
            var searchContext = new ApprovalTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                TicketId=searchModel.TicketId
            };

            var models = new List<ApprovalTicketModel>();
            var entities = _approvalTicketService.Get(searchContext);
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

        [Route("get-detail")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetDetail([FromQuery] ApprovalTicketSearchModel searchModel)
        {
            var searchContext = new ApprovalTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                TicketId = searchModel.TicketId
            };

            var models = new List<ApprovalTicketModel>();
            var entities = _approvalTicketService.GetDetail(searchContext);
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

        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] ApprovalTicketSearchModel searchModel)
        {
            var searchContext = new ApprovalTicketSearchContext
            {
                TicketId = searchModel.TicketId
            };

            var models = new List<ApprovalTicketModel>();
            var entities = _approvalTicketService.GetByApprovalTicketId(searchContext);

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