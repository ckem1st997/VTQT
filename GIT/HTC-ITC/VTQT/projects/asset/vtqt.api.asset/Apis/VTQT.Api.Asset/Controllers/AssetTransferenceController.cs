using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("asset-transference")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AssetTransference")]
    public class AssetTransferenceController : AdminApiController
    {
        #region Fields
        private readonly IAssetTransferenceService _assetTransferenceService;
        private readonly IHistoryService _historyService;
        private readonly IAssetService _assetService;
        private readonly IUserModelHelper _userHelper;
        #endregion

        #region Ctor
        public AssetTransferenceController(
            IAssetTransferenceService assetTransferenceService,
            IHistoryService historyService,
            IUserModelHelper userHelper,
            IAssetService assetService)
        {
            _assetTransferenceService = assetTransferenceService;
            _historyService = historyService;
            _assetService = assetService;
            _userHelper = userHelper;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetTransferences.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng điều chuyển
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(int assetType, string id)
        {
            AssetTransferenceModel model = new AssetTransferenceModel
            {
                AvailableUsers = _userHelper.GetMvcListItems(),
                AvailableAssets = _assetService.GetMvcListItems(assetType)
            };

            if (!string.IsNullOrEmpty(id) && id != "undefined")
            {
                var asset = await _assetService.GetByIdAsync(id);

                model.AssetId = asset.Id;
                model.FromCustomerCode = asset.CustomerCode;
                model.FromEmployeeId = asset.EmployeeId;
                model.FromOrganizationId = asset.OrganizationUnitId;
                model.FromProjectCode = asset.ProjectCode;
                model.FromStationCode = asset.StationCode;
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Lưu đối tượng điều chuyển
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(AssetTransferenceModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.TransferedDate = model.TransferedDate;            

            await _assetTransferenceService.InsertAsync(entity);

            await _historyService.InsertAsync(new History
            {
                AssetId = entity.AssetId,
                User = entity.Dispatcher,
                Action = "Điều chuyển",
                Content = "Điều chuyển tài sản",
                TimeStamp = DateTime.UtcNow
            });

            var asset = await _assetService.GetByIdAsync(entity.AssetId);

            if (asset != null)
            {
                bool changed = false;
                if (!string.IsNullOrEmpty(entity.ToCustomerCode))
                {
                    asset.CustomerCode = entity.ToCustomerCode;
                    asset.CustomerName = model.CustomerName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToEmployeeId))
                {
                    asset.EmployeeId = entity.ToEmployeeId;
                    asset.EmployeeName = model.EmployeeName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToOrganizationId))
                {
                    asset.OrganizationUnitId = entity.ToOrganizationId;
                    asset.OrganizationUnitName = model.OrganizationName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToProjectCode))
                {
                    asset.ProjectCode = entity.ToProjectCode;
                    asset.ProjectName = model.ProjectName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToStationCode))
                {
                    asset.StationCode = entity.ToStationCode;
                    asset.StationName = model.StationName;
                    changed = true;
                }

                if (changed)
                {
                    asset.ModifiedBy = entity.Dispatcher;
                    asset.ModifiedDate = DateTime.UtcNow;
                    await _assetService.UpdateAsync(asset);
                }                
            }            

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Transference"),
                    T("Common.Asset"))
            });
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
