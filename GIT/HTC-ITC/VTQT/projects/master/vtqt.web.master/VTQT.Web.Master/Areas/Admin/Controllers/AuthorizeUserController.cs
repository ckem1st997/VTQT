using System.Collections.Generic;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Master.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class AuthorizeUserController : AdminMvcController
    {
        #region Fields



        #endregion

        #region Ctor

        public AuthorizeUserController(
            )
        {
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new UserSearchModel();

            return View(searchModel);
        }

        public async Task<IActionResult> Authorize(string userId)
        {
            var res = await ApiHelper.ExecuteAsync<UserModel>("/authorize-user/authorize",
                new
                {
                    userId = userId
                }, Method.GET, ApiHosts.Master);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(string userId, IEnumerable<string> appActionIds, string appId)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/authorize-user/authorize",
                new
                {
                    userId = userId,
                    appActionIds = appActionIds,
                    appId = appId
                }
                , Method.POST, ApiHosts.Master);
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

        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, UserSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/authorize-user/get", searchModel, Method.GET, ApiHosts.Master);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        public async Task<IActionResult> GetAppActionTree(string appId, int? expandLevel, string userId, bool showHidden = true)
        {
            var res = await ApiHelper.ExecuteAsync<IList<AppActionTreeModel>>("/authorize-user/get-app-action-tree", new
            {
                appId = appId,
                expandLevel = expandLevel,
                userId = userId,
                showHidden = showHidden
            }, Method.GET, ApiHosts.Master);

            var model = res.data;

            return Ok(model);
        }

        #endregion

        #region Helpers



        #endregion

        #region Utilities



        #endregion
    }
}
