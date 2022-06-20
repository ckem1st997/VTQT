using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("infrastructor-fee-cr")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.InfrastructorFeeCR")]
    public class InfrastructorFeeCRController : AdminApiController
    {
        #region Fields
        private readonly IInfrastructorFeeCRService _infrastructorFeeCRServiceService;

        #endregion Fields

        #region Ctor

        public InfrastructorFeeCRController(IInfrastructorFeeCRService iInfrastructorFeeCRService)
        {
            _infrastructorFeeCRServiceService = iInfrastructorFeeCRService;
        }

        #endregion Ctor

        #region Methods

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.InfrastructorFeeCR.Deletes")]
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

            await _infrastructorFeeCRServiceService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.InfrastructorFeeCR"))
            });
        }

        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.InfrastructorFeeCR.Create")]
        public async Task<IActionResult> Create(InfrastructorFeeCRModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _infrastructorFeeCRServiceService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.InfrastructorFeeCR"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.InfrastructorFeeCR.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _infrastructorFeeCRServiceService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.InfrastructorFeeCR"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.InfrastructorFeeCR.Index")]
        public async Task<IActionResult> Edit(InfrastructorFeeCRModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _infrastructorFeeCRServiceService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.InfrastructorFeeCR"))
                });

            entity = model.ToEntity(entity);

            await _infrastructorFeeCRServiceService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.InfrastructorFeeCR"))
            });
        }

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.InfrastructorFeeCR.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.InfrastructorFeeCR.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new InfrastructorFeeCRModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("detail-get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] InfrastructorFeeCRSearchModel searchModel)
        {
            var searchContext = new InfrastructorFeeCRSearchContext
            {
                CrId = searchModel.CrId
            };

            var models = new List<InfrastructorFeeCRModel>();
            var entities = _infrastructorFeeCRServiceService.GetByInfrastructorFeeCRId(searchContext);

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
        public async Task<IActionResult> Get([FromQuery] InfrastructorFeeCRSearchModel searchModel)
        {
            var searchContext = new InfrastructorFeeCRSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                CrId = searchModel.CrId
            };

            var models = new List<InfrastructorFeeCRModel>();
            var entities = _infrastructorFeeCRServiceService.Get(searchContext);
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