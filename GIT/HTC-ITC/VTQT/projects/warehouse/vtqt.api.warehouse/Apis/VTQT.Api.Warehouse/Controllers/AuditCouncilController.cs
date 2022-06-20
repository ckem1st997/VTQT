using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Localization;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("audit-council")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.AuditCouncil")]
    public class AuditCouncilController : AdminApiController
    {
        #region Fields
        private readonly IAuditCouncilService _auditCouncilService;
        private readonly IUserModelHelper _userModelHelper;
        #endregion

        #region Ctor
        public AuditCouncilController(IAuditCouncilService auditCouncilService,
                                      IUserModelHelper userModelHelper)
        {
            _auditCouncilService = auditCouncilService;
            _userModelHelper = userModelHelper;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.AuditCouncils.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// lấy chi tiết danh sách hội đồng kiểm kê
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create()
        {
            var model = new AuditCouncilModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// tạo mới chi tiết danh sách hội đồng kiểm kê
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(AuditCouncilModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _auditCouncilService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.AuditCouncil"))
            });
        }

        /// <summary>
        /// Trả về chi tiết danh sách hội đồng kiểm kê
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _auditCouncilService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuditCouncil"))
                });

            var model = entity.ToModel();

            PrepareDetailModel(model);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật chi tiết danh sách hội đồng kiểm kê
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Edit(AuditCouncilModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _auditCouncilService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.AuditCouncil"))
                });

            entity = model.ToEntity(entity);

            await _auditCouncilService.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.AuditCouncil"))
            });
        }

        /// <summary>
        /// Xóa chi tiết danh sách hội đồng kiểm kê theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
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

            await _auditCouncilService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.AuditCouncil"))
            });
        }
        #endregion

        #region Lists
        /// <summary>
        /// Lấy chi tiết danh sách hội đồng kiểm kê phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] AuditCouncilSearchModel searchModel)
        {
            var searchContext = new AuditCouncilSearchContext
            {
                AuditId = searchModel.AuditId
            };

            var models = new List<AuditCouncilModel>();
            var entities = _auditCouncilService.GetByAuditCouncilId(searchContext);
            var userModels = _userModelHelper.GetAll(true);

            foreach (var e in entities)
            {
                var m = e.ToModel();
                if (!string.IsNullOrWhiteSpace(m.EmployeeId))
                    m.EmployeeName = userModels.FirstOrDefault(w => w.Id == m.EmployeeId)?.FullName;

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        #endregion

        #region Utilities
        private void PrepareDetailModel(AuditCouncilModel model)
        {
            var userModels = _userModelHelper.GetAll(true);
           
            if (!string.IsNullOrWhiteSpace(model.EmployeeId))
                model.EmployeeName = userModels.FirstOrDefault(w => w.Id == model.EmployeeId)?.FullName;

            model.AvailableUsers = userModels
                .Where(w => w.Active || w.Id.Equals(model.EmployeeId, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FullName} ({s.UserName})"
                }).ToList();
        }

        #endregion
    }
}
