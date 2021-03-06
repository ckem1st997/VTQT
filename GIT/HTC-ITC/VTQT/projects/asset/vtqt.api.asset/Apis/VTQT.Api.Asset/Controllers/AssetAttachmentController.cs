using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VTQT.Core;
using VTQT.Core.Domain.Asset;
using VTQT.Services.Asset;
using VTQT.Services.Localization;
using VTQT.SharedMvc.Asset.Extensions;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Asset.Controllers
{
    [Route("asset-attachment")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Asset.Controllers.AssetAttachment")]
    public class AssetAttachmentController : AdminApiController
    {
        #region Fields
        private readonly IAssetAttachmentService _assetAttachmentService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUserModelHelper _userHelper;
        private readonly IUsageStatusService _usageStatusService;
        private readonly IHistoryService _historyService;
        private readonly IAssetDetailService _assetDetailService;
        #endregion

        #region Ctor
        public AssetAttachmentController(
            IAssetAttachmentService assetAttachmentService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IUserModelHelper userHelper,
            IUsageStatusService usageStatusService,
            IHistoryService historyService,
            IAssetDetailService assetDetailService)
        {
            _assetAttachmentService = assetAttachmentService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _userHelper = userHelper;
            _usageStatusService = usageStatusService;
            _historyService = historyService;
            _assetDetailService = assetDetailService;
        }
        #endregion

        #region Method
        [Route("index")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetAttachments.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Th??m m???i t??i s???n
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Th??m m???i th??nh c??ng</response>
        [Route("create")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AssetAttachments.Create")]
        public async Task<IActionResult> Create(AssetAttachmentModel model)
        {
            model.Code = model.Code?.Trim();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _assetAttachmentService.ExistAsync(model.Code))
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
            entity.AllocationDate = model.AllocationDate.ToUniversalTime();
            entity.Reference = JsonConvert.SerializeObject(model.Reference);

            entity.OriginQuantity = model.OriginQuantity;
            entity.BrokenQuantity = model.BrokenQuantity;
            entity.SoldQuantity = model.SoldQuantity;
            entity.RecallQuantity = model.RecallQuantity;

            var history = new History
            {
                Action = AssetAction.Allocation.GetEnumDescription(),
                Content = $"Ghi t??ng {entity.OriginQuantity} t??i s???n",
                User = entity.CreatedBy,
                TimeStamp = entity.CreatedDate,
                AssetAttachmentId = entity.Id
            };

            await _assetAttachmentService.InsertAsync(entity);

            if (model.AssetProjects?.Count > 0)
            {
                var assetDetails = new List<AssetDetail>();
                model.AssetProjects.ToList()?.ForEach(x =>
                {
                    var assetDetail = new AssetDetail
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        AssetAttachmentId = entity.Id,
                        AssetId = x.Id
                    };
                    assetDetails.Add(assetDetail);
                });

                if (assetDetails.Count > 0)
                {
                    await _assetDetailService.InsertAsync(assetDetails);
                }
            }

            // Locales
            await UpdateLocalesAsync(entity, model);

            await _historyService.InsertAsync(history);

            return Ok(new XBaseResult
            {
                data = entity.Id,
                success = true,
                message = string.Format(
                    T("Common.Notify.Added"),
                    T("Common.AssetAttachment"))
            });
        }

        /// <summary>
        /// T???o m???i t??i s???n r???ng c?? ??a ng??n ng???
        /// </summary>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetAttachments.Create")]
        public async Task<IActionResult> Create()
        {
            var model = new AssetAttachmentModel
            {
                CreatedDate = DateTime.UtcNow,
                AllocationDate = DateTime.UtcNow,
                AvailableUsers = _userHelper.GetMvcListItems(),
                AvailableAssetStatus = _usageStatusService.GetMvcListItem(),
                OriginQuantity = 1
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
        /// C???p nh???t t??i s???n
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">C???p nh???t th??nh c??ng</response>
        [Route("edit")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AssetAttachments.Edit")]
        public async Task<IActionResult> Edit(AssetAttachmentModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _assetAttachmentService.GetByIdAsync(model.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetAttachment"))
                });

            entity = model.ToEntity(entity);
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = model.ModifiedBy;
            entity.AllocationDate = model.AllocationDate.ToUniversalTime();

            if (model.OriginQuantity != entity.OriginQuantity)
            {
                entity.OriginQuantity = model.OriginQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Ghi t??ng {entity.OriginQuantity} t??i s???n",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetAttachmentId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            if (model.SoldQuantity != entity.SoldQuantity)
            {
                entity.SoldQuantity = model.SoldQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Thanh l?? {entity.SoldQuantity} t??i s???n",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetAttachmentId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            if (model.BrokenQuantity != entity.BrokenQuantity)
            {
                entity.BrokenQuantity = model.BrokenQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"H???ng {entity.BrokenQuantity} t??i s???n",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetAttachmentId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            if (model.RecallQuantity != entity.RecallQuantity)
            {
                entity.RecallQuantity = model.RecallQuantity;
                var history = new History
                {
                    Action = AssetAction.Allocation.GetEnumDescription(),
                    Content = $"Thu h???i {entity.RecallQuantity} t??i s???n",
                    User = entity.CreatedBy,
                    TimeStamp = entity.ModifiedDate,
                    AssetAttachmentId = entity.Id
                };

                await _historyService.InsertAsync(history);
            }

            await _assetAttachmentService.UpdateAsync(entity);

            if (model.AssetProjects?.Count > 0)
            {
                var assetDetails = _assetDetailService.GetAssetDetailsByAssetAttachmentId(entity.Id);

                if (assetDetails?.Count > 0)
                {
                    var assetDetailIds = assetDetails.Select(x => x.Id);
                    await _assetDetailService.DeletesAsync(assetDetailIds);
                }

                assetDetails = new List<AssetDetail>();
                model.AssetProjects.ToList()?.ForEach(x =>
                {
                    var assetDetail = new AssetDetail
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        AssetAttachmentId = entity.Id,
                        AssetId = x.Id
                    };
                    assetDetails.Add(assetDetail);
                });

                if (assetDetails.Count > 0)
                {
                    await _assetDetailService.InsertAsync(assetDetails);
                }
            }

            // Locales
            await UpdateLocalesAsync(entity, model);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Updated"),
                    T("Common.AssetAttachment"))
            });
        }

        /// <summary>
        /// Tr??? v??? t??i s???n c???p nh???t c?? ??a ng??n ng???
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetAttachments.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _assetAttachmentService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetAttachment"))
                });

            var model = entity.ToModel();
            model.ModifiedDate = DateTime.UtcNow;
            model.CreatedDate = entity.CreatedDate;
            model.AllocationDate = entity.AllocationDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.AvailableUsers = _userHelper.GetMvcListItems();
            model.AvailableAssetStatus = _usageStatusService.GetMvcListItem();

            var assetProjects = _assetDetailService.GetAssetsByAssetAttachmentId(entity.Id);
            if (assetProjects?.Count > 0)
            {
                model.AssetProjects = new List<AssetProjectModel>();
                foreach (var e in assetProjects)
                {
                    var m = e.ToAssetProjectModel();
                    m.Name = e.Name;
                    m.CreatedDate = e.CreatedDate.ToLocalTime();
                    m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                    m.AllocationDate = e.AllocationDate.ToLocalTime();
                    m.CreatedBy = e.CreatedBy;
                    m.ModifiedBy = e.ModifiedBy;
                    m.MaintenancedDate = e.MaintenancedDate?.ToLocalTime();
                    model.AssetProjects.Add(m);
                }
            }

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
        /// Chi ti???t t??i s???n
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        [AppApiAction("Asset.AppActions.AssetAttachments.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _assetAttachmentService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetAttachment"))
                });

            var model = entity.ToModel();
            model.ModifiedDate = DateTime.UtcNow;
            model.CreatedDate = entity.CreatedDate;
            model.AllocationDate = entity.AllocationDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.AvailableUsers = _userHelper.GetMvcListItems();
            model.AvailableAssetStatus = _usageStatusService.GetMvcListItem();

            var assetProjects = _assetDetailService.GetAssetsByAssetAttachmentId(entity.Id);
            if (assetProjects?.Count > 0)
            {
                model.AssetProjects = new List<AssetProjectModel>();
                foreach (var e in assetProjects)
                {
                    var m = e.ToAssetProjectModel();
                    m.Name = e.Name;
                    m.CreatedDate = e.CreatedDate.ToLocalTime();
                    m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                    m.AllocationDate = e.AllocationDate.ToLocalTime();
                    m.CreatedBy = e.CreatedBy;
                    m.ModifiedBy = e.ModifiedBy;
                    m.MaintenancedDate = e.MaintenancedDate?.ToLocalTime();
                    model.AssetProjects.Add(m);
                }
            }

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
        /// X??a t??i s???n theo danh s??ch
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">X??a th??nh c??ng</response>
        [Route("deletes")]
        [HttpPost]
        [AppApiAction("Asset.AppActions.AssetAttachments.Deletes")]
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

            await _assetAttachmentService.DeleteAsync(ids);

            return Ok(new XBaseResult
            {
                success = true,
                message = string.Format(
                    T("Common.Notify.Deleted"),
                    T("Common.AssetAttachment"))
            });
        }

        /// <summary>
        /// L???y d??? li???u t??i s???n theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">L???y d??? li???u th??nh c??ng</response>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _assetAttachmentService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.AssetAttachment"))
                });

            var model = entity.ToModel();
            model.AllocationDate = entity.AllocationDate;
            model.CreatedBy = entity.CreatedBy;
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedBy = entity.ModifiedBy;
            model.ModifiedDate = entity.ModifiedDate;

            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }
        #endregion

        #region List
        /// <summary>
        /// L???y danh s??ch t??i s???n ph??n trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        /// <response code="200">L???y danh s??ch t??i s???n th??nh c??ng</response>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] AssetAttachmentSearchModel searchModel)
        {
            var searchContext = new AssetAttachmentSearchContext
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

            var models = new List<AssetAttachmentModel>();
            var entities = _assetAttachmentService.Get(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.AllocationDate = e.AllocationDate.ToLocalTime();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.MaintenancedDate = e.MaintenancedDate?.ToLocalTime();
                models.Add(m);
            }

            models.Sort();

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }

        /// <summary>
        /// L???y danh s??ch t??i s???n cho dropdown
        /// </summary>
        /// <returns></returns>
        /// <param name="searchModel"></param>
        /// <response code="200">L???y danh s??ch t??i s???n th??nh c??ng</response>
        [Route("get-select")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetSelect([FromQuery] AssetAttachmentSearchModel searchModel)
        {
            var searchContext = new AssetAttachmentSearchContext
            {
                Keywords = searchModel.Keywords,
                AssetType = (int)searchModel.AssetType,
                LanguageId = searchModel.LanguageId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
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

            var models = new List<AssetAttachmentModel>();
            var entities = _assetAttachmentService.GetMvcDropdownList(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.AllocationDate = e.AllocationDate.ToLocalTime();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.MaintenancedDate = e.MaintenancedDate?.ToLocalTime();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                totalCount = entities.TotalCount,
                success = true
            });
        }

        /// <summary>
        /// L???y danh s??ch t??i s???n ????nh k??m theo t??i s???n
        /// </summary>
        /// <returns></returns>
        /// <param name="assetId"></param>
        /// <response code="200">L???y danh s??ch t??i s???n th??nh c??ng</response>
        [Route("get-attachments-by-asset-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetAttachmentsByAssetId(string assetId)
        {            
            var models = new List<AssetAttachmentModel>();
            var entities = _assetDetailService.GetAssetAttachmentsByAssetId(assetId);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.AllocationDate = e.AllocationDate.ToLocalTime();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.MaintenancedDate = e.MaintenancedDate?.ToLocalTime();
                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models,
                success = true
            });
        }

        /// <summary>
        /// L???y d??? li???u xu???t excel
        /// </summary>
        /// <returns></returns>
        [Route("get-excel")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetExcelData([FromQuery] AssetAttachmentSearchModel searchModel)
        {
            var searchContext = new AssetAttachmentSearchContext
            {
                Keywords = searchModel.Keywords,
                AssetType = (int)searchModel.AssetType,
                LanguageId = searchModel.LanguageId,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
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

            var models = new List<AssetAttachmentModel>();
            var entities = _assetAttachmentService.GetExcelData(searchContext);
            foreach (var e in entities)
            {
                var m = e.ToModel();
                m.Name = await e.GetLocalizedAsync(x => x.Name, searchContext.LanguageId);
                m.CreatedDate = e.CreatedDate.ToLocalTime();
                m.ModifiedDate = e.ModifiedDate.ToLocalTime();
                m.AllocationDate = e.AllocationDate.ToLocalTime();
                m.CreatedBy = e.CreatedBy;
                m.ModifiedBy = e.ModifiedBy;
                m.MaintenancedDate = e.MaintenancedDate?.ToLocalTime();
                models.Add(m);
            }

            models.Sort();

            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }
        #endregion

        #region Utilities
        private async Task UpdateLocalesAsync(AssetAttachment entity, AssetAttachmentModel model)
        {
            foreach (var local in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, local.Name, local.LanguageId);
            }
        }
        #endregion
    }
}