using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Dashboard.Controllers
{
    [Route("home")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Dashboard.Controllers.Home")]
    public class HomeController : AdminApiController
    {
        #region Fields



        #endregion

        #region Ctor

        public HomeController(
            )
        {
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Dashboard.AppActions.Home.Index")]
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
