using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("print")]
    [ApiController]
    public class PrintController : AdminApiController
    {
        private readonly IPrint _print;

        public PrintController(IPrint print)
        { _print = print; }

        [Route("get-by-id-to-cr")]
        [HttpGet]
        public async Task<IActionResult> GetByToWordCR(string id)
        {
            var entity = await _print.GetByIdToWordCRAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.CR"))
                });

            var model = entity.ToModel();
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();
            model.StartDate = entity.StartDate.ToLocalTime();
            model.FinishDate = entity.FinishDate.ToLocalTime();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        [Route("get-by-id-to-ticket")]
        [HttpGet]
        public async Task<IActionResult> GetByToWordTicket(string id)
        {
            var entity = await _print.GetByIdToWordTicketAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Ticket"))
                });

            var model = entity.ToModel();
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();
            model.StartDate = entity.StartDate.ToLocalTime();
            model.FinishDate = entity.FinishDate.ToLocalTime();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
    }
}