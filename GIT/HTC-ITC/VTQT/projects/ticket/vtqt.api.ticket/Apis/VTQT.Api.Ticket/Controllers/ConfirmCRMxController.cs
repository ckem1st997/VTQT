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
    [Route("confirm-cr-mx")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ConfirmCRMx")]
    public class ConfirmCRMxController : AdminApiController
    {
        #region Fields

        private readonly IConfirmCRMxService _confirmCRMxService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion Fields

        #region Ctor

        public ConfirmCRMxController(IConfirmCRMxService confirmCRMxService,
            IApprovalProgressService approvalProgressService)
        {
            _confirmCRMxService = confirmCRMxService;
            _approvalProgressService = approvalProgressService;
        }

        #endregion Ctor

        #region Methods

        [Route("deletes")]
        [HttpPost]
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

            await _confirmCRMxService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ConfirmCRMx"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ConfirmCRMx.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.ConfirmCRMx.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new ConfirmCRMxModel
            {
                AvailableConfirmCRs = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.Noc).ToString(),
                        Text = ConfirmCR.Noc.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.MS).ToString(),
                        Text = ConfirmCR.MS.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.CS).ToString(),
                        Text = ConfirmCR.CS.GetEnumDescription()
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
        public async Task<IActionResult> Create(ConfirmCRMxModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _confirmCRMxService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.ConfirmCRMx"))
            });
        }

        [Route("edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _confirmCRMxService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ConfirmCRMx"))
                });

            var model = entity.ToModel();
            model.AvailableProgress = _approvalProgressService.GetMvcListItems(false);
            model.AvailableConfirmCRs = new List<SelectListItem>
            {
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.Noc).ToString(),
                        Text = ConfirmCR.Noc.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.MS).ToString(),
                        Text = ConfirmCR.MS.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.CS).ToString(),
                        Text = ConfirmCR.CS.GetEnumDescription()
                    }
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(ConfirmCRMxModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _confirmCRMxService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ConfirmCRMx"))
                });

            entity = model.ToEntity(entity);

            await _confirmCRMxService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.ConfirmCRMx"))
            });
        }

        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] ConfirmCRMxSearchModel searchModel)
        {
            var searchContext = new ConfirmCRMxSearchContext
            {
                CrMxId = searchModel.CrMxId
            };

            var models = new List<ConfirmCRMxModel>();
            var entities = _confirmCRMxService.GetByConfirmCRMxId(searchContext);

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
        public async Task<IActionResult> Get([FromQuery] ConfirmCRMxSearchModel searchModel)
        {
            var searchContext = new ConfirmCRMxSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                CrMxId = searchModel.CrMxId
            };

            var models = new List<ConfirmCRMxModel>();
            var entities = _confirmCRMxService.Get(searchContext);
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