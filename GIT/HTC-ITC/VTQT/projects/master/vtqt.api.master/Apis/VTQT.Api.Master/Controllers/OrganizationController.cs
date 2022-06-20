using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Services.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("organization")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.Organization")]
    public class OrganizationController : AdminApiController
    {
        #region Fields
        private readonly IOrganizationService _organizationService;
        private readonly IXBaseCacheManager _cacheManager;
        #endregion

        #region Ctor
        public OrganizationController(
            IOrganizationService organizationService,
            IXBaseCacheManager cacheManager)
        {
            _organizationService = organizationService;
            _cacheManager = cacheManager;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Organizations.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu tổ chức/ phòng ban theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _organizationService.GetByIdAsync(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Organization"))
                });
            }            

            var model = new OrganizationModel
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy giá trị lựa chọn phòng ban cuối cùng
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Route("get-last-selected")]
        [HttpGet]
        public async Task<IActionResult> GetLastSelectedDepartment(
            string appId,
            string userId,
            string path,
            string departmentId)
        {
            if (string.IsNullOrEmpty(appId) ||
                string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(path))
            {
                return Ok(new XBaseResult
                {
                    data = null
                });
            }

            var result = _organizationService.GetLastSelectedNodeTree(appId, userId, path, departmentId);

            return Ok(new XBaseResult
            {
                success = true,
                data = result
            });
        }

        /// <summary>
        /// Cập nhật cache key chọn phòng ban
        /// </summary>        
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Route("update-last-selected")]
        [HttpPost]
        public async Task<IActionResult> UpdateLastSelectedDepartment(            
            string appId,
            string userId,
            string path,
            string departmentId)
        {
            if (string.IsNullOrEmpty(appId) ||
                string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(path))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            await _cacheManager.HybridProvider.RemoveAsync(string.Format(ModelCacheKeys.DepartmentsTreeModelCacheKey, appId, userId, path));

            var result = _organizationService.GetLastSelectedNodeTree(appId, userId, path, departmentId);

            return Ok(new XBaseResult
            {
                success = true,
                data = result
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách tổ chức/ phòng ban phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] OrganizationSearchModel searchModel)
        {
            var searchContext = new OrganizationSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<OrganizationModel>();
            var entities = _organizationService.Get(searchContext);

            if (entities?.Count > 0)
            {
                foreach (var e in entities)
                {
                    var m = new OrganizationModel
                    {
                        Id = e.Id,
                        Code = e.Code,
                        Name = e.Name
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// Lấy danh sách tổ chức/ phòng ban cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = _organizationService.GetAvailable();
            var models = new List<OrganizationModel>();

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = new OrganizationModel
                    {
                        Id = e.Id,
                        Code = e.Code,
                        Name = e.Name
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        [Route("get-tree")]
        [HttpPost]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetTree()
        {
            var entity = _organizationService.GetOrganizationTree(2);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Organization"))
                });

            return Ok(new XBaseResult
            {
                data = entity
            });
        }
        #endregion
    }
}
