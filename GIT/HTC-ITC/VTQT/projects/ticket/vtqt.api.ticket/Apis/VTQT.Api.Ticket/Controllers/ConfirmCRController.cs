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
    [Route("confirm-cr")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.ConfirmCR")]
    public class ConfirmCRController : AdminApiController
    {
        #region Fields

        private readonly IConfirmCRService _confirmCRService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion Fields

        #region Ctor

        public ConfirmCRController(IConfirmCRService confirmCRService,
            IApprovalProgressService approvalProgressService)
        {
            _confirmCRService = confirmCRService;
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

            await _confirmCRService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.ConfirmCR"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.ConfirmCR.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ConfirmCRModel
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
                AvailableConfirmClass = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.DepartmentNoc).ToString(),
                        Text = ConfirmCR.DepartmentNoc.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.DepartmentMS).ToString(),
                        Text = ConfirmCR.DepartmentMS.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.DepartmentCSKH).ToString(),
                        Text = ConfirmCR.DepartmentCSKH.GetEnumDescription()
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
        public async Task<IActionResult> Create(ConfirmCRModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _confirmCRService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.ConfirmCR"))
            });
        }

        [Route("edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _confirmCRService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ConfirmCR"))
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

            model.AvailableConfirmClass = new List<SelectListItem>
            {
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.DepartmentNoc).ToString(),
                        Text = ConfirmCR.DepartmentNoc.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.DepartmentMS).ToString(),
                        Text = ConfirmCR.DepartmentMS.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ConfirmCR.DepartmentCSKH).ToString(),
                        Text = ConfirmCR.DepartmentCSKH.GetEnumDescription()
                    }
            };

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(ConfirmCRModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _confirmCRService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.ConfirmCR"))
                });

            entity = model.ToEntity(entity);

            await _confirmCRService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.ConfirmCR"))
            });
        }

        [Route("detail-get")]
        [HttpGet]
        public async Task<IActionResult> DetailGet([FromQuery] ConfirmCRSearchModel searchModel)
        {
            var searchContext = new ConfirmCRSearchContext
            {
                CrId = searchModel.CrId
            };

            var models = new List<ConfirmCRModel>();
            var entities = _confirmCRService.GetByConfirmCRId(searchContext);

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
        public async Task<IActionResult> Get([FromQuery] ConfirmCRSearchModel searchModel)
        {
            var searchContext = new ConfirmCRSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                CrId = searchModel.CrId
            };

            var models = new List<ConfirmCRModel>();
            var entities = _confirmCRService.Get(searchContext);
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