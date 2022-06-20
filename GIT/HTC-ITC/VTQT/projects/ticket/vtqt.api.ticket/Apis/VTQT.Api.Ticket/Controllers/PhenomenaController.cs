using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("phenomena")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Phenomena")]
    public class PhenomenaController : AdminApiController
    {
        #region Fields

        private readonly IPhenomenaService _phenomenaService;

        #endregion Fields

        #region Ctor

        public PhenomenaController(
            IPhenomenaService phenomenaService)
        {
            _phenomenaService = phenomenaService;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Phenomena.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }
        #endregion Methods

        #region List



        #endregion List

        #region Utilities

        /// <summary>
        /// Lấy danh sách dự án cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _phenomenaService.GetAll(showHidden);

            List<PhenomenaModel> result = new List<PhenomenaModel>();

            if (availableList?.Count > 0)
            {
                availableList.ToList().ForEach(x =>
                {
                    var model = x.ToModel();
                    result.Add(model);
                });
            }

            return Ok(new XBaseResult
            {
                data = result
            });
        }

        #endregion Utilities
    }
}
