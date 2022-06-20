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
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Master.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class AppController : AdminMvcController
    {
        #region Ctor

        public AppController()
        {
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new AppSearchModel();

            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AppModel>("/app/details", new { id = id }, Method.GET, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<AppModel>("/app/create", null, Method.GET, ApiHosts.Master);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/app/create", model, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AppModel>("/app/edit", new { id = id }, Method.GET, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/app/edit", model, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/app/deletes", ids, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region Lists

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, AppSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<AppModel>>("/app/get", searchModel, Method.GET, ApiHosts.Master);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion
    }
}
