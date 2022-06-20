using System.Net;
using Microsoft.AspNetCore.Mvc;
using VTQT.Web.Framework.Controllers;

namespace VTQT.SharedMvc.Controllers
{
    public class MvcErrorController : XBaseMvcController
    {
        #region Http Errors

        public ViewResult BadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Title = T("Common.Errors.400");
            ViewBag.Message = T("Common.Errors.400.Message");

            return View();
        }

        public ViewResult Forbidden()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            ViewBag.Title = T("Common.Errors.403");
            ViewBag.Message = T("Common.Errors.403.Message");

            return View();
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            ViewBag.Title = T("Common.Errors.404");
            ViewBag.Message = T("Common.Errors.404.Message");

            return View();
        }

        public ViewResult InternalServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ViewBag.Title = T("Common.Errors.500");
            ViewBag.Message = T("Common.Errors.500.Message");

            return View();
        }

        #endregion

        public ViewResult SessionExpired()
        {
            ViewBag.Title = T("Common.Errors.SessionExpired");
            ViewBag.Message = T("Common.Errors.SessionExpired.Message");

            return View();
        }
    }
}
