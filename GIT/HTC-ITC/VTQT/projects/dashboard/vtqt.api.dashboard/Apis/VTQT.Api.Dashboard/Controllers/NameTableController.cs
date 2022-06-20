using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Dashboard.Controllers
{
    [Route("name-table")]
    [ApiController]
  //  [XBaseApiAuthorize]
    [Produces("application/json")]
    //[AppApiController("Dashboard.Controllers.NameTable")]
    public class NameTableController : AdminApiController
    {

        #region Fields

        private readonly INameTableService _service;

        #endregion

        #region Ctor

        public NameTableController(
            INameTableService service
        )
        {
            _service = service;
        }
        [Route("index")]
        [HttpGet]
        //[AppApiAction("Dashboard.Controllers.NameTable.Index")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }
        /// <summary>
        /// API tạo danh sách các bảng có trong database cho người dùng chọn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.NameTable.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Create(NameTableExistModel model)
        {
            var entity = model.ToEntity();
            await _service.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.WareHouse"))
            });
        }

        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.NameTable.Edit")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Edit(NameTableExistModel model)
        {
            var entity = model.ToEntity();
            await _service.UpdateAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.WareHouse"))
            });
        }


        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("Dashboard.Controllers.NameTable.Deletes")]
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
                message = string.Format(T("Common.Notify.Deleted"), T("Common.WareHouse"))
            });
        }
        #endregion


        /// <summary>
        /// API lấy danh sách các bảng có trong database cho người dùng chọn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("get-list-combobox")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetListComboBox()
        {
            var entity = _service.GetMvcListItems();
            return Ok(new XBaseResult
            {
                data = entity
            });
        }



        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Get([FromQuery] NameTableSearchModel searchModel)
        {
            var searchContext = new NameTableSearchContext()
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };
            var models = new List<NameTableExistModel>();
            var entities = _service.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

    }
}
