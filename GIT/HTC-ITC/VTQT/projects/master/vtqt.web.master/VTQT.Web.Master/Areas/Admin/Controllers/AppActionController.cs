using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class AppActionController : AdminMvcController
    {
        #region Fields



        #endregion

        #region Ctor

        public AppActionController(
            )
        {
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index()
        {
            var res = await ApiHelper.ExecuteAsync<AppActionModel>("/app-action/index", null, Method.GET, ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res, isAjax: false);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<AppActionModel>("/app-action/details", new { id = id }, Method.GET, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Create(string appId, string parentId)
        {
            var res = await ApiHelper.ExecuteAsync<AppActionModel>("/app-action/create", new
            {
                appId = appId,
                parentId = parentId
            }, Method.GET, ApiHosts.Master);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppActionModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/app-action/create", model, Method.POST, ApiHosts.Master);
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
            var res = await ApiHelper.ExecuteAsync<AppActionModel>("/app-action/edit", new { id = id }, Method.GET, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppActionModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/app-action/edit", model, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
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

            var res = await ApiHelper.ExecuteAsync("/app-action/deletes", ids, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
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

            var res = await ApiHelper.ExecuteAsync("/app-action/activates", model, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowOnMenu(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/app-action/show-on-menu", model, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Moves(string id, string parentId, int? displayOrder)
        {
            var res = await ApiHelper.ExecuteAsync("/app-action/moves", new
            {
                id = id,
                parentId = parentId,
                displayOrder = displayOrder
            }, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<ActionResult> GetAppActionTree(string appId, int? expandLevel, bool showHidden = true)
        {
            var res = await ApiHelper.ExecuteAsync<List<AppActionTreeModel>>("/app-action/get-app-action-tree", new
            {
                appId = appId,
                expandLevel = expandLevel,
                showHidden = showHidden
            }, Method.GET, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return Ok(model);
        }

        public async Task<ActionResult> GetAppActions(string appId, string appApiControllerName)
        {
            var res = await ApiHelper.ExecuteAsync<dynamic>("/app-action/get-app-actions", new
            {
                appId = appId,
                appApiControllerName = appApiControllerName
            }, Method.GET, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return Ok(model);
        }

        #endregion

        #region Lists



        #endregion

        #region Helpers



        #endregion

        #region Utilities



        #endregion
    }
}
