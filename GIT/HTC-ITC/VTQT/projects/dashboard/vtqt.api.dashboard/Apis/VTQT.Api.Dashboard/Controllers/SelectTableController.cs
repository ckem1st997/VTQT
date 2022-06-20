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
using VTQT.Core.Domain.Dashboard;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Dashboard.Controllers
{
    [Route("select-table")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
   //[AppApiController("Dashboard.Controllers.SelectTable")]
    public class SelectTableController : AdminApiController
    {

        #region Fields

        private readonly ISelectTableService _service;
        private readonly IExtensionGetValue _extensionGetValue;
        #endregion

        #region Ctor

        public SelectTableController(
            ISelectTableService service, IExtensionGetValue extensionGetValue)

        {
            _service = service;
            _extensionGetValue = extensionGetValue;
        }

        [Route("index")]
        [HttpGet]
       //[AppApiAction("Dashboard.Controllers.SelectTable.Index")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }


        /// <summary>
        /// API tạo các cột cần tìm kiếm khi người dùng chọn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("create")]
        [HttpPost]
       //[AppApiAction("Dashboard.Controllers.SelectTable.Create")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Create(IEnumerable<SelectTableModel> model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.SelectTable"))
                });
            var listEntity = new List<SelectTable>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                if (string.IsNullOrEmpty(tem.Id))
                    tem.Id = Guid.NewGuid().ToString();
                listEntity.Add(tem);
            }

            var check = await _service.InsertAsync(listEntity);
            // Locales

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Added"), T("Common.SelectTable"))
            });
        }

        [Route("edit")]
        [HttpPost]
        [DisableRequestSizeLimit]
       //[AppApiAction("Dashboard.Controllers.SelectTable.Edit")]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> Edit(IEnumerable<SelectTableModel> model)
        {
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH "))
                });
            var listEntity = new List<SelectTable>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                listEntity.Add(tem);
            }

            var check = await _service.UpdateAsync(listEntity);
            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Updated"), T("Common.FTTH"))
            });
        }



        [Route("deletes")]
        [HttpPost]
       //[AppApiAction("Dashboard.Controllers.SelectTable.Deletes")]
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

            var check = await _service.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Deleted"), T("Common.SelectTable"))
            });
        }

        #endregion




        /// <summary>
        /// API lấy danh sách các cột cần tìm kiếm khi người dùng chọn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("get-select-table")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetSelect(string NameTable)
        {
            if (string.IsNullOrEmpty(NameTable))
            {
                return Ok(new XBaseResult
                {
                    success = true,
                    data = null
                });
            }

            var models = new List<SelectTableModel>();
            var entities = await _extensionGetValue.GetColumnName(NameTable);
            foreach (var e in entities)
            {
                if (!e.Field.Equals("Id"))
                {
                    var m = new SelectTableModel
                    {
                        TableShow = NameTable,
                        TextShow = e.Field,
                        ValueShow = "`" + e.Field + "`"
                    };
                    models.Add(m);
                }

            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }


        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]


        public async Task<IActionResult> Get([FromQuery] SelectTableSearchModel searchModel)
        {
            var searchContext = new SelectTableSearchContext()
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
            };
            var models = new List<SelectTableModel>();
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