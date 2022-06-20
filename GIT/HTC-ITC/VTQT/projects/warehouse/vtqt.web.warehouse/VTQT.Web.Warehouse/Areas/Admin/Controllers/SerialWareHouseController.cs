using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class SerialWareHouseController : AdminMvcController
    {
        #region Fields

        #endregion

        #region Ctor
        public SerialWareHouseController()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var searchModel = new SerialWareHouseSearchModel();

            return View(searchModel);
        }

        /// <summary>
        /// Lấy chi tiết Serial
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<SerialWareHouseModel>("/warehouse-serial/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        /// <summary>
        /// Tạo mới Serial
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper
                .ExecuteAsync<SerialWareHouseModel>("/warehouse-serial/create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;


            return View(model);
        }

       
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách Serial phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, SerialWareHouseSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseItemModel>>("/warehouse-serial/get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }
        #endregion
    }
}