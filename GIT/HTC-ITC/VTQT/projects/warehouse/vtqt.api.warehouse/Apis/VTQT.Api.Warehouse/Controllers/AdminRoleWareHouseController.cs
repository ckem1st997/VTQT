using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("admin-role-wareHouse")]
    [ApiController]
    public class AdminRoleWareHouseController : AdminApiController
    {
        #region Fields

        private readonly IAdminRoleWareHouseService _adminRoleWareHouseService;

        #endregion Fields

        #region Ctor

        public AdminRoleWareHouseController(
            IAdminRoleWareHouseService adminRoleWareHouseService)
        {
            _adminRoleWareHouseService = adminRoleWareHouseService;
        }

        #endregion Ctor

        #region Methods

        [Route("details")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _adminRoleWareHouseService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AdminRoleWareHouse"))
                });

            var model = entity.ToModel();

            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminRoleWareHouseModel();

            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(AdminRoleWareHouseModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _adminRoleWareHouseService.InsertAsync(entity);

            // Locales

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.AdminRoleWareHouse"))
            });
        }

        [Route("deletes")]
        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _adminRoleWareHouseService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.AdminRoleWareHouse"))
            });
        }

        [Route("activates")]
        [HttpPost]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            await _adminRoleWareHouseService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.AdminRoleWareHouse"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.AdminRoleWareHouse"))
            });
        }

        #endregion Methods

        #region Lists

        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AdminRoleWareHouseSearchModel searchModel)
        {
            var searchContext = new AdminRoleWareHouseContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<AdminRoleWareHouseModel>();
            var entities = _adminRoleWareHouseService.Get(searchContext);

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        [Route("get-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _adminRoleWareHouseService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AdminRoleWareHouse"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        #endregion Lists
    }
}