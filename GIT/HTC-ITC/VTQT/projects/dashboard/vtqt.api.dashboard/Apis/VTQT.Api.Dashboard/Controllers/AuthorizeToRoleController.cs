using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nest;
using VTQT.Api.Dashboard.Helper;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;


namespace VTQT.Api.Dashboard.Controllers
{
    [Route("authorize-to-role")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Dashboard.Controllers.AuthorizeToRole")]
    public class AuthorizeToRoleController : AdminApiController
    {
        #region Fields

        private readonly IAuthorizeToRoleService _service;
        private readonly IWorkContext _workContext;
        private readonly IUserModelHelper _userModelHelper;
        private readonly ITypeValueService _typeValueService;
        private readonly IStorageValueService _storageValueService;

        #endregion

        #region Ctor

        public AuthorizeToRoleController(
            IAuthorizeToRoleService service,
            IWorkContext workContext,
            IUserModelHelper userModelHelper,
            ITypeValueService typeValueService,
            IStorageValueService storageValueService
        )
        {
            _service = service;
            _workContext = workContext;
            _userModelHelper = userModelHelper;
            _typeValueService = typeValueService;
            _storageValueService = storageValueService;
        }

        /// <summary>
        /// Lấy chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Dashboard.Controllers.AuthorizeToRole.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuthorizeToRole"))
                });
            return Ok(new XBaseResult
            {
                data = entity
            });
        }


        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Dashboard.Controllers.AuthorizeToRole.Create")]
        public async Task<IActionResult> Create(string idTypeValue)
        {
            var model = new AuthorizeToRoleModel();
            model.DelegatorId = _workContext.UserId;
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableTypeValues = _typeValueService.GetMvcListItems();
            if (!string.IsNullOrEmpty(idTypeValue))
            {
                model.TypeValueId = idTypeValue;
                model.AvailableFiles = _storageValueService.GetMvcListItems(idTypeValue);

            }
            return Ok(new XBaseResult
            {
                data = model
            });
        }


        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Dashboard.Controllers.AuthorizeToRole.Create")]
        public async Task<IActionResult> Create(IEnumerable<AuthorizeToRoleModel> model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuthorizeToRole"))
                });
            var listEntity = new List<AuthorizeToRole>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                tem.Name = _workContext.UserName;
                tem.CreateDate = DateTime.UtcNow;
                listEntity.Add(tem);
            }

            var check = await _service.InsertAsync(listEntity);
            // Locales

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.AuthorizeToRole.Added"))
            });
        }



        /// <summary>
        /// Trả về danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Dashboard.Controllers.AuthorizeToRole.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<AuthorizeToRoleModel> model)
        {
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuthorizeToRole"))
                });
            var listEntity = new List<AuthorizeToRole>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                listEntity.Add(tem);
            }

            var check = await _service.UpdateAsync(listEntity);
            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Updated"), T("Common.AuthorizeToRole"))
            });
        }

        /// <summary>
        /// Xóa danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Dashboard.Controllers.AuthorizeToRole.Deletes")]
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

            var check = await _service.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Deleted"), T("Common.AuthorizeToRole"))
            });
        }
        [Route("index")]
        [HttpGet]
        [AppApiAction("Dashboard.Controllers.AuthorizeToRole.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        #endregion

        /// <summary>
        /// Lấy danh sách  kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-query")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetQuery(AuthorizeToRoleSearchModel searchModel)
        {
            var searchContext = new AuthorizeToRoleSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };
            var models = new List<AuthorizeToRoleModel>();
            var entities = await _service.GetAllQuery(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = models.Count
            });
        }

        [Route("get-list")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult GetList(string idTypeValue)
        {
            var idDelegatorId = _workContext.UserId;

            if (string.IsNullOrEmpty(idTypeValue) || string.IsNullOrEmpty(idDelegatorId))
            {

                return Ok(new XBaseResult
                {
                    success = false,
                    data = null,
                    totalCount = 0
                });
            }

            var models = new List<AuthorizeToRoleModel>();
            var entities = _service.GetAll(idTypeValue, idDelegatorId);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = models.Count
            });
        }


    }
}