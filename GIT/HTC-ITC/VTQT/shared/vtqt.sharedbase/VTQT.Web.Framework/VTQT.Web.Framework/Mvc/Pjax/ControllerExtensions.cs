using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VTQT.Web.Framework.Mvc.Pjax
{
    public static class ControllerExtensions
    {
        public static void PjaxFullLoad(this Controller controller)
        {
            controller.ControllerContext.HttpContext.Session.SetString(Constants.PjaxVersion, Guid.NewGuid().ToString("N"));
        }
    }
}
