using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Asset;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("history")]
    [ApiController]
    [Produces("application/json")]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.History")]
    public class HistoryController : AdminApiController
    {
        #region Fields
        private readonly IHistoryService _historyService;
        #endregion

        #region Ctor
        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }
        #endregion

        #region Methods
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.Histories.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách lịch sử
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] HistorySearchModel searchModel)
        {
            var searchContext = new HistorySearchContext
            {
                Keywords = searchModel.Keywords,                
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                AssetId = searchModel.AssetId
            };

            var models = new List<HistoryModel>();
            var entities = _historyService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.TimeStamp = e.TimeStamp;
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// Lấy danh sách lịch sử tài sản đính kèm
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get-history-attachment")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetHistoryAttachment([FromQuery] HistorySearchModel searchModel)
        {
            var searchContext = new HistorySearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                AssetAttachmentId = searchModel.AssetAttachmentId
            };

            var models = new List<HistoryModel>();
            var entities = _historyService.GetHistoryAttachment(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.TimeStamp = e.TimeStamp;
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }
        #endregion

        #region Utilities

        #endregion
    }
}
