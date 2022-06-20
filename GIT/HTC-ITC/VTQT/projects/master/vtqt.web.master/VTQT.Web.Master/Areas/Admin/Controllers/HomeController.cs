using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Master.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class HomeController : AdminMvcController
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

        public IActionResult Index()
        {
            return View();
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
