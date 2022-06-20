using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class CSRatingController : AdminMvcController
    {
        #region Fields

        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public CSRatingController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion Ctor

        #region List

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var searchModel = new RatingTicketSearchModel();
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00004" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<RatingTicketModel>("rating-ticket/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            //var resEmployees = await ApiHelper.ExecuteAsync<List<SharedMvc.Master.Models.UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            //var list = new List<SelectListItem>();
            //if (resEmployees.data?.Count > 0)
            //{
            //    foreach (var item in resEmployees.data)
            //    {
            //        var tem = new SelectListItem()
            //        {
            //            Text = item.FullName + "-" + item.UserName,
            //            Value = item.Id
            //        };
            //        list.Add(tem);
            //    }
            //}

            //ViewData["employees"] = list;

            return View(searchModel);
        }

        /// <summary>
        /// Lấy danh sách RatingTicket phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            RatingTicketSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.StrStartDate = searchModel.StartDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrFinishDate = searchModel.FinishDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00004" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<RatingTicketModel>>("/rating-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion List
    }
}