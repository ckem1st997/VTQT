using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("area")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Area")]
    public class AreaController : AdminApiController
    {
        #region Fields

        private readonly IAreaService _areaService;

        #endregion

        #region Ctor

        public AreaController(
            IAreaService areaService)
        {
            _areaService = areaService;
        }

        #endregion

        #region Utilities

        #endregion
    }
}
