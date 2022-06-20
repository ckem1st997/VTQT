using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using RestSharp;
using VTQT.Core;
using VTQT.Services.Security;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Master.Models;

namespace VTQT.Web.Master.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class UserController : AdminMvcController
    {
        #region Fields

        private readonly IKeycloakService _keycloakService;

        #endregion

        #region Ctor

        public UserController(IKeycloakService keycloakService)
        {
            _keycloakService = keycloakService;
        }

        #endregion

        #region Methods



        
        public async Task<IActionResult> Index()
        {
            // var res = await ApiHelper.ExecuteAsync<UserSearchModel>("/user/index", null, Method.GET, ApiHosts.Master);
            // if (!res.success)
            //     return HandleApiError(response: res, isAjax: false);
            //
            // var searchModel = res.data;
            //
            // return View(searchModel);
            return View(new UserSearchModel());
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<UserModel>("/user/details", new { id = id }, Method.GET,
                ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<UserModel>("/user/edit", new { id = id }, Method.GET,
                ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/user/edit", model, Method.POST, ApiHosts.Master);

            return HandleApiResponse(res);
        }


        public async Task<IActionResult> GetListRole()
        {
            var searchModel = new RoleSearchModel()
            {
                Keywords = "",
                PageSize = 1000,
                PageIndex = 1
            };
            var res = await ApiHelper.ExecuteAsync<List<RoleModel>>("/role/get", searchModel, Method.GET,
                ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var data = res.data;
            var list = new List<SelectItem>();
            if (res.data != null)
            {
                foreach (var item in res.data)
                {
                    var tem = new SelectItem()
                    {
                        text = item.DisplayName,
                        id = item.Id
                    };
                    list.Add(tem);
                }
            }

            var result = new DataSourceResult
            {
                Data = list,
                Total = res.totalCount
            };
            return Ok(result);
        }


        public async Task<IActionResult> Authorize(string userId)
        {
            var user = _keycloakService.GetUserById(userId);
            var res = await ApiHelper.ExecuteAsync<string>("/user/get-user-role?userId="+userId+"",null, Method.GET, ApiHosts.Master);
            var model = res?.data;
            
            
            var searchModel = new RoleSearchModel()
            {
                Keywords = "",
                PageSize = 1000,
                PageIndex = 1
            };
            var ress = await ApiHelper.ExecuteAsync<List<RoleModel>>("/role/get", searchModel, Method.GET,
                ApiHosts.Master);
            if (!ress.success)
                return HandleApiError(response: ress);

            var data = ress.data;
            var listt = new List<SelectItem>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var tem = new SelectItem()
                    {
                        text = item.DisplayName,
                        id = item.Id
                    };
                    listt.Add(tem);
                }
            }
            
            if (model != null && model.Split(',').Length > 0)
            {
                var tem = "[";
                for (int i = 0; i < model.Split(',').Length; i++)
                {
                  //  tem = tem + "'" + EnumGetNameArgs(listt,model.Split(',')[i]) + "'";
                    tem = tem + "'" + model.Split(',')[i]+ "'";
                    if (i == model.Split(',').Length - 1)
                        tem = tem + "]";
                    else
                        tem = tem + ",";
                }

                model = tem;
            }
            else
            {
                model = "[]";
            }
            var userAuthrize = new UserRoleModel()
            {
                FullName = user.FullName,
                UserName = user.Email,
                UserId = userId,
                Id = userId,
                ListRole = model
                
            };
          

            return View(userAuthrize);
        }

        private string EnumGetNameArgs(List<SelectItem> list,string id)
        {
            // foreach (var UPPER in list)
            // {
            //   if(UPPER.id.Equals(id))
            //     return UPPER.text;
            // }
            // return "";
            return id;
        }
        
        
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(UserRoleModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            var resss = new UserRolesModel()
            {
                RoleId = model.ListRole,
                UserId = model.UserId
            };

            var res = await ApiHelper.ExecuteAsync("/user/set-user-role", resss, Method.POST, ApiHosts.Master);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult()
            {
                data = model
            });
        }

        #endregion

        #region Lists

        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request, UserSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/authorize-user/get", searchModel, Method.GET,
                ApiHosts.Master);
            if (!res.success)
                return HandleApiError(response: res);

            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = data?.Count ?? 0
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