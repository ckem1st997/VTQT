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
    [Route("network-link")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.NetworkLink")]
    public class NetworkLinkController : AdminApiController
    {
        #region Fields

        private readonly INetworkLinkService _networkLinkService;
        private readonly IAutoCodeService _autoCodeService;

        #endregion

        #region Ctor

        public NetworkLinkController(IAutoCodeService autoCodeService,
            INetworkLinkService networkLinkService)
        {
            _networkLinkService = networkLinkService;
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
        [AppApiAction("Ticket.AppActions.NetworkLink.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng link mạng
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLink.Create")]
        public IActionResult Create()
        {
            var model = new NetworkLinkModel();
            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới link mạng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.NetworkLink.Create")]
        public async Task<IActionResult> Create(NetworkLinkModel model)
        {
            model.Code = await _autoCodeService.GenerateCode(nameof(NetworkLink));
            if (!ModelState.IsValid)
                return InvalidModelResult();
            if (await _networkLinkService.ExistedAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.Project.Fields.Code"))
                });
            var entity = model.ToEntity();
            entity.Code = model.Code;

            await _networkLinkService.InsertAsync(entity);

            // Locales
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.NetworkLink"))
            });
        }

        /// <summary>
        /// Lấy dữ link mạng cần update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLink.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _networkLinkService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLink"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật link mạng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.NetworkLink.Edit")]
        public async Task<IActionResult> Edit(NetworkLinkModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _networkLinkService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLink"))
                });

            entity = model.ToEntity(entity);

            await _networkLinkService.UpdateAsync(entity);

            // Locales

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.NetworkLink"))
            });
        }

        /// <summary>
        /// Lấy chi tiết link mạng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.NetworkLink.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _networkLinkService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.NetworkLink"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách link mạng
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.NetworkLink.Deletes")]
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

            await _networkLinkService.DeletesAsync(ids);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.NetworkLink"))
            });
        }

        /// <summary>
        /// Kích hoạt trạng thái link mạng
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

            await _networkLinkService.ActivatesAsync(model.Ids, model.Active);

            return Ok(new XBaseResult
            {
                message = model.Active
                    ? string.Format(T("Common.Notify.Activated"), T("Common.NetworkLink"))
                    : string.Format(T("Common.Notify.Deactivated"), T("Common.NetworkLink"))
            });
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

            var entity = await _networkLinkService.GetByIdAsync(id);
            var model = new NetworkLinkModel();

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

        /// <summary>
        /// Lấy dữ liệu danh sách link mạng
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] NetworkLinkSearchModel searchModel)
        {
            var searchContext = new NetworkLinkSearchContext
            {
                Keywords = searchModel.Keywords,
                Status = (int)searchModel.Status,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };

            var models = new List<NetworkLinkModel>();
            var entities = _networkLinkService.Get(searchContext);
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
        /// Lấy danh sách mạng cho dropdown
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailableList(bool showHidden = false)
        {
            var availableList = _networkLinkService.GetAll(showHidden);

            List<NetworkLinkModel> result = new List<NetworkLinkModel>();

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