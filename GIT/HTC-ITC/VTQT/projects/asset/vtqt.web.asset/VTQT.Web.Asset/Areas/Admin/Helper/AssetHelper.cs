using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset.Enum;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;

namespace VTQT.Web.Asset.Areas.Admin.Helper
{
    /// <summary>
    /// Lớp Asset helper
    /// </summary>
    public static class AssetHelper
    {
        /// <summary>
        /// Hàm lấy dữ liệu cho các dropdown
        /// </summary>
        /// <returns></returns>
        public static async Task GetAvailableData(AssetModel model, int assetType = 0)
        {
            Task getDurations = GetAvailableDurations(model);
            Task getCategories = GetAvailableAssetCategories(model, assetType);
            Task getItems = GetAvailableItems(model);
            Task getOrganizations = GetAvailableOrganization(model);
            Task getStations = GetAvailableStation(model);
            Task getProjects = GetAvailableProject(model);
            Task getCustomers = GetAvailableCustomer(model);
            Task getUsers = GetAvailableUsers(model);
            Task getUnits = GetAvailableUnits(model);
            List<Task> getData = new List<Task>
            {
                getDurations,
                getCategories,
                getItems,
                getOrganizations,
                getStations,
                getProjects,
                getCustomers,
                getUsers,
                getUnits
            };
            await Task.WhenAll(getData);
        }

