using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrapeCity.Documents.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Dashboard.Areas.Admin.Controllers
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

        public async Task<IActionResult> Index()
        {
            //Workbook workbook = new Workbook();
            //workbook.Worksheets[0].Range["A1"].Value = "Hello Word!";
            //workbook.Save("HelloWord.xlsx");
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
