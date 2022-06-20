using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("organization-unit")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.OrganizationUnit")]
    public class OrganizationUnitController : AdminApiController
    {
        #region Fields
        private readonly IOrganizationUnitService _organizationUnitService;
        #endregion

        #region Ctor
        public OrganizationUnitController(IOrganizationUnitService organizationUnitService)
        {
            _organizationUnitService = organizationUnitService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.OrganizationUnit.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var entity = await _organizationUnitService.GetByIdAsync(id);
            var model = new OrganizationUnitModel();

            if (entity != null)
            {
                model = entity.ToModel();
            }

            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }
        #endregion

        #region List
        [Route("get-list-organization")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetProcessingUnitByManagementUnitId(string unitId)
        {
            var results = _organizationUnitService.GetProcessingUnitByManagementUnitId(unitId);

            return Ok(new XBaseResult
            {
                data = results
            });
        }
        #endregion
    }
}