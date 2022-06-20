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
    [Route("priority")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Priority")]
    public class PriorityController : AdminApiController
    {
        #region Fields

        private readonly IPriorityService _priorityService;
        private readonly IAutoCodeService _autoCodeService;

        #endregion Fields

        #region Ctor

        public PriorityController(
            IPriorityService priorityService,
            IAutoCodeService autoCodeService)
        {
            _priorityService = priorityService;
            _autoCodeService = autoCodeService;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Priority.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng phân cấp sự cố
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Priority.Create")]
        public IActionResult Create()
        {
            var model = new PriorityModel();

            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới phân cấp sự cố
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Priority.Create")]
        public async Task<IActionResult> Create(PriorityModel model)
        {
            model.Code = await _autoCodeService.GenerateCode(nameof(Priority));

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _priorityService.ExistedAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.Priority.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _priorityService.InsertAsync(entity);

            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Priority"))
            });
        }

        /// <summary>
        /// Lấy dữ phân cấp sự cố cần update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Priority.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _priorityService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Priority"))
                });

            var model = entity.ToModel();

            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật phân cấp sự cố
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Priority.Edit")]
        public async Task<IActionResult> Edit(PriorityModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _priorityService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Priority"))
                });

            entity = model.ToEntity(entity);

            await _priorityService.UpdateAsync(entity);

            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Priority"))
            });
        }

        /// <summary>
        /// Lấy chi tiết phân cấp sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Priority.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _priorityService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Priority"))
                });

            var model = entity.ToModel();

            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Lấy chi tiết phân cấp sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details-name")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public IActionResult DetailNames(string id)
        {
            var entity = _priorityService.GetByIdName(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Priority"))
                });

            var model = entity.ToModel();

            // Locales

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách phân cấp sự cố
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Priority.Deletes")]
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

            await _priorityService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Priority"))
            });
        }

        /// <summary>
        /// Kích hoạt trạng thái phân cấp sự cố
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

            await _priorityService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.Priority"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.Priority"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu phân cấp sự cố theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _priorityService.GetByCodeAsync(code);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Priority"))
                });
            }

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        #endregion

        #region List

        /// <summary>
        /// Lấy danh sách priority cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden, string projectId)
        {
            var availableList = _priorityService.GetAll(showHidden, projectId);

            List<PriorityModel> result = new List<PriorityModel>();

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

        /// <summary>
        /// Lấy dữ liệu danh sách phân cấp sự cố
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] PrioritySearchModel searchModel)
        {
            var searchContext = new PrioritySearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<PriorityModel>();
            var entities = _priorityService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Project = e.Project == null
                   ? new ProjectModel
                   {
                       Code = "",
                       Name = ""
                   }
                   : new ProjectModel
                   {
                       Id = e.Project.Id,
                       Code = e.Project.Code,
                       Name = e.Project.Name
                   };

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

        #endregion
    }
}