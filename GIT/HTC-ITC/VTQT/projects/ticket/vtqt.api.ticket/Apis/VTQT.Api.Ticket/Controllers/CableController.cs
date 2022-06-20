using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("cable")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Cable")]
    public class CableController : AdminApiController
    {
        #region Fields

        private readonly ICableService _cableService;

        #endregion Fields

        #region Ctor

        public CableController(
            ICableService cableService)
        {
            _cableService = cableService;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Cable.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng cáp
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Cable.Create")]
        public IActionResult Create()
        {
            var model = new CableModel();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới cáp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Cable.Create")]
        public async Task<IActionResult> Create(CableModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();

            await _cableService.InsertAsync(entity);

            // Locales
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Cable"))
            });
        }

        /// <summary>
        /// Lấy dữ cáp cần update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Cable.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _cableService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Cable"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật cáp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Cable.Edit")]
        public async Task<IActionResult> Edit(CableModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _cableService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Cable"))
                });

            entity = model.ToEntity(entity);

            await _cableService.UpdateAsync(entity);

            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.Cable"))
            });
        }

        /// <summary>
        /// Lấy chi tiết cáp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Cable.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _cableService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Cable"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách cáp
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Cable.Deletes")]
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

            await _cableService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Cable"))
            });
        }

        /// <summary>
        /// Kích hoạt trạng thái cáp
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

            await _cableService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.Cable"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.Cable"))
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
        public async Task<IActionResult> Get([FromQuery] CableSearchModel searchModel)
        {
            var searchContext = new CableSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<CableModel>();
            var entities = _cableService.Get(searchContext);
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

        #endregion List

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
            var availableList = _cableService.GetAll(showHidden);

            List<CableModel> result = new List<CableModel>();

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

        #endregion Utilities
    }
}