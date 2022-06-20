using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class ChannelController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ChannelController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion

        #region List
        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var searchModel = new ChannelSearchModel();

            var resEmployees = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            var list = new List<SelectListItem>();
            foreach (var item in resEmployees.data)
            {
                var tem = new SelectListItem()
                {
                    Text = item.FullName + "-" + item.UserName,
                    Value = item.Id
                };
                list.Add(tem);
            }
            ViewData["employees"] = list;

            var availableCustomer = await ApiHelper
               .ExecuteAsync<List<CustomerModel>>("/customer/get-available", null, Method.GET, ApiHosts.Master);
            var list1 = new List<SelectListItem>();
            foreach (var item1 in availableCustomer.data)
            {
                var tem1 = new SelectListItem()
                {
                    Text = item1.Code + "-" + item1.Name,
                    Value = item1.Id.ToString()
                };
                list1.Add(tem1);
            }
            ViewData["customer"] = list1;

            return View(searchModel);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Lấy chi tiết kênh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<ChannelModel>("/channel/details", new { id = id }, Method.GET, ApiHosts.Ticket);
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
                .ExecuteAsync<ChannelModel>("/channel/create", null, Method.GET, ApiHosts.Ticket);

            var model = res.data;
            await GetDropDownList(model);
            model.CreatedDate= DateTime.UtcNow.ToLocalTime();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChannelModel model)
        {
            model.DistanceTimesBandwidth = model.DistanceKilometer * model.TotalBandwidth;
            model.CreatedBy = _workContext.UserId;
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/channel/create", model, Method.POST, ApiHosts.Ticket);
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
        private async Task GetDropDownList(ChannelModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<ChannelCategoryModel>>("/channel-category/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelStatus = await ApiHelper
                .ExecuteAsync<List<ChannelStatusModel>>("/channel-status/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelArea = await ApiHelper
               .ExecuteAsync<List<ChannelAreaModel>>("/channel-area/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableCustomerClass = await ApiHelper
               .ExecuteAsync<List<CustomerClassModel>>("/customer-class/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableCustomer = await ApiHelper
               .ExecuteAsync<List<CustomerModel>>("/customer/get-available", null, Method.GET, ApiHosts.Master);

            var availableUser = await ApiHelper
               .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);

            var availableNetworkLinkCategory = await ApiHelper
               .ExecuteAsync<List<NetworkLinkCategoryModel>>("/network-linkCategory/get-available", null, Method.GET, ApiHosts.Ticket);

            if (availableNetworkLinkCategory?.data?.Count > 0)
            {
                availableNetworkLinkCategory.data.ForEach(item =>
                {
                    model.AvailableNetworkLinkCategory
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }

            if (availableUnits?.data?.Count > 0)
            {
                availableUnits.data.ForEach(item =>
                {
                    model.AvailableChannelCategory
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
                    model.AvailableChannelStatus
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
                    model.AvailableChannelArea
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }

            if (availableCustomerClass?.data?.Count > 0)
            {
                availableCustomerClass.data.ForEach(item =>
                {
                    model.AvailableCustomerClass
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                });
            }

            if (availableCustomer?.data?.Count > 0)
            {
                availableCustomer.data.ForEach(item =>
                {
                    model.AvailableCustomer
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = "[" + item.Code + "] " + item.Name + "",
                    Value = item.Id.ToString()
                    });
                });
            }

            if (availableUser?.data?.Count > 0)
            {
                availableUser.data.ForEach(item =>
                {
                    model.AvailableCreatedBy
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.FullName,
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
            var res = await ApiHelper.ExecuteAsync<ChannelModel>("/channel/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            await GetDropDownList(model);

            if (model.AvailableChannelCategory.Count > 0 &&
                !string.IsNullOrEmpty(model.CategoryId))
            {
                var item = model.AvailableChannelCategory
                    .FirstOrDefault(x => x.Value.Equals(model.CategoryId));

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (model.AvailableChannelStatus.Count > 0 &&
                !string.IsNullOrEmpty(model.ChannelStatusId))
            {
                var item1 = model.AvailableChannelStatus
                    .FirstOrDefault(x => x.Value.Equals(model.ChannelStatusId));

                if (item1 != null)
                {
                    item1.Selected = true;
                }
            }

            if (model.AvailableChannelArea.Count > 0 &&
                !string.IsNullOrEmpty(model.ChannelAreaId))
            {
                var item2 = model.AvailableChannelArea
                    .FirstOrDefault(x => x.Value.Equals(model.ChannelAreaId));

                if (item2 != null)
                {
                    item2.Selected = true;
                }
            }

            if (model.AvailableCustomerClass.Count > 0 &&
                !string.IsNullOrEmpty(model.CustomerCode))
            {
                var item3 = model.AvailableCustomerClass
                    .FirstOrDefault(x => x.Value.Equals(model.CustomerCode));

                if (item3 != null)
                {
                    item3.Selected = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa kênh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(ChannelModel model)
        {
            ModelState.Remove("Code");
            model.DistanceTimesBandwidth = model.DistanceKilometer * model.TotalBandwidth;
            model.ModifiedBy = _workContext.UserId;
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/channel/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa kênh
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
                .ExecuteAsync("/channel/deletes", ids, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync("/channel/activates", model, Method.POST, ApiHosts.Ticket);
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
                                             ChannelSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<ChannelModel>>("/channel/get", searchModel, Method.GET, ApiHosts.Ticket);
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

        private async Task GetAvailableCategories(ChannelModel model)
        {
            var availableUnits = await ApiHelper
                .ExecuteAsync<List<ChannelCategoryModel>>("/channel-category/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelStatus = await ApiHelper
                .ExecuteAsync<List<ChannelStatusModel>>("/channel-status/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableChannelArea = await ApiHelper
               .ExecuteAsync<List<ChannelAreaModel>>("/channel-area/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableCustomerClass = await ApiHelper
               .ExecuteAsync<List<CustomerClassModel>>("/customer-class/get-available", null, Method.GET, ApiHosts.Ticket);

            var availableCustomer = await ApiHelper
               .ExecuteAsync<List<CustomerModel>>("/customer/get-available", null, Method.GET, ApiHosts.Master);

            var availableUser = await ApiHelper
               .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);

            var availableNetworkLinkCategory = await ApiHelper
               .ExecuteAsync<List<NetworkLinkCategoryModel>>("/network-linkCategory/get-available", null, Method.GET, ApiHosts.Ticket);

            //NetworkLinkCategory

            var categories6 = new List<SelectListItem>();
            var data6 = availableNetworkLinkCategory.data;

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

            model.AvailableNetworkLinkCategory = new List<SelectListItem>(categories6);

            //Units

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

            model.AvailableChannelCategory = new List<SelectListItem>(categories);

            //ChannelStatus
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

            model.AvailableChannelStatus = new List<SelectListItem>(categories1);

            //ChannelArea

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

            model.AvailableChannelArea = new List<SelectListItem>(categories2);

            //CustomerClass
            var categories3 = new List<SelectListItem>();
            var data3 = availableCustomerClass.data;

            if (data3?.Count > 0)
            {
                foreach (var m3 in data3)
                {
                    var item3 = new SelectListItem
                    {
                        Value = m3.Id,
                        Text = m3.Name
                    };
                    categories3.Add(item3);
                }
            }
            categories3.OrderBy(e => e.Text);
            if (categories3 == null || categories3.Count == 0)
            {
                categories3 = new List<SelectListItem>();
            }

            model.AvailableCustomerClass = new List<SelectListItem>(categories3);

            //Customer
            var categories4 = new List<SelectListItem>();
            var data4 = availableCustomer.data;

            if (data4?.Count > 0)
            {
                foreach (var m4 in data4)
                {
                    var item4 = new SelectListItem
                    {
                        Value = m4.Id.ToString(),
                        Text = m4.Name
                    };
                    categories4.Add(item4);
                }
            }
            categories4.OrderBy(e => e.Text);
            if (categories4 == null || categories4.Count == 0)
            {
                categories4 = new List<SelectListItem>();
            }

            model.AvailableCustomer = new List<SelectListItem>(categories4);

            //User
            var categories5 = new List<SelectListItem>();
            var data5 = availableUser.data;

            if (data5?.Count > 0)
            {
                foreach (var m5 in data5)
                {
                    var item5 = new SelectListItem
                    {
                        Value = m5.Id,
                        Text = m5.FullName
                    };
                    categories5.Add(item5);
                }
            }
            categories5.OrderBy(e => e.Text);
            if (categories5 == null || categories5.Count == 0)
            {
                categories5 = new List<SelectListItem>();
            }

            model.AvailableCreatedBy = new List<SelectListItem>(categories5);
        }

        #endregion
    }
}
