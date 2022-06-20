using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Api.Dashboard.Helper;
using VTQT.Core;
using VTQT.Core.Domain.Dashboard;
using VTQT.Services.Dashboard;
using VTQT.SharedMvc.Dashboard;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Dashboard.Controllers
{
    
    [Route("ftth-mb-31-3-2022")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
    //   [AppApiController("Dashboard.Controllers.StorageValue")]
    public class FTTHMB3132022Controller : AdminApiController
    {

        #region Fields

        private readonly IFTTHMB3132022Service _service;
        private readonly ITypeValueModelHelper _wareHouseModelHelper;
        private readonly IWorkContext _workContext;
        private readonly IUserModelHelper _userModelHelper;

        #endregion

        #region Ctor

        public FTTHMB3132022Controller(
            IFTTHMB3132022Service service,
            ITypeValueModelHelper wareHouseModelHelper,
            IWorkContext workContext,
            IUserModelHelper userModelHelper
        )
        {
            _service = service;
            _wareHouseModelHelper = wareHouseModelHelper;
            _workContext = workContext;
            _userModelHelper = userModelHelper;
        }

        /// <summary>
        /// Lấy chi tiết danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.MasterVTCNTT"))
                });
            return Ok(new XBaseResult
            {
                data = entity
            });
        }


        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        // [AppApiAction("WareHouse.AppActions.Inwards.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new FTTHModel();
            return Ok(new XBaseResult
            {
                data = model
            });
        }


        /// <summary>
        /// Thêm mới danh sách nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        // [AppApiAction("WareHouse.AppActions.Inwards.Create")]
        public async Task<IActionResult> Create(IEnumerable<FTTHMB3132022Model> model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH"))
                });
            var listEntity = new List<FTTHMB3132022>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                listEntity.Add(tem);
            }

            var check = await _service.InsertAsync(listEntity);
            // Locales

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Added"), T("Common.FTTH"))
            });
        }

        /// <summary>
        /// Trả về danh sách nhập kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpPost]
        [DisableRequestSizeLimit]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Edit")]
        public async Task<IActionResult> Edit(IEnumerable<FTTHMB3132022Model> model)
        {
            if (model == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH "))
                });
            var listEntity = new List<FTTHMB3132022>();
            foreach (var item in model)
            {
                var tem = item.ToEntity();
                listEntity.Add(tem);
            }

            var check = await _service.UpdateAsync(listEntity);
            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Updated"), T("Common.FTTH"))
            });
        }

        [Route("add-or-edit")]
        [HttpPost]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Edit")]
        public async Task<IActionResult> Edit(FTTHMB3132022AddOrUpdateModel model)
        {
            if (model.models == null && model.modelsAdd == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH"))
                });
            long resCount = 0, check = 0, checkAdd = 0;
            if (model.models != null)
            {
                var listEntity = new List<FTTHMB3132022>();
                foreach (var item in model.models)
                {
                    var tem = item.ToEntity();
                    listEntity.Add(tem);
                }

                check = await _service.UpdateAsync(listEntity);
                resCount = resCount + check;
            }

            if (model.modelsAdd != null)
            {
                var listEntityAdd = new List<FTTHMB3132022>();
                foreach (var item in model.modelsAdd)
                {
                    var tem = item.ToEntity();
                    tem.Id ??= Guid.NewGuid().ToString();
                    listEntityAdd.Add(tem);
                }

                checkAdd = await _service.InsertAsync(listEntityAdd);
                resCount = resCount + checkAdd;
            }

            var mes = "";
            switch (resCount > 0)
            {
                case true when check < 1 && model.models == null:
                    mes = string.Format(T("Common.Notify.Updated.Error"), T("Common.FTTH"));
                    break;
                case true when checkAdd < 1 && model.modelsAdd == null:
                    mes = string.Format(T("Common.Notify.Add.Error"), T("Common.FTTH"));
                    break;
                default:
                {
                    if (resCount < 1)
                        mes = string.Format(T("Common.Notify.AddOrUpdate.Error"), T("Common.FTTH"));
                    else if (resCount > 0)
                        mes = string.Format(T("Common.Notify.Updated"), T("Common.FTTH"));
                    break;
                }
            }

            return Ok(new XBaseResult
            {
                success = resCount > 0,
                message = mes
            });
        }

        /// <summary>
        /// Xóa danh sách nhập kho theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("deletes")]
        [HttpPost]
        //  [AppApiAction("WareHouse.AppActions.Inwards.Deletes")]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var check = await _service.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                success = check > 0,
                message = string.Format(T("Common.Notify.Deleted"), T("Common.MasterVTCNTT"))
            });
        }

        #endregion

        /// <summary>
        /// Lấy danh sách  kiểm kê kho phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-query")]
        [HttpPost]
        //  [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAll(string key, int numberPage, IEnumerable<SelectListItem> listItem, string idStorageValue)
        {
            var models = new List<FTTHMB3132022Model>();
            var entities = await _service.GetAllQuery(key, numberPage,listItem, idStorageValue);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }
        
        [Route("get-object")]
        [HttpGet]
        //  [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetObject()
        {
            var models = new List<FTTHMB3132022Model>();
            var entities = await _service.GetObject();
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = await _service.GetCountQuery()
            });
        }
        
        [Route("get-list-share")]
        [HttpGet]
        //  [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetListShare()
        {
            var models = new List<FTTHMB3132022Model>();
            var entities = await _service.GetList();
            foreach (var e in entities)
            {
                var m = e.ToModel();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.Count
            });
        }
    }
}
