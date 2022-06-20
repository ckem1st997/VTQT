using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("language")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Master.Controllers.Language")]
    public class LanguageController : AdminApiController
    {
        #region Fields



        #endregion

        #region Ctor

        public LanguageController(
            )
        {

        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Languages.Index")]
        public async Task<IActionResult> Index()
        {
            var searchModel = new LanguageSearchModel();

            return Ok(new XBaseResult
            {
                data = searchModel
            });
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
