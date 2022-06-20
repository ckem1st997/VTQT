using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class CableController : AdminMvcController
    {
        #region Ctor

        public CableController()
        {
        }

        #endregion Ctor

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new CableSearchModel();

            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<CableModel>("/cable/details", new { id = id }, Method.GET, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<CableModel>("/cable/create", null, Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CableModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/cable/create", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<CableModel>("/cable/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CableModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/cable/edit", model, Method.POST, ApiHosts.Ticket);
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

            var res = await ApiHelper.ExecuteAsync("/cable/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methods

        #region Lists

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, CableSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<CableModel>>("/cable/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }

        #endregion Lists
    }
}