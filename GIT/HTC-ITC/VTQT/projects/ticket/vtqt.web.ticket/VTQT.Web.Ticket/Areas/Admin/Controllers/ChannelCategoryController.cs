using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class ChannelCategoryController : AdminMvcController
    {
        #region Ctor

        public ChannelCategoryController()
        {
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new ChannelCategorySearchModel();

            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<ChannelCategoryModel>("/channel-category/details", new { id = id }, Method.GET,
                ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            await GetAvailableCategories(model);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<ChannelCategoryModel>("/channel-category/create", null, Method.GET,
                ApiHosts.Ticket);

            var model = res.data;
            await GetDropDownList(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChannelCategoryModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/channel-category/create", model, Method.POST, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<ChannelCategoryModel>("/channel-category/edit", new { id = id }, Method.GET,
                ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            await GetDropDownList(model);

            if (model.AvailableChannelArea.Count > 0 &&
                !string.IsNullOrEmpty(model.ChannelAreaId))
            {
                var item = model.AvailableChannelArea
                    .FirstOrDefault(x => x.Value.Equals(model.ChannelAreaId));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ChannelCategoryModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/channel-category/edit", model, Method.POST, ApiHosts.Ticket);
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

            var res = await ApiHelper.ExecuteAsync("/channel-category/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/channel-category/activates", model, Method.POST, ApiHosts.Ticket);
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

        // TODO-Remove
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
            ChannelCategorySearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<ChannelCategoryModel>>("/channel-category/get", searchModel, Method.GET,
                ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion

        #region Utilities

        private async Task GetDropDownList(ChannelCategoryModel model)
        {
            var availableChannelArea = await ApiHelper
                .ExecuteAsync<List<ChannelAreaModel>>("/channel-area/get-available", null, Method.GET, ApiHosts.Ticket);

            if (availableChannelArea?.data?.Count > 0)
            {
                availableChannelArea.data.ForEach(item =>
                {
                    model.AvailableChannelArea
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }
        }

        private async Task GetAvailableCategories(ChannelCategoryModel model)
        {
            var availableChannelArea = await ApiHelper
                .ExecuteAsync<List<ChannelAreaModel>>("/channel-area/get-available", null, Method.GET, ApiHosts.Ticket);

            //NetworkLinkCategory

            var categories6 = new List<SelectListItem>();
            var data6 = availableChannelArea.data;

            if (data6?.Count > 0)
            {
                foreach (var m6 in data6)
                {
                    var item6 = new SelectListItem
                    {
                        Text = m6.Name,
                        Value = m6.Id,
                    };
                    categories6.Add(item6);
                }
            }
            categories6.OrderBy(e => e.Text);
            if (categories6 == null || categories6.Count == 0)
            {
                categories6 = new List<SelectListItem>();
            }

            model.AvailableChannelArea = new List<SelectListItem>(categories6);
        }

        #endregion
    }
}