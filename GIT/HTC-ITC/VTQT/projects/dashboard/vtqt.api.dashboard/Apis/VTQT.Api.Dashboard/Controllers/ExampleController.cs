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

    [Route("example")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
    //   [AppApiController("Dashboard.Controllers.StorageValue")]
    public class ExampleController : AdminApiController
    {
        #region Fields

        private readonly IExcampleService _service;
        private readonly ITypeValueModelHelper _wareHouseModelHelper;
        private readonly IWorkContext _workContext;
        private readonly IUserModelHelper _userModelHelper;

        #endregion

        #region Ctor

        public ExampleController(
            IExcampleService service,
            ITypeValueModelHelper wareHouseModelHelper,
            IWorkContext workContext,
            IUserModelHelper userModelHelper
        )
        {
            _service = service;
            _wareHouseModelHelper = wareHouseModelHelper;
            _workContext = workContext;
            _userModelHelper = userModelHelper;
        }

        /// <summary>
        /// Lấy chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.MasterVTCNTT"))
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
        // [AppApiAction("WareHouse.AppActions.Inwards.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new MasterVTCNTTModel();
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
        // [AppApiAction("WareHouse.AppActions.Inwards.Create")]
        public async Task<IActionResult> Create(IEnumerable<ExampleModel> model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.MasterVTCNTT"))
                });
            var listEntity = new List<Example>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                listEntity.Add(tem);
            }

            var check = await _service.InsertAsync(listEntity);
            // Locales

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Added"), T("Common.MasterVTCNTT"))
            });
        }

        [Route("Create-10000")]
        [HttpGet]
        public async Task<IActionResult> Create10000()
        {
            var list = new List<Example>();
            for (int i = 10001; i < 20000; i++)
            {
                var tem = new Example()
                {
                    Name = "Example" + (i + 1),
                    NumOne = i,
                    NumTwo = i + 1,
                    DateExample = DateTime.Now
                };
                list.Add(tem);
            }
            var res =await _service.InsertAsync(list);
            return Ok(res);
        }

        /// <summary>
        /// Trả về danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<ExampleModel> model)
        {
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.MasterVTCNTT"))
                });
            var listEntity = new List<Example>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                listEntity.Add(tem);
            }

            var check = await _service.UpdateAsync(listEntity);
            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Updated"), T("Common.MasterVTCNTT"))
            });
        }

        /// <summary>
        /// Xóa danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Deletes")]
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
                message = string.Format(T("Common.Notify.Deleted"), T("Common.MasterVTCNTT"))
            });
        }

        #endregion

        /// <summary>
        /// Lấy danh sách  kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-all")]
        [HttpGet]
        //  [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAll()
        {
            var models = new List<ExampleModel>();
            var entities = _service.GetAll();
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