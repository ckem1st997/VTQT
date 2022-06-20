using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("attachment-decreased")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AssetAttachmentDecreased")]
    public class AssetAttachmentDecreasedController : AdminApiController
    {
        #region Fields
        private readonly IAssetDecreasedService _assetDecreasedService;
        private readonly IUserModelHelper _userHelper;
        private readonly IHistoryService _historyService;
        private readonly IDecreaseReasonService _decreaseReasonService;
        private readonly IAssetService _assetService;
        private readonly IWareHouseService _wareHouseService;
        private readonly IAssetAttachmentService _assetAttachmentService;
        private readonly IAssetDetailService _assetDetailService;
        #endregion

        #region Ctor
        public AssetAttachmentDecreasedController(
            IAssetDecreasedService assetDecreasedService,
            IUserModelHelper userHelper,
            IHistoryService historyService,
            IDecreaseReasonService decreaseReasonService,
            IAssetService assetService,
            IWareHouseService wareHouseService,
            IAssetAttachmentService assetAttachmentService,
            IAssetDetailService assetDetailService)
        {
            _assetDecreasedService = assetDecreasedService;
            _userHelper = userHelper;
            _historyService = historyService;
            _decreaseReasonService = decreaseReasonService;
            _assetService = assetService;
            _wareHouseService = wareHouseService;
            _assetAttachmentService = assetAttachmentService;
            _assetDetailService = assetDetailService;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetAttachmentDecreaseds.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng ghi giảm tài sản
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(int assetType)
        {
            AssetAttachmentDecreasedModel assetDecreased = new AssetAttachmentDecreasedModel
            {
                AvailableReasons = _decreaseReasonService.GetMvcListItem(),
                AvailableUsers = _userHelper.GetMvcListItems(),
                AvailableAssets = _assetAttachmentService.GetMvcListItems(assetType),
                AvailableWarehouses = _wareHouseService.GetMvcListItems()
            };

            return Ok(new XBaseResult
            {
                success = true,
                data = assetDecreased
            });
        }

        /// <summary>
        /// Thêm mới hủy tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Create(AssetAttachmentDecreasedModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.CreatedDate = entity.ModifiedDate = DateTime.UtcNow;
            entity.DecreaseDate = model.DecreaseDate;
            entity.Id = Guid.NewGuid().ToString("D");

            await _assetDecreasedService.InsertAsync(entity);

            if (!string.IsNullOrEmpty(entity.DecreaseReason) && !string.IsNullOrEmpty(entity.AssetAttachmentId))
            {
                var reason = await _decreaseReasonService.GetByIdAsync(entity.DecreaseReason);

                var assetAttachment = await _assetAttachmentService.GetByIdAsync(entity.AssetAttachmentId);

                if (reason != null && assetAttachment != null)
                {
                    if (reason.Code == AssetAction.Broken.ToString())
                    {
                        var history = new History
                        {
                            AssetAttachmentId = entity.AssetAttachmentId,
                            TimeStamp = entity.CreatedDate,
                            User = entity.EmployeeId,
                            Action = AssetAction.Broken.GetEnumDescription(),
                            Content = $"Bị hỏng {entity.Quantity} tài sản"
                        };

                        assetAttachment.BrokenQuantity += entity.Quantity;

                        await _assetAttachmentService.UpdateAsync(assetAttachment);
                        await _historyService.InsertAsync(history);
                    }
                    else if (reason.Code == AssetAction.Recall.ToString())
                    {
                        var history = new History
                        {
                            AssetAttachmentId = entity.AssetAttachmentId,
                            TimeStamp = entity.CreatedDate,
                            User = entity.EmployeeId,
                            Action = AssetAction.Recall.GetEnumDescription(),
                            Content = $"Thu hồi {entity.Quantity} tài sản"
                        };

                        assetAttachment.RecallQuantity += entity.Quantity;

                        await _assetAttachmentService.UpdateAsync(assetAttachment);
                        await _historyService.InsertAsync(history);
                    }
                    else if (reason.Code == AssetAction.Sold.ToString())
                    {
                        var history = new History
                        {
                            AssetAttachmentId = entity.AssetAttachmentId,
                            TimeStamp = entity.CreatedDate,
                            User = entity.EmployeeId,
                            Action = AssetAction.Sold.GetEnumDescription(),
                            Content = $"Thanh lý {entity.Quantity} tài sản"
                        };

                        assetAttachment.SoldQuantity += entity.Quantity;

                        await _assetAttachmentService.UpdateAsync(assetAttachment);
                        await _historyService.InsertAsync(history);
                    }

                    /*if (!string.IsNullOrEmpty(entity.FromAssetId))
                    {
                        var detailIds = _assetDetailService
                            .GetAssetDetails(entity.FromAssetId, entity.AssetAttachmentId)
                            .Select(x => x.Id);

                        if (detailIds?.Count() > 0)
                        {
                            if (detailIds.Count() <= entity.Quantity)
                            {
                                assetAttachment.AttachmentQuantity = 0;
                                await _assetDetailService.DeletesAsync(detailIds);

                            }
                            else
                            {
                                var detailIdsNew = new List<string>();
                                for (int i = 0; i < entity.Quantity; i++)
                                {
                                    detailIdsNew.Add(detailIds.ToList()[i]);
                                }
                                assetAttachment.AttachmentQuantity -= entity.Quantity;
                                await _assetDetailService.DeletesAsync(detailIdsNew);
                            }
                        }
                    }*/
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Decreased"),
                    T("Common.AssetAttachment"))
            });
        }
        #endregion

        #region List

        #endregion
    }
}
