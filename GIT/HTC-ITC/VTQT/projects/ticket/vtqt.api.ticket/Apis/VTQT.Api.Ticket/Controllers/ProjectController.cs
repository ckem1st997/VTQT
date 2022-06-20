using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Services.Master;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("project")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Project")]
    public class ProjectController : AdminApiController
    {
        #region Fields

        private readonly Services.Ticket.IProjectService _projectService;
        private readonly IAutoCodeService _autoCodeService;

        #endregion

        #region Ctor

        public ProjectController(
            Services.Ticket.IProjectService projectService,
            IAutoCodeService autoCodeService)
        {
            _projectService = projectService;
            _autoCodeService = autoCodeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Project.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }


        /// <summary>
        /// Khởi tạo đối tượng dự án
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Project.Create")]
        public IActionResult Create()
        {
            var model = new ProjectModel();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới dự án
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Project.Create")]
        public async Task<IActionResult> Create(ProjectModel model)
        {
            model.Code = await _autoCodeService.GenerateCode(nameof(Project));

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _projectService.ExistedAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.Project.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _projectService.InsertAsync(entity);

            // Locales
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Project"))
            });
        }

        /// <summary>
        /// Lấy dữ dự án cần update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Project.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _projectService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Project"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật dự án
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Project.Edit")]
        public async Task<IActionResult> Edit(ProjectModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _projectService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Project"))
                });

            entity = model.ToEntity(entity);

            await _projectService.UpdateAsync(entity);

            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Project"))
            });
        }

        /// <summary>
        /// Lấy chi tiết dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Project.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _projectService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Project"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách dự án
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Project.Deletes")]
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

            await _projectService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Project"))
            });
        }

        /// <summary>
        /// Kích hoạt trạng thái dự án
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Activated successfully</response>
        [Route("activates")]
        [HttpPost]
        [MapAppApiAction(nameof(Edit))]
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

            await _projectService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.Project"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.Project"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu dự án theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _projectService.GetByCodeAsync(code);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Project"))
                });
            }

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy dữ liệu danh sách dự án
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ProjectSearchModel searchModel)
        {
            var searchContext = new Services.Ticket.ProjectSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId
            };

            var models = new List<ProjectModel>();
            var entities = _projectService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }

        #endregion

        #region Utilities
        /// <summary>
        /// Lấy danh sách dự án cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _projectService.GetAll(showHidden);

            List<ProjectModel> result = new List<ProjectModel>();

            if (availableList?.Count > 0)
            {
                availableList.ToList().ForEach(x =>
                {
                    var model = x.ToModel();
                    result.Add(model);
                });
            }

            return Ok(new XBaseResult
            {
                data = result
            });
        }
        #endregion
    }
}