using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System;
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
    public class StationController : AdminMvcController
    {
        #region Fields

        #endregion

        #region Ctor

        public StationController()
        {
        }

        #endregion

        #region List
        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var searchModel = new StationSearchModel();

            return View(searchModel);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Lấy chi tiết trạm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<StationModel>("/station/details", new { id = id }, Method.GET, ApiHosts.Ticket);
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
        /// Tạo mới trạm
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<StationModel>("/station/create", null, Method.GET, ApiHosts.Ticket);

            var model = res.data;
            await GetDropDownList(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StationModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/station/create", model, Method.POST, ApiHosts.Ticket);
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
        private async Task GetDropDownList(StationModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<StationCategoryModel>>("/station-category/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelStatus = await ApiHelper
                .ExecuteAsync<List<AreaModel>>("/area/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelArea = await ApiHelper
               .ExecuteAsync<List<StationLevelModel>>("/station-level/get-available", null, Method.GET, ApiHosts.Ticket);

            if (availableUnits?.data?.Count > 0)
            {
                availableUnits.data.ForEach(item =>
                {
                    model.AvailableStationCategory
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
                    model.AvailableArea
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }

            if (availableChannelArea?.data?.Count > 0)
            {
                availableChannelArea.data.ForEach(item =>
                {
                    model.AvailableStationLevel
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }
        }

        /// <summary>
        /// Lấy về kênh cần chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<StationModel>("/station/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetDropDownList(model);

            if (model.AvailableStationCategory.Count > 0 &&
                !string.IsNullOrEmpty(model.StationCategory))
            {
                var item = model.AvailableStationCategory
                    .FirstOrDefault(x => x.Value.Equals(model.StationCategory));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (model.AvailableArea.Count > 0 &&
                !string.IsNullOrEmpty(model.AreaId))
            {
                var item1 = model.AvailableArea
                    .FirstOrDefault(x => x.Value.Equals(model.AreaId));

                if (item1 != null)
                {
                    item1.Selected = true;
                }
            }

            if (model.AvailableStationLevel.Count > 0 &&
                !string.IsNullOrEmpty(model.StationLevel))
            {
                var item2 = model.AvailableStationLevel
                    .FirstOrDefault(x => x.Value.Equals(model.StationLevel));

                if (item2 != null)
                {
                    item2.Selected = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa trạm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(StationModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/station/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa trạm
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
                .ExecuteAsync("/station/deletes", ids, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync("/station/activates", model, Method.POST, ApiHosts.Ticket);
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
        /// Lấy danh sách kênh phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
                                             StationSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<StationModel>>("/station/get", searchModel, Method.GET, ApiHosts.Ticket);
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

        private async Task GetAvailableCategories(StationModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<StationCategoryModel>>("/station-category/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelStatus = await ApiHelper
                .ExecuteAsync<List<AreaModel>>("/area/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelArea = await ApiHelper
               .ExecuteAsync<List<StationLevelModel>>("/station-level/get-available", null, Method.GET, ApiHosts.Ticket);

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

            model.AvailableStationCategory = new List<SelectListItem>(categories);

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

            model.AvailableArea = new List<SelectListItem>(categories1);

            var categories2 = new List<SelectListItem>();
            var data2 = availableChannelArea.data;

            if (data2?.Count > 0)
            {
                foreach (var m2 in data2)
                {
                    var item2 = new SelectListItem
                    {
                        Value = m2.Id,
                        Text = m2.Name
                    };
                    categories2.Add(item2);
                }
            }
            categories2.OrderBy(e => e.Text);
            if (categories2 == null || categories2.Count == 0)
            {
                categories2 = new List<SelectListItem>();
            }

            model.AvailableStationLevel = new List<SelectListItem>(categories2);
        }

        #endregion
    }
}
