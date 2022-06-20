using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("infrastructor-fee")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.InfrastructorFee")]
    public class InfrastructorFeeController : AdminApiController
    {
        #region Fields
        private readonly IInfrastructorFeeService _infrastructorFeeServiceService;

        #endregion

        #region Ctor

        public InfrastructorFeeController(IInfrastructorFeeService iInfrastructorFeeService)
        {
            _infrastructorFeeServiceService = iInfrastructorFeeService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.InfrastructorFee.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.InfrastructorFee.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new InfrastructorFeeModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.InfrastructorFee.Create")]
        public async Task<IActionResult> Create(InfrastructorFeeModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _infrastructorFeeServiceService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.InfrastructorFee"))
            });
        }

        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.InfrastructorFee.Index")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _infrastructorFeeServiceService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.InfrastructorFee"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.InfrastructorFee.Index")]
        public async Task<IActionResult> Edit(InfrastructorFeeModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _infrastructorFeeServiceService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.InfrastructorFee"))
                });

            entity = model.ToEntity(entity);

            await _infrastructorFeeServiceService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.InfrastructorFee"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("WareHouse.AppActions.InfrastructorFee.Deletes")]
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

            await _infrastructorFeeServiceService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.InfrastructorFee"))
            });
        }

        #endregion

        #region List

        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] InfrastructorSearchFeeModel searchModel)
        {
            var searchContext = new InfrastructorFeeSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                TicketId = searchModel.TicketId
            };

            var models = new List<InfrastructorFeeModel>();
            var entities = _infrastructorFeeServiceService.Get(searchContext);
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
        public async Task<IActionResult> DetailGet([FromQuery] InfrastructorSearchFeeModel searchModel)
        {
            var searchContext = new InfrastructorFeeSearchContext
            {
                TicketId = searchModel.TicketId
            };

            var models = new List<InfrastructorFeeModel>();
            var entities = _infrastructorFeeServiceService.GetByInfrastructorFeeId(searchContext);

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