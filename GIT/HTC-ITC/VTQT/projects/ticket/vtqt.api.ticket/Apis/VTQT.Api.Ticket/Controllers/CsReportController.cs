using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("csReport")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.CsReport")]
    public class CsReportController : AdminApiController
    {
        #region Fields

        private readonly ICsReportService _csReportService;

        #endregion Fields

        #region Ctor

        public CsReportController(ICsReportService csReportService)
        {
            _csReportService = csReportService;
        }

        #endregion Ctor

        #region Method

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.CsReport.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.CsReport.Create")]
        public IActionResult Create()
        {
            var model = new CsReportModel();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.CsReport.Create")]
        public async Task<IActionResult> Create(CsReportModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _csReportService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.CsReport"))
            });
        }

        /// <summary>
        /// Lấy dữ CsReport
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.CsReport.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _csReportService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.CsReport"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật CsReport
        /// </summary>
        /// <param name="csReportModel"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.CsReport.Edit")]
        public async Task<IActionResult> Edit(CsReportModel csReportModel)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _csReportService.GetByIdAsync(csReportModel.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.CsReport"))
                });

            entity = csReportModel.ToEntity(entity);

            await _csReportService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.CsReport"))
            });
        }

        #endregion Method
    }
}