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
    public class DeviceController : AdminMvcController
    {
        #region Ctor

        public DeviceController()
        {
        }

        #endregion Ctor

        #region List

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var searchModel = new DeviceSearchModel();
            return View(searchModel);
        }

        #endregion List

        #region Methods

        /// <summary>
        /// Lấy chi tiết thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<DeviceModel>("/device/details", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetAvailableCategories(model);
            return View(model);
        }

        /// <summary>
        /// Tạo mới kênh
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<DeviceModel>("/device/create", null, Method.GET, ApiHosts.Ticket);

            var model = res.data;
            await GetDropDownList(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeviceModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/device/create", model, Method.POST, ApiHosts.Ticket);
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
        private async Task GetDropDownList(DeviceModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelStatus = await ApiHelper
                .ExecuteAsync<List<NetworkLinkModel>>("/network-link/get-available", null, Method.GET, ApiHosts.Ticket);

            if (availableUnits?.data?.Count > 0)
            {
                availableUnits.data.ForEach(item =>
                {
                    model.AvailableStation
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }

            if (availableChannelStatus?.data?.Count > 0)
            {
                availableChannelStatus.data.ForEach(item =>
                {
                    model.AvailableNetworkLink
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }
        }

        /// <summary>
        /// Lấy về thiết bị cần chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<DeviceModel>("/device/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetDropDownList(model);

            if (model.AvailableStation.Count > 0 &&
                !string.IsNullOrEmpty(model.StationId))
            {
                var item = model.AvailableStation
                    .FirstOrDefault(x => x.Value.Equals(model.StationId));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (model.AvailableNetworkLink.Count > 0 &&
                !string.IsNullOrEmpty(model.CategoryId))
            {
                var item1 = model.AvailableNetworkLink
                    .FirstOrDefault(x => x.Value.Equals(model.CategoryId));

                if (item1 != null)
                {
                    item1.Selected = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa thiết bị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(DeviceModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/device/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa thiết bị
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
                .ExecuteAsync("/device/deletes", ids, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync("/device/activates", model, Method.POST, ApiHosts.Ticket);
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
        /// Lấy danh sách thiết bị phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             DeviceSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<DeviceModel>>("/device/get", searchModel, Method.GET, ApiHosts.Ticket);
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

        private async Task GetAvailableCategories(DeviceModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelStatus = await ApiHelper
                .ExecuteAsync<List<NetworkLinkModel>>("/network-link/get-available", null, Method.GET, ApiHosts.Ticket);

            var categories = new List<SelectListItem>();
            var data = availableUnits.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id,
                    };
                    categories.Add(item);
                }
            }
            categories.OrderBy(e => e.Text);
            if (categories == null || categories.Count == 0)
            {
                categories = new List<SelectListItem>();
            }

            model.AvailableStation = new List<SelectListItem>(categories);

            var categories1 = new List<SelectListItem>();
            var data1 = availableChannelStatus.data;

            if (data1?.Count > 0)
            {
                foreach (var m1 in data1)
                {
                    var item1 = new SelectListItem
                    {
                        Text = m1.Name,
                        Value = m1.Id,
                    };
                    categories1.Add(item1);
                }
            }
            categories1.OrderBy(e => e.Text);
            if (categories1 == null || categories1.Count == 0)
            {
                categories1 = new List<SelectListItem>();
            }

            model.AvailableNetworkLink = new List<SelectListItem>(categories1);
        }

        #endregion Utilities
    }
}