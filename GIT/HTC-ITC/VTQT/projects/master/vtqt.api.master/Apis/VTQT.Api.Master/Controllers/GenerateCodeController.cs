using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Master;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("generate-code")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.GenerateCode")]
    public class GenerateCodeController : AdminApiController
    {
        #region Fields
        private readonly IAutoCodeService _autoCodeService;
        #endregion

        #region Ctor
        public GenerateCodeController(IAutoCodeService autoCodeService)
        {
            _autoCodeService = autoCodeService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy mã tự động theo tên bảng
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [AppApiAction("Master.AppActions.GenerateCodes.GenerateCode")]
        public async Task<IActionResult> GenerateCode(string tableName)
        {
            var code = await _autoCodeService.GenerateCode(tableName);

            return Ok(new XBaseResult
            {
                data = code
            });
        }
        #endregion

        #region Utilities

        #endregion
    }
}
