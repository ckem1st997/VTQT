using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Master.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class RoleController : AdminMvcController
    {
        #region Fields



        #endregion

        #region Ctor

        public RoleController(
            )
        {
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index()
        {
            var res = await ApiHelper.ExecuteAsync<RoleSearchModel>("/role/index", null, Method.GET, ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res, isAjax: false);

            var searchModel = res.data;

            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<RoleModel>("/role/details", new { id = id }, Method.GET, ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<RoleModel>("/role/create", null, Method.GET, ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/role/create", model, Method.POST, ApiHosts.Master);

            return HandleApiResponse(res);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<RoleModel>("/role/edit", new { id = id }, Method.GET, ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/role/edit", model, Method.POST, ApiHosts.Master);

            return HandleApiResponse(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/role/deletes", ids, Method.POST, ApiHosts.Master);

            return HandleApiResponse(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/role/activates", model, Method.POST, ApiHosts.Master);

            return HandleApiResponse(res);
        }

        #endregion

        #region Lists

        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, RoleSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<RoleModel>>("/role/get", searchModel, Method.GET, ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion

        #region Helpers



        #endregion

        #region Utilities



        #endregion
    }
}
