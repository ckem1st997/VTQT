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
    public class NetworkLinkController : AdminMvcController
    {
        #region Ctor

        public NetworkLinkController()
        {
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var searchModel = new NetworkLinkSearchModel();

            return View(searchModel);
        }

        /// <summary>
        /// Lấy chi tiết link mạng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<NetworkLinkModel>("/network-link/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetAvailableCategories();
            return View(model);
        }

        /// <summary>
        /// Tạo mới link mạng
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<NetworkLinkModel>("/network-link/create", null, Method.GET, ApiHosts.Ticket);

            var model = res.data;

            await GetDropDownList(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NetworkLinkModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/network-link/create", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { });
        }

        /// <summary>
        /// Khởi tạo danh sách dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task GetDropDownList(NetworkLinkModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<CableModel>>("/cable/get-available", null, Method.GET, ApiHosts.Ticket);

            if (availableUnits?.data?.Count > 0)
            {
                availableUnits.data.ForEach(item =>
                {
                    model.AvailableCables
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }
        }

        /// <summary>
        /// Lấy về link mạng cần chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<NetworkLinkModel>("/network-link/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetDropDownList(model);

            if (model.AvailableCables.Count > 0 &&
                !string.IsNullOrEmpty(model.CableId))
            {
                var item = model.AvailableCables
                    .FirstOrDefault(x => x.Value.Equals(model.CableId));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa link mạng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(NetworkLinkModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/network-link/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa link mạng
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/network-link/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api kích hoạt, ngừng kích hoạt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/network-link/activates", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sáchlink mạng phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             NetworkLinkSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<NetworkLinkModel>>("/network-link/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion List

        #region Utilities

        private async Task GetAvailableCategories()
        {
            var res = await ApiHelper
                .ExecuteAsync<List<CableModel>>("/cable/get-available", null, Method.GET, ApiHosts.Ticket);

            var categories = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.Name
                    };
                    categories.Add(item);
                }
            }
            categories.OrderBy(e => e.Text);
            ViewData["cable"] = categories;
        }

        #endregion Utilities
    }
}