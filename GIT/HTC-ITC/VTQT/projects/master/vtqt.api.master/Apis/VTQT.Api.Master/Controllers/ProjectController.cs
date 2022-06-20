using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("project")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.Project")]
    public class ProjectController : AdminApiController
    {
        #region Fields
        private readonly IProjectService _projectService;
        #endregion

        #region Ctor
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Projects.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu dự án theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _projectService.GetByIdAsync(id);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Project"))
                });
            }

            var model = new ProjectModel
            {
                Id = entity.Id,
                Code = entity.ProjectCode,
                Name = entity.ProjectName
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lấy dữ liệu dự án theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("get-by-code")]
        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var entity = await _projectService.GetByCodeAsync(code);

            if (entity == null)
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Project"))
                });
            }

            var model = new ProjectModel
            {
                Id = entity.Id,
                Code = entity.ProjectCode,
                Name = entity.ProjectName,
                Address = entity.District + ", " + entity.City
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách dự án phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] ProjectSearchModel searchModel)
        {
            var searchContext = new ProjectSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var models = new List<ProjectModel>();
            var entities = _projectService.Get(searchContext);

            if (entities?.Count > 0)
            {
                foreach (var e in entities)
                {
                    var m = new ProjectModel
                    {
                        Id = e.Id,
                        Code = e.ProjectCode,
                        Name = e.ProjectName
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
        /// Lấy danh sách dự án cho dropdown
        /// </summary>
        /// <returns></returns>
        [Route("get-available")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = _projectService.GetAvailable();
            var models = new List<ProjectModel>();

            if (entities?.Count > 0)
            {
                foreach(var e in entities)
                {
                    var m = new ProjectModel
                    {
                        Id = e.Id,
                        Code = e.ProjectCode,
                        Name = e.ProjectName
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }
        #endregion
    }
}
