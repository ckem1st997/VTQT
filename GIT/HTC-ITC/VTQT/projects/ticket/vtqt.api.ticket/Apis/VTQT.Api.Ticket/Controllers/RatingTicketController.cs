using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("rating-ticket")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.RatingTicket")]
    public class RatingTicketController : AdminApiController
    {
        #region Fields
        private readonly IRatingTicketService _ratingTicketService;
        #endregion

        #region Ctor
        public RatingTicketController(
            IRatingTicketService ratingTicketService)
        {
            _ratingTicketService = ratingTicketService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.RatingTicket.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng RatingTicket
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.RatingTicket.Create")]
        public IActionResult Create()
        {
            var model = new CRModel
            {
            };
            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách RatingTicket
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] RatingTicketSearchModel searchModel)
        {
            var searchContext = new RatingTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                ProjectId = searchModel.ProjectId,
                StartDate = searchModel.StartDate,
                FinishDate = searchModel.FinishDate
            };
            if (!string.IsNullOrEmpty(searchModel.StrStartDate))
            {
                searchContext.StartDate = DateTime.ParseExact(searchModel.StrStartDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrFinishDate))
            {
                searchContext.FinishDate = DateTime.ParseExact(searchModel.StrFinishDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }


            var models = await _ratingTicketService.Get(searchContext);

            return Ok(new XBaseResult
            {
                data = models,
                success = true,
                totalCount = models.TotalCount
            });
        }

        #endregion

        #region Utilities

        #endregion
    }
}
