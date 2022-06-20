using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Asset;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Asset.Controllers
{
    [Route("print")]
    [ApiController]
    [Produces("application/json")]
    public class PrintController : AdminApiController
    {
        #region Ctor

        private readonly IPrint _print;

        public PrintController(IPrint print)
        { _print = print; }

        #endregion Ctor

        #region Method

        [Route("get-by-id-to-audit")]
        [HttpGet]
        public async Task<IActionResult> GetByToWordAudit(string id)
        {
            var entity = await _print.GetByIdToWordAuditAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Audit"))
                });

            var model = entity.ToModel();
            model.VoucherDate = entity.VoucherDate.ToLocalTime();
            model.CreatedBy = entity.CreatedBy;
            model.AssetType = entity.AssetType;
            model.ModifiedBy = entity.ModifiedBy;
            model.AuditLocation = entity.AuditLocation;
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();
            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        #endregion Method
    }
}