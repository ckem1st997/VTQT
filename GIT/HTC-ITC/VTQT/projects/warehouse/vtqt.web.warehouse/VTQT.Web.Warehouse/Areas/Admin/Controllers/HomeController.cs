using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class HomeController : AdminMvcController
    {
        #region Fields

        #endregion

        #region Ctor

        public HomeController(
        )
        {
        }

        #endregion

        #region Methods

        public  IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Lists

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetItemId()
        {
            var resWarehouseItem = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("/warehouse-item/get-select",
                null, Method.GET, ApiHosts.Warehouse);
            // var res = await ApiHelper.ExecuteAsync<InwardModel>("/inward/create", null, Method.GET, ApiHosts.Warehouse);

            var list = new List<SelectItem>();
            foreach (var item in resWarehouseItem.data)
            {
                var tem = new SelectItem();
                tem.text = "[" + item.Code + "] " + item.Name + "";
                tem.id = item.Id;
                list.Add(tem);
            }

            return Ok(list);
        }

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetWareHouseId()
        {
            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/outward/create", null, Method.GET,
                ApiHosts.Warehouse);
            var list = new List<SelectItem>();
            foreach (var item in res.data.AvailableToWareHouses)
            {
                var tem = new SelectItem();
                tem.text = item.Text;
                tem.id = item.Value;
                list.Add(tem);
            }

            return Ok(list);
        }


        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> ReadWareHouseBalance([DataSourceRequest] DataSourceRequest request,
            WarehouseBalanceSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<WarehouseBalanceModel>>("/warehouse-balance/get-list-to-home",
                searchModel, Method.GET, ApiHosts.Warehouse);

            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> ReadWareHouseLimit([DataSourceRequest] DataSourceRequest request,
            WareHouseLimitSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<WareHouseLimitModel>>("/warehouse-limit/get-list-to-home",
                searchModel, Method.GET, ApiHosts.Warehouse);

            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion

        #region Helpers

        #endregion

        #region Utilities

        #endregion
    }
}