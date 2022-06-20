using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("approval-cr")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ApprovalCR")]
    public class ApprovalCRController : AdminApiController
    {
        #region Fields

        private readonly IApprovalCRService _approvalCRService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion Fields

        #region Ctor

        public ApprovalCRController(IApprovalCRService approvalCRService,
            IApprovalProgressService approvalProgressService)
        {
            _approvalCRService = approvalCRService;
            _approvalProgressService = approvalProgressService;
        }

        #endregion Ctor

        #region Methods

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.ApprovalCR.Deletes")]
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

            await _approvalCRService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ApprovalCR"))
            });
        }

        // GET
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ApprovalCR.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ApprovalCR.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new ApprovalCRModel
            {
                AvailableApprovers = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ApproverCR.DeputyGeneralManagerTechnical).ToString(),
                        Text = ApproverCR.DeputyGeneralManagerTechnical.GetEnumDescription()
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
        //[AppApiAction("Ticket.AppActions.ApprovalCR.Create")]
        public async Task<IActionResult> Create(ApprovalCRModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _approvalCRService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.ApprovalCR"))
            });
        }

        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] ApprovalCRSearchModel searchModel)
        {
            var searchContext = new ApprovalCRSearchContext
            {
                CrId = searchModel.CrId
            };

            var models = new List<ApprovalCRModel>();
            var entities = _approvalCRService.GetByApprovalCRId(searchContext);

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

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ApprovalCR.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _approvalCRService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ApprovalCR"))
                });

            var model = entity.ToModel();
            model.AvailableProgress = _approvalProgressService.GetMvcListItems(false);
            model.AvailableApprovers = new List<SelectListItem>
            {
                new SelectListItem
                    {
                        Value = ((int)ApproverCR.DeputyGeneralManagerTechnical).ToString(),
                        Text = ApproverCR.DeputyGeneralManagerTechnical.GetEnumDescription()
                    }
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.ApprovalCR.Index")]
        public async Task<IActionResult> Edit(ApprovalCRModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _approvalCRService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ApprovalCR"))
                });

            entity = model.ToEntity(entity);

            await _approvalCRService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.ApprovalCR"))
            });
        }

        #endregion Methods

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ApprovalCRSearchModel searchModel)
        {
            var searchContext = new ApprovalCRSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                CrId = searchModel.CrId
            };

            var models = new List<ApprovalCRModel>();
            var entities = _approvalCRService.Get(searchContext);
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
    }
}