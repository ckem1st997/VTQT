using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VTQT.Web.Framework.Controllers;

namespace VTQT.SharedMvc.Controllers
{
    public class MvcAccountController : XBaseMvcController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MvcAccountController(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult SignOut()
        {
            //_httpContextAccessor.HttpContext.Session.Clear();

            return SignOut(
                "Cookies",
                "oidc");
        }
    }
}
