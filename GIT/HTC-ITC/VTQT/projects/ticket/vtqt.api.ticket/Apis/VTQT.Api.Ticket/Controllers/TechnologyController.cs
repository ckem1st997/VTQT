using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("technology")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Technology")]
    public class TechnologyController : AdminApiController
    {
        #region Fields

        private readonly ITechnologyService _technologyService;

        #endregion Fields

        #region Ctor

        public TechnologyController(ITechnologyService technologyService)
        {
            _technologyService = technologyService;
        }

        #endregion Ctor

        #region Method

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Technology.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Technology.Create")]
        public IActionResult Create()
        {
            var model = new TechnologyModel();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Technology.Create")]
        public async Task<IActionResult> Create(TechnologyModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _technologyService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Technology"))
            });
        }

        /// <summary>
        /// Lấy dữ Technology
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Technology.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _technologyService.GetByIdAsync(id);
            
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Technology"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật Technology
        /// </summary>
        /// <param name="technologyModel"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Technology.Edit")]
        public async Task<IActionResult> Edit(TechnologyModel technologyModel)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _technologyService.GetByIdAsync(technologyModel.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Technology"))
                });

            entity = technologyModel.ToEntity(entity);

            await _technologyService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Technology"))
            });
        }

        #endregion Method
    }
}