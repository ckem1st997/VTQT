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
    [Route("attachment-transference")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AssetAttachmentTransference")]
    public class AssetAttachmentTransferenceController : AdminApiController
    {
        #region Fields
        private readonly IAssetTransferenceService _assetTransferenceService;
        private readonly IHistoryService _historyService;
        private readonly IAssetService _assetService;
        private readonly IUserModelHelper _userHelper;
        private readonly IAssetAttachmentService _assetAttachmentService;
        private readonly IAssetDetailService _assetDetailService;
        #endregion

        #region Ctor
        public AssetAttachmentTransferenceController(
            IAssetTransferenceService assetTransferenceService,
            IHistoryService historyService,
            IUserModelHelper userHelper,
            IAssetService assetService,
            IAssetDetailService assetDetailService,
            IAssetAttachmentService assetAttachmentService)
        {
            _assetTransferenceService = assetTransferenceService;
            _historyService = historyService;
            _assetService = assetService;
            _userHelper = userHelper;
            _assetAttachmentService = assetAttachmentService;
            _assetDetailService = assetDetailService;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetAttachmentTransferences.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng điều chuyển
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetAttachmentId"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(int assetType, string assetAttachmentId)
        {
            AssetAttachmentTransferenceModel model = new AssetAttachmentTransferenceModel
            {
                AvailableUsers = _userHelper.GetMvcListItems(),
                AvailableAssets = _assetAttachmentService.GetMvcListItems(assetType)
            };

            if (!string.IsNullOrEmpty(assetAttachmentId) && assetAttachmentId != "undefined")
            {
                var assetAttachment = await _assetAttachmentService.GetByIdAsync(assetAttachmentId);

                if (assetAttachment != null)
                {
                    model.AssetAttachmentId = assetAttachment.Id;
                    model.FromEmployeeId = assetAttachment.EmployeeId;
                    model.FromOrganizationId = assetAttachment.OrganizationUnitId;
                    model.FromProjectCode = assetAttachment.ProjectCode;
                    model.FromStationCode = assetAttachment.StationCode;
                }                
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
        public async Task<IActionResult> Create(AssetAttachmentTransferenceModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.TransferedDate = model.TransferedDate;

            await _assetTransferenceService.InsertAsync(entity);

            await _historyService.InsertAsync(new History
            {
                AssetAttachmentId = entity.AssetAttachmentId,
                User = entity.Dispatcher,
                Action = "Điều chuyển",
                Content = "Điều chuyển tài sản",
                TimeStamp = DateTime.UtcNow
            });

            var assetAttachment = await _assetAttachmentService.GetByIdAsync(entity.AssetAttachmentId);

            if (assetAttachment != null)
            {
                bool changed = false;

                if (!string.IsNullOrEmpty(entity.ToEmployeeId))
                {
                    assetAttachment.EmployeeId = entity.ToEmployeeId;
                    assetAttachment.EmployeeName = model.EmployeeName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToOrganizationId))
                {
                    assetAttachment.OrganizationUnitId = entity.ToOrganizationId;
                    assetAttachment.OrganizationUnitName = model.OrganizationName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToProjectCode))
                {
                    assetAttachment.ProjectCode = entity.ToProjectCode;
                    assetAttachment.ProjectName = model.ProjectName;
                    changed = true;
                }

                if (!string.IsNullOrEmpty(entity.ToStationCode))
                {
                    assetAttachment.StationCode = entity.ToStationCode;
                    assetAttachment.StationName = model.StationName;
                    changed = true;
                }

                if (changed)
                {
                    assetAttachment.ModifiedBy = entity.Dispatcher;
                    assetAttachment.ModifiedDate = DateTime.UtcNow;
                    await _assetAttachmentService.UpdateAsync(assetAttachment);
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Transference"),
                    T("Common.AssetAttachment"))
            });
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