        public static async Task GetAvailableUsers(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            var users = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = $"{m.FullName} - {m.Email} ({m.UserName})"
                    };
                    users.Add(item);
                }
            }

            users.OrderBy(e => e.Text);
            if (users == null || users.Count == 0)
            {
                users = new List<SelectListItem>();
            }

            model.AvailableUsers = new List<SelectListItem>(users);
        }

        public static async Task GetAvailableCustomer(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<CustomerModel>>("/customer/get-available", null, Method.GET, ApiHosts.Master);
            var customers = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Code,
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    customers.Add(item);
                }
            }

            customers.OrderBy(e => e.Text);
            if (customers == null || customers.Count == 0)
            {
                customers = new List<SelectListItem>();
            }

            model.AvailableCustomers = new List<SelectListItem>(customers);
        }

        public static async Task GetAvailableProject(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<ProjectModel>>("/project/get-available", null, Method.GET, ApiHosts.Master);
            var projects = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Code,
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    projects.Add(item);
                }
            }

            projects.OrderBy(e => e.Text);
            if (projects == null || projects.Count == 0)
            {
                projects = new List<SelectListItem>();
            }

            model.AvailableProjects = new List<SelectListItem>(projects);  
        }

        public static async Task GetAvailableStation(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<StationModel>>("/station/get-available", null, Method.GET, ApiHosts.Master);
            var stations = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Code,
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    stations.Add(item);
                }
            }

            stations.OrderBy(e => e.Text);
            if (stations == null || stations.Count == 0)
            {
                stations = new List<SelectListItem>();
            }

            model.AvailableStations = new List<SelectListItem>(stations);
        }

        public static async Task GetAvailableOrganization(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<OrganizationModel>>("/organization/get-available", null, Method.GET, ApiHosts.Master);
            var organizations = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    organizations.Add(item);
                }
            }

            organizations.OrderBy(e => e.Text);
            if (organizations == null || organizations.Count == 0)
            {
                organizations = new List<SelectListItem>();
            }

            model.AvailableOrganizations = new List<SelectListItem>(organizations);            
        }

        public static async Task GetAvailableAssetCategories(AssetModel model,int assetType = 0)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<AssetCategoryModel>>($"/asset-category/get-available?assetType={assetType}", null, Method.GET, ApiHosts.Asset);
            var categories = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    categories.Add(item);
                }
            }

            categories.OrderBy(e => e.Text);
            if (categories == null || categories.Count == 0)
            {
                categories = new List<SelectListItem>();
            }

            model.AvailableCategories = new List<SelectListItem>(categories);
        }

        public static async Task GetAvailableDurations(AssetModel model)
        {
            var durations = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)Duration.Date).ToString(),
                    Text = Duration.Date.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)Duration.Month).ToString(),
                    Text = Duration.Month.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)Duration.Year).ToString(),
                    Text = Duration.Year.GetEnumDescription()
                }
            };

            model.AvailableDurations = new List<SelectListItem>(durations);
        }        

        public static async Task GetAvailableItems(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get-select", null, Method.GET, ApiHosts.Warehouse);
            var items = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Code,
                        Text = $"[{m.Code}] {m.Name}"
                    };
                    items.Add(item);
                }
            }

            items.OrderBy(e => e.Text);
            if (items == null || items.Count == 0)
            {
                items = new List<SelectListItem>(); 
            }

            model.AvailableItems = new List<SelectListItem>(items);
        }

        public static async Task GetAvailableUnits(AssetModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<UnitModel>>("/unit/get-select", null, Method.GET, ApiHosts.Warehouse);
            var items = new List<SelectListItem>();
            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Id,
                        Text = m.UnitName
                    };
                    items.Add(item);
                }
            }

            items.OrderBy(e => e.Text);
            if (items == null || items.Count == 0)
            {
                items = new List<SelectListItem>();
            }

            model.AvailableUnits = new List<SelectListItem>(items);
        }
        
        public static void ConvertNumberStr(AssetModel model)
        {
            model.SelectedDepreciationUnit = model.DepreciationUnit.ToString();
            model.SelectedWarrantyUnit = model.WarrantyUnit.ToString();
        }

        public static void UpdatePropertyNameAvailable(AssetModel model)
        {
            if (!string.IsNullOrEmpty(model.WareHouseItemCode) && model.AvailableItems?.Count > 0)
            {
                var item = model.AvailableItems.FirstOrDefault(x => x.Value == model.WareHouseItemCode);
                if (item != null)
                {
                    model.WareHouseItemName = item.Text;
                }
            }

            if (!string.IsNullOrEmpty(model.CustomerCode) && model.AvailableCustomers?.Count > 0)
            {
                var item = model.AvailableCustomers.FirstOrDefault(x => x.Value == model.CustomerCode);
                if (item != null)
                {
                    model.CustomerName = item.Text;
                }
            }

            if (!string.IsNullOrEmpty(model.ProjectCode) && model.AvailableProjects?.Count > 0)
            {
                var item = model.AvailableProjects.FirstOrDefault(x => x.Value == model.ProjectCode);
                if (item != null)
                {
                    model.ProjectName = item.Text;
                }
            }

            if (!string.IsNullOrEmpty(model.OrganizationUnitId) && model.AvailableOrganizations?.Count > 0)
            {
                var item = model.AvailableOrganizations.FirstOrDefault(x => x.Value == model.OrganizationUnitId);
                if (item != null)
                {
                    model.OrganizationUnitName = item.Text;
                }
            }

            if (!string.IsNullOrEmpty(model.StationCode) && model.AvailableStations?.Count > 0)
            {
                var item = model.AvailableStations.FirstOrDefault(x => x.Value == model.StationCode);
                if (item != null)
                {
                    model.StationName = item.Text;
                }
            }

            if (!string.IsNullOrEmpty(model.EmployeeId) && model.AvailableUsers?.Count > 0)
            {
                var item = model.AvailableUsers.FirstOrDefault(x => x.Value == model.EmployeeId);
                if (item != null)
                {
                    model.EmployeeName = item.Text;
                }
            }

            if (!string.IsNullOrEmpty(model.UnitId) && model.AvailableUnits?.Count > 0)
            {
                var item = model.AvailableUnits.FirstOrDefault(x => x.Value == model.UnitId);
                if (item != null)
                {
                    model.UnitName = item.Text;
                }
            }
        }
    }
}
