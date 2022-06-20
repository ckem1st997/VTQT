using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("vendor-billing")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.VendorBilling")]
    public class VendorBillingController : AdminApiController
    {
        #region Fields
        private readonly IVendorBillingService _vendorService;
        #endregion

        #region Ctor
        public VendorBillingController(IVendorBillingService vendorService)
        {
            _vendorService = vendorService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.VendorBillings.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu nhà cung cấp theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _vendorService.GetByIdAsync(id);

            if (entity == null || entity.IsBuyer)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Vendor"))
                });
            }

            var model = new VendorBillingModel
            {
                Id = entity.Id,
                Code = entity.ContractorCode,
                Name = entity.ContractorFullName
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách nhà cung cấp phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] VendorBillingSearchModel searchModel)
        {
            var searchContext = new VendorBillingSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<VendorBillingModel>();
            var entities = _vendorService.Get(searchContext);

            if (entities?.Count > 0)
            {
                foreach (var e in entities)
                {
                    var m = new VendorBillingModel
                    {
                        Id = e.Id,
                        Code = e.ContractorCode,
                        Name = e.ContractorFullName
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// Lấy danh sách nhà cung cấp cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = _vendorService.GetAvailable();
            var models = new List<VendorBillingModel>();

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = new VendorBillingModel
                    {
                        Id = e.Id,
                        Code = e.ContractorCode,
                        Name = e.ContractorFullName
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }
        #endregion
    }
}
