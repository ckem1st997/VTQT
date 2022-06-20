using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Asset;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Asset.Controllers
{
    [Route("usage-status")]
    [ApiController]
    public class UsageStatusController : AdminApiController
    {
        #region Fields
        private readonly IUsageStatusService _usageStatusService;
        #endregion

        #region Ctor
        public UsageStatusController(IUsageStatusService usageStatusService)
        {
            _usageStatusService = usageStatusService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy dữ liệu trạng thái sử dụng tài sản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _usageStatusService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.UsageStatus"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List

        #endregion
    }
}
