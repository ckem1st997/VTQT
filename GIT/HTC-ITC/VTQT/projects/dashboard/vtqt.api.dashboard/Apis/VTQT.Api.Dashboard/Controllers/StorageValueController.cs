using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nest;
using VTQT.Api.Dashboard.Helper;
using VTQT.Core;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Dashboard.Controllers
{
    [Route("storage-value")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    //[AppApiController("Dashboard.Controllers.StorageValue")]
    public class StorageValueController : AdminApiController
    {
        #region Fields

        private readonly IStorageValueService _service;
        private readonly ITypeValueModelHelper _wareHouseModelHelper;
        private readonly IWorkContext _workContext;
        private readonly IUserModelHelper _userModelHelper;
        private readonly IAuthorizeToRoleService _dynamicService;
        private readonly INameTableService _nameTableService;
        private readonly IDashboardUserService _iWareHouseUserService;

        #endregion

        #region Ctor

        public StorageValueController(
            IStorageValueService service,
            ITypeValueModelHelper wareHouseModelHelper,
            IWorkContext workContext,
            IUserModelHelper userModelHelper,
            IAuthorizeToRoleService dynamicService,
            INameTableService nameTableService,
            IDashboardUserService iWareHouseUserService
        )
        {
            _service = service;
            _wareHouseModelHelper = wareHouseModelHelper;
            _workContext = workContext;
            _userModelHelper = userModelHelper;
            _dynamicService = dynamicService;
            _nameTableService = nameTableService;
            _iWareHouseUserService = iWareHouseUserService;
        }


        [Route("index")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Index")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }


        /// <summary>
        /// Lấy chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Details")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.StorageValue"))
                });
            var list = await _wareHouseModelHelper.GetTypeValueDropdownTreeAsync(showList: true);
            var model = entity.ToModel();
            model.AvailableTypeValue = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        [Route("edits")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Edits")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Edits(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.StorageValue"))
                });
            var check = await _iWareHouseUserService.ExistAsync(entity.TypeValueId, _workContext.UserId);
            if (!check)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotAuthorize"))
                });
            var list = await _wareHouseModelHelper.GetTypeValueDropdownTreeAsync(showList: true);
            var model = entity.ToModel();
            model.AvailableTypeValue = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableNameTable = _nameTableService.GetMvcListItems();
            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }

        [Route("run-query")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunQueryImport(SelectListItem mode)
        {
            if (string.IsNullOrEmpty(mode.Value))
                return Ok(new XBaseResult
                    { success = false, message = "Có lỗi xảy ra ngoài ý muốn, xin vui lòng thực hiện lại !" });
            var res = await _service.RunQueryAsync(mode.Value);
            return Ok(new XBaseResult { success = true, data = res });
        }


        [Route("run-query-object")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunQueryToObjectAsync(SelectListItem mode)
        {
            if (string.IsNullOrEmpty(mode.Value))
                return Ok(new XBaseResult
                    { success = false, message = "Có lỗi xảy ra ngoài ý muốn, xin vui lòng thực hiện lại !" });
            var res = await _service.RunQueryToObjectAsync(mode.Value);
            return Ok(new XBaseResult { success = true, data = res });
        }

        [Route("count-table")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> CountTable(SelectListItem mode)
        {
            if (string.IsNullOrEmpty(mode.Value))
                return Ok(new XBaseResult
                    { success = false, message = "Có lỗi xảy ra ngoài ý muốn, xin vui lòng thực hiện lại !" });
            var res = await _service.RunQueryCounttAsync(mode.Value);
            return Ok(new XBaseResult { success = true, data = res });
        }
        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create()
        {
            var model = new StorageValueModel();
            var list = await _wareHouseModelHelper.GetTypeValueDropdownTreeAsync(showList: true);
            model.AvailableTypeValue = list
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableNameTable = _nameTableService.GetMvcListItems();
            model.ActiveGetAllData = false;
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
        [HttpPost, DisableRequestSizeLimit]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(StorageValueModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            if (string.IsNullOrEmpty(model.FileName))
                return Ok(new XBaseResult
                {
                    message = string.Format(T("Common.Validators.InputFields.Required"),
                        T("Common.Fields.StorageValueModel.SaveValue"))
                });
            if (string.IsNullOrEmpty(model.Id))
                model.Id = Guid.NewGuid().ToString();
            var entity = model.ToEntity();
            entity.CreatedBy = _workContext.UserId;
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedBy = _workContext.UserId;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedByName = _workContext.User.FullName + " - " + _workContext.UserName;
            entity.VoucherByName = _workContext.User.FullName + " - " + _workContext.UserName;
            entity.VoucherDate = DateTime.UtcNow;
            await _service.InsertAsync(entity);
            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.StorageValue"))
            });
        }

        /// <summary>
        /// Trả về danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Edit")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Edit(StorageValueModel model)
        {
            var entity = await _service.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.StorageValue"))
                });
            var check = await _iWareHouseUserService.ExistAsync(entity.TypeValueId, _workContext.UserId);
            if (!check)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotAuthorize"))
                });
            if (model.DataSave == null)
            {
                model.DataSave = entity.DataSave;
            }

            model.VoucherByName = entity.VoucherByName;
            var vdOld = entity.VoucherDate;
            entity = model.ToEntity(entity);
            entity.ModifiedBy = _workContext.UserId;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedByName = _workContext.User.FullName + " - " + _workContext.UserName;
            entity.VoucherDate = vdOld;
            await _service.UpdateAsync(entity);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.StorageValue"))
            });
        }

        /// <summary>
        /// Xóa danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.StorageValue.Deletes")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
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

            await _service.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.StorageValue"))
            });
        }

        #endregion

        /// <summary>
        /// Lấy danh sách  kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        //  [MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] StorageValueSearchModel searchModel)
        {
            var searchContext = new StorageValueSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                TypeValueId = searchModel.TypeValueId
            };

            if (searchModel.FromDate.HasValue)
            {
                searchContext.FromDate = searchModel.FromDate;
            }

            if (searchModel.ToDate.HasValue)
            {
                searchContext.ToDate = searchModel.ToDate;
            }

            var models = new List<StorageValueModel>();
            var entities = await _service.GetAsync(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.CreatedBy = e.CreatedBy;
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.VoucherDate = e.VoucherDate.ToLocalTime();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = new PagedList<StorageValueModel>(models, searchContext.PageIndex, searchContext.PageSize),
                totalCount = entities.TotalCount
            });
        }


        [Route("get-mvc-list")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        //      [MapAppApiAction(nameof(Index))]
        public IActionResult GetMvcList(string idTypeValue)
        {
            if (string.IsNullOrEmpty(idTypeValue))
            {
                return Ok(new XBaseResult
                {
                    success = true,
                    data = null
                });
            }

            var list = _service.GetMvcListItems(idTypeValue);
            return Ok(new XBaseResult
            {
                success = true,
                data = list
            });
        }
    }
}