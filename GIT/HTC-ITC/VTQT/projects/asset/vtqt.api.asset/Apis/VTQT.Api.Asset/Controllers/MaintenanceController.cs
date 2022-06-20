using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("maintenance")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.Maintenance")]
    public class MaintenanceController : AdminApiController
    {
        #region Fields
        private readonly IMaintenanceService _maintenanceService;
        private readonly IUserModelHelper _userHelper;
        private readonly IMaintenanceDetailService _maintenanceDetailService;
        private readonly IAssetService _assetService;
        private readonly IAssetCategoryService _assetCategoryService;
        private readonly IHistoryService _historyService;
        #endregion

        #region Ctor
        public MaintenanceController(
            IMaintenanceService maintenanceService,
            IUserModelHelper userHelper,
            IMaintenanceDetailService maintenanceDetailService,
            IAssetService assetService,
            IAssetCategoryService assetCategoryService,
            IHistoryService historyService)
        {
            _maintenanceService = maintenanceService;
            _userHelper = userHelper;
            _maintenanceDetailService = maintenanceDetailService;
            _assetService = assetService;
            _assetCategoryService = assetCategoryService;
            _historyService = historyService;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.Maintenances.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Trả về dữ liệu form bảo dưỡng mới
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("create-maintenance")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(IEnumerable<string> ids)
        {
            var maintenance = new MaintenanceModel
            {
                AvailableUsers = _userHelper.GetMvcListItems(),
                AvailableActions = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)AssetAction.Maintenance).ToString(),
                        Text = AssetAction.Maintenance.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)AssetAction.Repair).ToString(),
                        Text = AssetAction.Repair.GetEnumDescription()
                    }
                },
                MaintenancedDate = DateTime.UtcNow,
                MaintenanceDetails = new List<MaintenanceDetailModel>()
            };

            if (ids?.Count() > 0)
            {
                var assets = _assetService.GetByIdsAsync(ids);
                if (assets?.Count > 0)
                {
                    assets.ToList().ForEach(x =>
                    {
                        var maintenanceDetail = new MaintenanceDetailModel
                        {
                            MaintenanceId = maintenance.Id,
                            AssetCategoryId = x.CategoryId,
                            AssetId = x.Id,
                            AssetCode = x.Code,
                            AssetName = x.Name,
                            AvailablesCategories = _assetCategoryService.GetMvcListItems()                          
                        };
                        maintenance.MaintenanceDetails.Add(maintenanceDetail);
                    });
                }
            }
            
            return Ok(new XBaseResult
            {
                success = true,
                data = maintenance
            });
        }

        /// <summary>
        /// Thêm mới phiếu bảo dưỡng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(MaintenanceModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.MaintenancedDate = model.MaintenancedDate;            

            await _maintenanceService.InsertAsync(entity);

            if (model.MaintenanceDetails?.Count > 0)
            {
                model.MaintenanceDetails.ToList().ForEach(async x =>
                {
                    x.MaintenanceId = entity.Id;

                    var history = new History
                    {
                        Action = model.Action ?? "Bảo dưỡng/ sửa chữa",
                        Content = model.Content ?? $"{model.Action} tài sản",
                        AssetId = x.AssetId,
                        User = entity.EmployeeId,
                        TimeStamp = entity.MaintenancedDate
                    };

                    await _historyService.InsertAsync(history);

                    var asset = await _assetService.GetByIdAsync(x.AssetId);
                    asset.ModifiedBy = entity.EmployeeId;
                    asset.ModifiedDate = DateTime.UtcNow;
                    asset.MaintenancedDate = entity.MaintenancedDate;

                    await _assetService.UpdateAsync(asset);
                });
            }

            if (model.MaintenanceDetails?.Count > 0)
            {
                var entities = new List<MaintenanceDetail>();
                model.MaintenanceDetails.ToList().ForEach(m =>
                {
                    var e = m.ToEntity();
                    entities.Add(e);                    
                });

                if (entities?.Count > 0)
                {
                    await _maintenanceDetailService.InsertAsync(entities);
                }
            }            

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(T("Common.Notify.Added"), T("Common.Maintenance"))
            });
        }
        #endregion

        #region Details
        /// <summary>
        /// lấy chi tiết các tài sản bảo dưỡng
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("detail-create")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailCreate([FromQuery] MaintenanceDetailSearchModel searchModel)
        {
            var searchContext = new MaintenanceDetailSearchContext
            {
                Keywords = searchModel.KeywordsMaintenanceDetail,
                OrganizationId = searchModel.OrganizationMaintenanceDetailId,
                AssetCategoryId = searchModel.AssetCategoryId,
                StationCode = searchModel.StationMaintenanceDetailCode,
                ProjectCode = searchModel.ProjectMaintenanceDetailCode,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                AssetType = (int)searchModel.AssetTypeMaintenanceDetail
            };

            var entities = _maintenanceDetailService.Get(searchContext);
            var models = new List<AssetModel>();
            
            if (entities?.Count > 0)
            {
                entities.ToList().ForEach(x =>
                {
                    var m = x.ToModel();
                    m.CreatedBy = x.CreatedBy;
                    m.ModifiedBy = x.ModifiedBy;
                    m.CreatedDate = x.CreatedDate;
                    m.ModifiedDate = x.ModifiedDate;
                    models.Add(m);
                });
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount
            });
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
