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
    [Route("approval-cr-mx")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ApprovalCRMx")]
    public class ApprovalCRMxController : AdminApiController
    {
        #region Fields

        private readonly IApprovalCRMxService _approvalCRMxService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion Fields

        #region Ctor

        public ApprovalCRMxController(IApprovalCRMxService approvalCRMxService,
            IApprovalProgressService approvalProgressService)
        {
            _approvalCRMxService = approvalCRMxService;
            _approvalProgressService = approvalProgressService;
        }

        #endregion Ctor

        #region Methods

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.ApprovalCRMx.Deletes")]
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

            await _approvalCRMxService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ApprovalCRMx"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ApprovalCRMx.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ApprovalCRMx.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new ApprovalCRMxModel
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
        //[AppApiAction("Ticket.AppActions.ApprovalCRMx.Create")]
        public async Task<IActionResult> Create(ApprovalCRMxModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _approvalCRMxService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.ApprovalCRMx"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ApprovalCRMx.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _approvalCRMxService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ApprovalCRMx"))
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
        //[AppApiAction("Ticket.AppActions.ApprovalCRMx.Index")]
        public async Task<IActionResult> Edit(ApprovalCRMxModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _approvalCRMxService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ApprovalCRMx"))
                });

            entity = model.ToEntity(entity);

            await _approvalCRMxService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.ApprovalCRMx"))
            });
        }

        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] ApprovalCRMxSearchModel searchModel)
        {
            var searchContext = new ApprovalCRMxSearchContext
            {
                CrMxId = searchModel.CrMxId
            };

            var models = new List<ApprovalCRMxModel>();
            var entities = _approvalCRMxService.GetByApprovalCRMxId(searchContext);

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

        #endregion Methods

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ApprovalCRMxSearchModel searchModel)
        {
            var searchContext = new ApprovalCRMxSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                CrMxId = searchModel.CrMxId
            };

            var models = new List<ApprovalCRMxModel>();
            var entities = _approvalCRMxService.Get(searchContext);
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