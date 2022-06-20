using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;

namespace VTQT.Web.Warehouse.Helper
{
    public static class WareHouseHelper
    {
        public static async Task GetAvailableData(WareHouseItemCategoryModel model)
        {
            Task getCategories = GetAvailableWareHouseItemCategories(model);
            List<Task> getData = new List<Task>
            {
                getCategories,
            };
            await Task.WhenAll(getData);
        }

        public static async Task GetAvailableWareHouseItemCategories(WareHouseItemCategoryModel model)
        {
            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseItemCategoryModel>>("/warehouse-item-category/get-available", null, Method.GET, ApiHosts.Warehouse);
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
            if (categories == null || categories.Count == 0)
            {
                categories = new List<SelectListItem>();
            }

            model.AvailableCategories = new List<SelectListItem>(categories);
        }
    }
}
