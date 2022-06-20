using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("report")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.Report")]
    public class ReportController : AdminApiController
    {
        #region Fields



        #endregion

        #region Ctor

        public ReportController(
            )
        {
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.Report.Index")]
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
