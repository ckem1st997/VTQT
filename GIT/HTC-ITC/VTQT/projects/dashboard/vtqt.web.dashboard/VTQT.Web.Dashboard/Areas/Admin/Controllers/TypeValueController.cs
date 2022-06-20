using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using VTQT.Core;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Dashboard.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Dashboard.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class TypeValueController : AdminMvcController
    {
        private readonly IWorkContext _workContext;

        public TypeValueController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #region Methods

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var searchModel = new TypeValueSearchModel();

            return View(searchModel);
        }

        /// <summary>
        /// Xem chi tiết đơn vị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<TypeValueModel>("/type-value/details", new { id = id }, Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var res = await ApiHelper.ExecuteAsync<TypeValueModel>("/type-value/edit", new { id = id }, Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            return Ok(res.data);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<TypeValueModel>("/type-value/create", null, Method.GET,
                ApiHosts.Dashboard);
            var model = res?.data;
            return View(model);
        }

        /// <summary>
        /// Gọi Api tạo mới đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(TypeValueModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/type-value/create", model, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Chỉnh sửa đơn vị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<TypeValueModel>("/type-value/details", new { id = id }, Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        /// <summary>
        /// Gọi Api chỉnh sửa đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(TypeValueModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/type-value/edit", model, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api xóa đơn vị
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

            var res = await ApiHelper.ExecuteAsync("/type-value/deletes", ids, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Gọi Api kích hoạt/ ngừng kích hoạt
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

            var res = await ApiHelper.ExecuteAsync("/type-value/activates", model, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region List

        // TODO-Remove
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
            TypeValueSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<TypeValueModel>>("/type-value/get", searchModel,
                Method.GET, ApiHosts.Dashboard);
            var data = res?.data;
            if(data == null)
                return Ok(new DataSourceResult { Data = new List<TypeValueModel>() });
            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }

        public async Task<IActionResult> SetRole(string idTypeValue)
        {
            var res = await ApiHelper.ExecuteAsync<AuthorizeToRoleModel>(
                "/authorize-to-role/create?idTypeValue=" + idTypeValue + "", null, Method.GET,
                ApiHosts.Dashboard);
            var data = res?.data;
            return View(data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetListRoleById(string id)
        {
            var mode = new AuthorizeToRoleModel
            {
                TypeValueId = id
            };
            //var res = await ApiHelper.ExecuteAsync<List<AuthorizeToRoleModel>>("/authorize-to-role/get-list?idTypeValue=" + idTypeValue + "", null, Method.GET,
            //    ApiHosts.Dashboard);
            //var data = res?.data;
            return View(mode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="idTypeValue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetListRoleByIdAjax([DataSourceRequest] DataSourceRequest request,
            string idTypeValue)
        {
            var res = await ApiHelper.ExecuteAsync<List<AuthorizeToRoleModel>>(
                "/authorize-to-role/get-list?idTypeValue=" + idTypeValue + "", null, Method.GET,
                ApiHosts.Dashboard);
            var data = res?.data;
            var result = new DataSourceResult
            {
                Data = data,
                Total = res is { totalCount: 0 } ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetListFile(string idTypeValue)
        {
            var res = await ApiHelper.ExecuteAsync<List<SelectListItem>>(
                "/storage-value/get-mvc-list?idTypeValue=" + idTypeValue + "", null, Method.GET,
                ApiHosts.Dashboard);
            if (res.data != null)
            {
                var list = new List<SelectItem>();
                foreach (var item in res.data)
                {
                    var tem = new SelectItem()
                    {
                        text = item.Text,
                        id = item.Value
                    };
                    list.Add(tem);
                }

                return Ok(list);
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> SetListFile(AuthorizeToRoleModel model)
        {
            var list = new List<AuthorizeToRoleModel>();
            list.Add(model);
            var res = await ApiHelper.ExecuteAsync<List<SelectListItem>>("/authorize-to-role/create", list, Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { success = true });
        }


        //role set user

        public IActionResult SetListRole(string id)
        {
            var model = new TypeValueModel();
            model.Id = id;
            return View(model);
        }
        
        public async Task<IActionResult> ListRoleByUser()
        {
            var model = new DashBoardUserModel();
            return View(model);
        }
        
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetUser([DataSourceRequest] DataSourceRequest request,
            TypeValueSearchModel
                searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET,
                ApiHosts.Master);
            var listRole = await ApiHelper.ExecuteAsync<List<DashBoardUserModel>>(
                "/dashboard-user/get-role?idTypeValue=" + searchModel.Keywords + "", null, Method.GET,
                ApiHosts.Dashboard);
            var data = res?.data;
            if (data != null && listRole?.data != null)
                foreach (var item in data)
                {
                    item.Active = true;
                    foreach (var item1 in listRole?.data)
                    {
                        if (item1.UserId.Equals(item.Id))
                        {
                            item.Active = false;
                            break;
                        }
                    }
                }

            var result = new DataSourceResult
            {
                Data = data.OrderBy(x => x.Active),
                Total = data.Count()
            };
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> SetRole(string idUser, string idTypeValue)
        {
            var list = new List<DashBoardUserModel>();
            var model = new DashBoardUserModel()
            {
                UserId = idUser,
                TypeValueId = idTypeValue,
                CreatedBy = _workContext.UserId
            };
            list.Add(model);
            var res = await ApiHelper.ExecuteAsync<List<UserModel>>("/dashboard-user/create", list, Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var data = res?.data;

            var result = new XBaseResult
            {
                data = data,
                success = res.success
            };
            NotifySuccess(res.message);

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> DeSetRole(string idUser, string idTypeValue)
        {
            var res = await ApiHelper.ExecuteAsync<List<UserModel>>(
                $"/dashboard-user/delete?idTypeValue={idTypeValue}&idUser={idUser}", null, Method.DELETE,
                ApiHosts.Dashboard);
            var data = res?.data;
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            var result = new XBaseResult
            {
                data = data,
                success = res.success
            };
            NotifySuccess(res.message);

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetListRoleByUser(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/dashboard-user/setlist-role-by-user", model, Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }
        
        public async Task<IActionResult> CheckRole(string idUser, string idTypeValue)
        {
            if (string.IsNullOrEmpty(idUser) || string.IsNullOrEmpty(idTypeValue))
                return Ok(new XBaseResult { success = false });
            var res = await ApiHelper.ExecuteAsync($"dashboard-user/check?idUser={idUser}&idTypeValue={idTypeValue}",
                null, Method.GET, ApiHosts.Dashboard);
            return Ok(res);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AccessDeniedPath()
        {
            // var checkRole = await ApiHelper.ExecuteAsync<List<TypeValueModel>>("/dashboard-user/check-role-user?idUser=" + _workContext.UserId + "", null, Method.GET, ApiHosts.Dashboard);
            // if (checkRole.success)
            //     return RedirectToAction("Index", "StorageValue");
            return View();
        }
        #endregion
    }
}