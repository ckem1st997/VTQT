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
    [Route("asset-decreased")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AssetDecreased")]
    public class AssetDecreasedController : AdminApiController
    {
        #region Fields
        private readonly IAssetDecreasedService _assetDecreasedService;
        private readonly IUserModelHelper _userHelper;
        private readonly IHistoryService _historyService;
        private readonly IDecreaseReasonService _decreaseReasonService;
        private readonly IAssetService _assetService;
        private readonly IWareHouseService _wareHouseService;       
        #endregion

        #region Ctor
        public AssetDecreasedController(
            IAssetDecreasedService assetDecreasedService,
            IUserModelHelper userHelper,
            IHistoryService historyService,
            IDecreaseReasonService decreaseReasonService,
            IAssetService assetService,
            IWareHouseService wareHouseService)
        {
            _assetDecreasedService = assetDecreasedService;
            _userHelper = userHelper;
            _historyService = historyService;
            _decreaseReasonService = decreaseReasonService;
            _assetService = assetService;
            _wareHouseService = wareHouseService;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetDecreaseds.Index")]
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
            AssetDecreasedModel assetDecreased = new AssetDecreasedModel
            {
                AvailableReasons = _decreaseReasonService.GetMvcListItem(),
                AvailableUsers = _userHelper.GetMvcListItems(),
                AvailableAssets = _assetService.GetMvcListItems(assetType),
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
        public async Task<IActionResult> Create(AssetDecreasedModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.CreatedDate = entity.ModifiedDate = DateTime.UtcNow;
            entity.DecreaseDate = model.DecreaseDate;
            entity.Id = Guid.NewGuid().ToString("D");

            await _assetDecreasedService.InsertAsync(entity);

            if (!string.IsNullOrEmpty(model.DecreaseReason) && !string.IsNullOrEmpty(model.AssetId))
            {
                var reason = await _decreaseReasonService.GetByIdAsync(model.DecreaseReason);

                var asset = await _assetService.GetByIdAsync(model.AssetId);

                if (reason != null && asset != null)
                {
                    if (reason.Code == AssetAction.Broken.ToString())
                    {
                        var history = new History
                        {
                            AssetId = entity.AssetId,
                            TimeStamp = entity.CreatedDate,
                            User = entity.EmployeeId,
                            Action = AssetAction.Broken.GetEnumDescription(),
                            Content = $"Bị hỏng {entity.Quantity} tài sản"
                        };

                        asset.BrokenQuantity += entity.Quantity;

                        await _assetService.UpdateAsync(asset);
                        await _historyService.InsertAsync(history);
                    }
                    else if (reason.Code == AssetAction.Recall.ToString())
                    {
                        var history = new History
                        {
                            AssetId = entity.AssetId,
                            TimeStamp = entity.CreatedDate,
                            User = entity.EmployeeId,
                            Action = AssetAction.Recall.GetEnumDescription(),
                            Content = $"Thu hồi {entity.Quantity} tài sản"
                        };

                        asset.RecallQuantity += entity.Quantity;

                        await _assetService.UpdateAsync(asset);
                        await _historyService.InsertAsync(history);
                    }
                    else if (reason.Code == AssetAction.Sold.ToString())
                    {
                        var history = new History
                        {
                            AssetId = entity.AssetId,
                            TimeStamp = entity.CreatedDate,
                            User = entity.EmployeeId,
                            Action = AssetAction.Sold.GetEnumDescription(),
                            Content = $"Thanh lý {entity.Quantity} tài sản"
                        };

                        asset.SoldQuantity += entity.Quantity;

                        await _assetService.UpdateAsync(asset);
                        await _historyService.InsertAsync(history);
                    }
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Decreased"),
                    T("Common.Asset"))
            });
        }
        #endregion

        #region List

        #endregion
    }
}
