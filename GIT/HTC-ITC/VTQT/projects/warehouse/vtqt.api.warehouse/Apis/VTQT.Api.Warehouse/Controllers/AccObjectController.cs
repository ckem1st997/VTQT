using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Services.Warehouse;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("acc-object")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
  //  [AppApiController("WareHouse.Controllers.Audit")]
    public class AccObjectController : AdminApiController
    {
        #region Fields
        private readonly IAccObjectService _service;

        #endregion

        #region Ctor
        public AccObjectController(
            IAccObjectService service

        )
        {
         _service = service;
        }
        #endregion
        
        
        [Route("get-select")]
        [HttpGet]
       // [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelect(bool active=false)
        {
            var entity = _service.GetMvcListItems(active);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouse"))
                });
            return Ok(new XBaseResult
            {
                success = true,
                data = entity
            });
        }

    }
}