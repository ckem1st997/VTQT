using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("warehouse-request")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("WareHouse.Controllers.WarehouseRequest")]
    public class WarehouseRequestController : AdminApiController
    {
        #region Fields



        #endregion

        #region Ctor

        public WarehouseRequestController(
            )
        {
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.WarehouseRequests.Home.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        #endregion

        #region Lists



        #endregion

        #region Helpers



        #endregion

        #region Utilities



        #endregion
    }
}
