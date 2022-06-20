using Microsoft.AspNetCore.Mvc;
using VTQT.Web.Framework.Mvc.Pjax;

namespace VTQT.Web.Framework.Controllers
{
    [Area(AreaNames.Admin)]
    //[AdminValidateIpAddress]
    [Pjax]
    //[AutoValidateAntiforgeryToken]
    public abstract class AdminMvcController : XBaseMvcController, IPjax
    {
        public bool IsPjaxRequest { get; set; }
        public string PjaxVersion { get; set; }
    }
}
