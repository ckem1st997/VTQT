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
    [Route("customer")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.Customer")]
    public class CustomerController : AdminApiController
    {
        #region Fields
        private readonly ICustomerService _customerService;
        #endregion

        #region Ctor
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Customers.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu khách hàng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _customerService.GetByIdAsync(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Customer"))
                });
            }

            var model = new CustomerModel
            {
                Id = entity.Id,
                Code = entity.CustomerCode,
                Name = entity.FullName
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy dữ liệu khách hàng theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _customerService.GetByCodeAsync(code);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Customer"))
                });
            }

            var model = new CustomerModel
            {
                Id = entity.Id,
                Code = entity.CustomerCode,
                Name = entity.FullName,
                //Address = entity.Address
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
        /// Lấy danh sách khách hàng phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] CustomerSearchModel searchModel)
        {
            var searchContext = new CustomerSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<CustomerModel>();
            var entities = _customerService.Get(searchContext);

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = new CustomerModel
                    {
                        Id = e.Id,
                        Code = e.CustomerCode,
                        Name = e.FullName
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
        /// Lấy danh sách khách hàng cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = _customerService.GetAvailable();
            var models = new List<CustomerModel>();

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = new CustomerModel
                    {
                        Id = e.Id,
                        Code = e.CustomerCode,
                        Name = e.FullName
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
