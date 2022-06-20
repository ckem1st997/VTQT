using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Asset.Controllers
{
    [Route("asset")]
    [ApiController]
    [Produces("application/json")]
    public class AssetController : AdminApiController
    {
        #region Fields
        private readonly IAssetService _assetService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUserModelHelper _userHelper;
        private readonly IUsageStatusService _usageStatusService;
        private readonly IHistoryService _historyService;
        #endregion

        #region Ctor
        public AssetController(
            IAssetService assetService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IUserModelHelper userHelper,
            IUsageStatusService usageStatusService,
            IHistoryService historyService)
        {
            _assetService = assetService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _userHelper = userHelper;
            _usageStatusService = usageStatusService;
            _historyService = historyService;
        }
        #endregion

        #region Method
        /// <summary>
        /// Thêm mới tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Thêm mới thành công</response>
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(AssetModel model)
        {
            model.Code = model.Code?.Trim();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _assetService.ExistAsync(model.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.AlreadyExist"),
                        T("Asset.Assets.Fields.Code"))
                });

            var entity = model.ToEntity();
            entity.Code = model.Code;
            entity.CreatedDate = entity.ModifiedDate = DateTime.UtcNow;
            entity.CreatedBy = entity.ModifiedBy = model.CreatedBy;            
            entity.Reference = JsonConvert.SerializeObject(model.Reference);

            entity.OriginQuantity = model.OriginQuantity;
            entity.BrokenQuantity = model.BrokenQuantity;
            entity.SoldQuantity = model.SoldQuantity;
            entity.RecallQuantity = model.RecallQuantity;

            var history = new History
            {
                Action = AssetAction.Allocation.GetEnumDescription(),
                Content = $"Ghi tăng {entity.OriginQuantity} tài sản",
                User = entity.CreatedBy,
                TimeStamp = entity.CreatedDate,
                AssetId = entity.Id
            };          

            await _assetService.InsertAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);

            await _historyService.InsertAsync(history);

            return Ok(new XBaseResult
            {
                data = entity.Id,
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.Asset"))
            });
        }

        /// <summary>
        /// Tạo mới tài sản rỗng có đa ngôn ngữ
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AssetModel
            {
                CreatedDate = DateTime.UtcNow,
                AvailableUsers = _userHelper.GetMvcListItems(),
                OriginQuantity = 1,
                ManufactureYear = DateTime.Now.Year,
                AvailableAssetStatus = _usageStatusService.GetMvcListItem()
            };

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Cập nhật tài sản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Cập nhật thành công</response>
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(AssetModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _assetService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Asset"))
                });

            entity = model.ToEntity(entity);
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = model.ModifiedBy;

            if (model.OriginQuantity != entity.OriginQuantity)
            {
                entity.OriginQuantity = model.OriginQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Ghi tăng {entity.OriginQuantity} tài sản",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }            

            if (model.SoldQuantity != entity.SoldQuantity)
            {
                entity.SoldQuantity = model.SoldQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Thanh lý {entity.SoldQuantity} tài sản",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            if (model.BrokenQuantity != entity.BrokenQuantity)
            {
                entity.BrokenQuantity = model.BrokenQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Hỏng {entity.BrokenQuantity} tài sản",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            if (model.RecallQuantity != entity.RecallQuantity)
            {
                entity.RecallQuantity = model.RecallQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Thu hồi {entity.RecallQuantity} tài sản",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            await _assetService.UpdateAsync(entity);

            // Locales
            await UpdateLocalesAsync(entity, model);            

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.Asset"))
            });
        }

        /// <summary>
        /// Trả về tài sản cập nhật có đa ngôn ngữ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _assetService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Asset"))
                });

            var model = entity.ToModel();
            model.ModifiedDate = DateTime.UtcNow;
            model.CreatedDate = entity.CreatedDate;
            model.AvailableUsers = _userHelper.GetMvcListItems();
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.AvailableAssetStatus = _usageStatusService.GetMvcListItem();

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// Xóa tài sản theo danh sách
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Xóa thành công</response>
        [Route("deletes")]
        [HttpPost]
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

            await _assetService.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.Asset"))
            });
        }

        /// <summary>
        /// Lấy dữ liệu tài sản theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Lấy dữ liệu thành công</response>
        [Route("get-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _assetService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Asset"))
                });

            var model = entity.ToModel();

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách tài sản phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">Lấy danh sách tài sản thành công</response>
        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AssetSearchModel searchModel)
        {
            var searchContext = new AssetSearchContext
            {
                Keywords = searchModel.Keywords,
                AssetType = (int)searchModel.AssetType,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                LanguageId = searchModel.LanguageId,
                OrganizationId = searchModel.OrganizationId,
                EmployeeId = searchModel.EmployeeId,
                CustomerCode = searchModel.CustomerCode,
                ProjectCode = searchModel.ProjectCode,
                StationCode = searchModel.StationCode,
                CategoryId = searchModel.CategoryId
            };

            if (!string.IsNullOrEmpty(searchModel.StrFromDate))
            {
                searchContext.FromDate = DateTime.ParseExact(searchModel.StrFromDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrToDate))
            {
                searchContext.ToDate = DateTime.ParseExact(searchModel.StrToDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            var models = new List<AssetModel>();
            var entities = _assetService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.MaintenancedDate = e.MaintenancedDate.ToLocalTime();
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
        /// Lấy danh sách tài sản cho theo kiểu tài sản
        /// </summary>
        /// <returns></returns>
        /// <param name="assetType"></param>
        /// <response code="200">Lấy danh sách tài sản thành công</response>
        [Route("get-available")]
        [HttpGet]
        public ActionResult GetAvailable(int assetType)
        {
            var results = _assetService.GetMvcListItems(assetType);         

            return Ok(new XBaseResult
            {
                data = results
            });
        }
        
        [Route("get-available-short")]
        [HttpGet]
        public async Task<ActionResult> GetAvailableSort()
        {
            var entities = _assetService.GetListDropDownd();
            var models = new List<AssetModel>();

            if (entities?.Count > 0)
            {
                foreach (var e in entities)
                {
                    var m = e.ToModel();
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }
        #endregion

        #region Utilities
        private async Task UpdateLocalesAsync(Core.Domain.Asset.Asset entity, AssetModel model)
        {
            foreach (var local in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, local.Name, local.LanguageId);
            }
        }
        #endregion
    }
}
