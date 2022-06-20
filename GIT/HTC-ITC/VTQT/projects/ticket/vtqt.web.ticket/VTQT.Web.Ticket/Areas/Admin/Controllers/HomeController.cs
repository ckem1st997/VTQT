using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class HomeController : AdminMvcController
    {
        #region Ctor

        public HomeController(
            )
        {
        }

        #endregion Ctor

        #region Methods

        public async Task<IActionResult> Index()
        {
            return View();
        }

        #endregion Methods
    }
}