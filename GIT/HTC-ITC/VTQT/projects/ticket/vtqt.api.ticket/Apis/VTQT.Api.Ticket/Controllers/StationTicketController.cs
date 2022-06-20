using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket.Enum;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Security;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using IStationService = VTQT.Services.Ticket.IStationService;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("station-ticket")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.StationTicket")]
    public class StationTicketController : AdminApiController
    {
        #region Fields
        private readonly IStationTicketService _stationTicketService;
        private readonly ITicketService _ticketService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly ITicketCategoryService _ticketCategoryService;
        private readonly IStatusSerive _statusService;
        private readonly IPriorityService _priorityService;
        private readonly IOrganizationUnitService _organizationUnitService;
        private readonly ITicketAreaService _ticketAreaService;
        private readonly ITicketReasonService _ticketReasonService;
        private readonly ITicketProvinceService _ticketProvinceService;
        private readonly IApprovalTicketService _approvalTicketService;
        private readonly INetworkLinkTicketService _networkLinkTicketService;
        private readonly IChannelTicketService _channelTicketService;
        private readonly IDeviceTicketService _deviceTicketService;
        private readonly IInfrastructorFeeService _infrastructorFeeService;
        private readonly ICommentService _commentService;
        private readonly IFileService _fileService;
        private readonly INetworkLinkService _networkLinkService;
        private readonly IChannelService _channelService;
        private readonly IDeviceService _deviceService;
        private readonly IPhenomenaService _phenomenaService;
        private readonly ICableService _cableService;
        private readonly IApprovalProgressService _approvalProgressService;
        private readonly IStationService _stationService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;
        private readonly IKeycloakService _keycloakService;
        #endregion

        #region Ctor
        public StationTicketController(
            IStationTicketService stationTicketService,
            ITicketService ticketService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService,
            IUserModelHelper userModelHelper,
            ITicketCategoryService ticketCategoryService,
            IStatusSerive statusSerive,
            IPriorityService priorityService,
            IOrganizationUnitService organizationUnitService,
            ITicketAreaService ticketAreaService,
            ITicketReasonService ticketReasonService,
            ITicketProvinceService ticketProvinceService,
            IApprovalTicketService approvalTicketService,
            INetworkLinkTicketService networkLinkTicketService,
            IChannelTicketService channelTicketService,
            IDeviceTicketService deviceTicketService,
            IInfrastructorFeeService infrastructorFeeService,
            ICommentService commentService,
            IFileService fileService,
            INetworkLinkService networkLinkService,
            IChannelService channelService,
            IDeviceService deviceService,
            IPhenomenaService phenomenaService,
            IApprovalProgressService approvalProgressService,
            ICableService cableService,
            IStationService stationService,
            ISendDiscordHelper sendDiscordHelper,
            IWorkContext workContext,
            IKeycloakService keycloakService)
        {
            _stationTicketService = stationTicketService;
            _ticketService = ticketService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _autoCodeService = autoCodeService;
            _userModelHelper = userModelHelper;
            _ticketCategoryService = ticketCategoryService;
            _statusService = statusSerive;
            _priorityService = priorityService;
            _organizationUnitService = organizationUnitService;
            _ticketAreaService = ticketAreaService;
            _ticketReasonService = ticketReasonService;
            _ticketProvinceService = ticketProvinceService;
            _approvalTicketService = approvalTicketService;
            _networkLinkTicketService = networkLinkTicketService;
            _channelTicketService = channelTicketService;
            _deviceTicketService = deviceTicketService;
            _infrastructorFeeService = infrastructorFeeService;
            _commentService = commentService;
            _fileService = fileService;
            _networkLinkService = networkLinkService;
            _channelService = channelService;
            _deviceService = deviceService;
            _phenomenaService = phenomenaService;
            _cableService = cableService;
            _approvalProgressService = approvalProgressService;
            _stationService = stationService;
            _sendDiscordHelper = sendDiscordHelper;
            _workContext = workContext;
            _keycloakService = keycloakService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.StationTicket.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng ticket sự cố
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.StationTicket.Create")]
        public IActionResult Create(string projectId)
        {
            var model = new TicketModel
            {
                AvailableUsers = _userModelHelper.GetMvcListItems(),
                AvailableCategories = _ticketCategoryService.GetMvcListItems(false),
                AvailableStatus = _statusService.GetMvcListItems(false, projectId),
                AvailablePriorities = _priorityService.GetMvcListItems(false, projectId),
                AvailableOrganizationUnits = _organizationUnitService.GetMvcListItems(false, projectId),
                AvailableCables = _cableService.GetMvcListItems(false),
                AvailablePhenomenas = _phenomenaService.GetMvcListItems(false),
                AvailableAprrovalProgress = _approvalProgressService.GetMvcListItems(false),    
                AvailableStations = _stationService.GetMvcListItems(false),
                StationTicketModel = new StationTicketModel
                {
                    AvailableTicketAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableTicketProvinces = _ticketProvinceService.GetMvcListItems(false)
                },
                AvailableApprovers = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ApproverTicket.DeputyGeneralManagerTechnical).ToString(),
                        Text = ApproverTicket.DeputyGeneralManagerTechnical.GetEnumDescription()
                    },
                    new SelectListItem
                    {
                        Value = ((int)ApproverTicket.DeputyGeneralManagerNetworkInfrastructor).ToString(),
                        Text = ApproverTicket.DeputyGeneralManagerNetworkInfrastructor.GetEnumDescription()
                    }
                }
            };

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới ticket sự cố
        /// </summary>
        /// <param name="ticketModel"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.StationTicket.Create")]
        public async Task<IActionResult> Create(TicketModel ticketModel)
        {
            ticketModel.Code = DateTime.UtcNow.ToUnixTime().ToString();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _ticketService.ExistedAsync(ticketModel.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.Tickets.Fields.Code"))
                });

            InsertTicket(ticketModel);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(ticketModel.CreatedBy).UserName + " đã thêm mới ticket trạm có mã là: " + ticketModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Ticket"))
            });
        }




        /// <summary>
        /// Lấy dữ ticket sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.StationTicket.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _ticketService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Ticket"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;
            model.Deadline = entity.Deadline;

            UpdataDataTicket(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Subject = entity.GetLocalized(x => x.Subject, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật ticket sự cố
        /// </summary>
        /// <param name="ticketModel"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.StationTicket.Edit")]
        public async Task<IActionResult> Edit(TicketModel ticketModel)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _ticketService.GetByIdAsync(ticketModel.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Ticket"))
                });

            UpdateTicket(ticketModel, entity);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(ticketModel.CreatedBy).UserName + " đã sửa ticket trạm có mã là: " + ticketModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.TicketCategory"))
            });
        }

        /// <summary>
        /// Lấy chi tiết loại ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.StationTicket.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _ticketService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.Ticket"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;
            model.Deadline = entity.Deadline;

            UpdataDataTicket(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Subject = entity.GetLocalized(x => x.Subject, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách ticket sự cố
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.StationTicket.Deletes")]
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
            var mes = _workContext.UserName + " đã xóa ticket trạm có mã là: ";
            foreach (var item in ids)
            {
                var entity = await _ticketService.GetByIdAsync(item);
                if (entity != null)
                {
                    mes = mes + entity.Code + "  ";
                }
            }
            var res = await _ticketService.DeletesAsync(ids);
            if (res > 0)
                await _sendDiscordHelper.SendMessage(mes);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.Ticket"))
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách ticket và ticket sự cố
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] StationTicketGridSearchModel searchModel)
        {
            var searchContext = new StationTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                ProjectId = searchModel.ProjectId,
                CreatedBy = searchModel.CreatedBy,
                PriorityId = searchModel.PriorityId,
                StatusId = searchModel.StatusId,
                Assignee = searchModel.Assignee,
                TicketAreaId = searchModel.TicketAreaId,
                TicketProvinceId = searchModel.TicketProvinceId                
            };
            if (!string.IsNullOrEmpty(searchModel.StrStartDate))
            {
                searchContext.StartDate = DateTime.ParseExact(searchModel.StrStartDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrFinishDate))
            {
                searchContext.FinishDate = DateTime.ParseExact(searchModel.StrFinishDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            var models = _stationTicketService.Get(searchContext);

            return Ok(new XBaseResult
            {
                data = models,
                success = true,
                totalCount = models.TotalCount
            });
        }
        #endregion

        #region Excel
        [Route("get-excel")]
        [HttpGet]
        public async Task<IActionResult> GetExcelData([FromQuery] StationTicketGridSearchModel searchModel)
        {
            var searchContext = new StationTicketSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                ProjectId = searchModel.ProjectId,
                CreatedBy = searchModel.CreatedBy,
                PriorityId = searchModel.PriorityId,
                StatusId = searchModel.StatusId,
                Assignee = searchModel.Assignee,
                TicketAreaId = searchModel.TicketAreaId,
                TicketProvinceId = searchModel.TicketProvinceId

            };
            if (!string.IsNullOrEmpty(searchModel.StrStartDate))
            {
                searchContext.StartDate = DateTime.ParseExact(searchModel.StrStartDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrFinishDate))
            {
                searchContext.FinishDate = DateTime.ParseExact(searchModel.StrFinishDate, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            var models = _stationTicketService.GetExcelData(searchContext);

            return Ok(new XBaseResult
            {
                data = models,
                success = true,
                totalCount = models.TotalCount
            });
        }
        #endregion

        #region Utitlities
        private async Task UpdateLocalesAsync(Core.Domain.Ticket.Ticket entity, TicketModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Subject, localized.Subject, localized.LanguageId);
            }
        }

        private async void InsertTicket(TicketModel ticketModel)
        {
            var entity = ticketModel.ToEntity();
            entity.CreatedDate = entity.ModifiedDate = DateTime.UtcNow;
            entity.Code = ticketModel.Code;
            entity.StartDate = ticketModel.StartDate;
            entity.FinishDate = ticketModel.FinishDate;
            entity.Deadline = ticketModel.Deadline;
            if (ticketModel.StartDate.HasValue && ticketModel.FinishDate.HasValue)
            {
                var unixTimeStart = ticketModel.StartDate.Value.ToUnixTime();
                var unixTimeFinish = ticketModel.FinishDate.Value.ToUnixTime();
                entity.Duration = unixTimeFinish - unixTimeStart;
            }

            await _ticketService.InsertAsync(entity);

            if (ticketModel.StationTicketModel != null)
            {
                ticketModel.StationTicketModel.TicketId = entity.Id;
                var stationTicketEntity = ticketModel.StationTicketModel.ToEntity();
                await _stationTicketService.InsertAsync(stationTicketEntity);
            }
            
            // Locales
            await UpdateLocalesAsync(entity, ticketModel);
        }

        private async void UpdateTicket(TicketModel ticketModel, Core.Domain.Ticket.Ticket entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.StartDate = ticketModel.StartDate;
            entity.FinishDate = ticketModel.FinishDate;
            entity.Deadline = ticketModel.Deadline;

            entity = ticketModel.ToEntity(entity);
            if (ticketModel.StartDate.HasValue && ticketModel.FinishDate.HasValue)
            {
                var unixTimeStart = ticketModel.StartDate.Value.ToUnixTime();
                var unixTimeFinish = ticketModel.FinishDate.Value.ToUnixTime();
                entity.Duration = unixTimeFinish - unixTimeStart;
            }

            await _ticketService.UpdateAsync(entity);

            if (ticketModel.StationTicketModel != null)
            {
                var stationTicketEntity = await _stationTicketService.GetByTicketIdAsync(entity.Id);
                if (stationTicketEntity != null)
                {
                    stationTicketEntity = ticketModel.StationTicketModel.ToEntity(stationTicketEntity);
                    stationTicketEntity.TicketId = entity.Id;
                    await _stationTicketService.UpdateAsync(stationTicketEntity);
                }
            }            

            // Locales
            await UpdateLocalesAsync(entity, ticketModel);
        }

        private async void UpdataDataTicket(TicketModel model)
        {
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableCategories = _ticketCategoryService.GetMvcListItems(false);
            model.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);
            model.AvailablePriorities = _priorityService.GetMvcListItems(false, model.ProjectId);
            model.AvailableOrganizationUnits = _organizationUnitService.GetMvcListItems(false, model.ProjectId);
            model.AvailableCables = _cableService.GetMvcListItems(false);
            model.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
            model.AvailableAprrovalProgress = _approvalProgressService.GetMvcListItems(false);
            model.AvailableStations = _stationService.GetMvcListItems(false);
            model.AvailableApprovers = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int) ApproverTicket.DeputyGeneralManagerTechnical).ToString(),
                    Text = ApproverTicket.DeputyGeneralManagerTechnical.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int) ApproverTicket.DeputyGeneralManagerNetworkInfrastructor).ToString(),
                    Text = ApproverTicket.DeputyGeneralManagerNetworkInfrastructor.GetEnumDescription()
                }
            };

            var stationTicketEntity = await _stationTicketService.GetByTicketIdAsync(model.Id);
            if (stationTicketEntity != null)
            {
                model.StationTicketModel = stationTicketEntity.ToModel();
                model.StationTicketModel.AvailableTicketAreas = _ticketAreaService.GetMvcListItems(false);
                model.StationTicketModel.AvailableTicketProvinces = _ticketProvinceService.GetMvcListItems(false);
            }
            else
            {
                model.StationTicketModel = new StationTicketModel
                {
                    AvailableTicketAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableTicketProvinces = _ticketProvinceService.GetMvcListItems(false)
                };
            }            

            var comments = _commentService.GetByTicketIdAsync(model.Id);
            if (comments?.Count > 0)
            {
                foreach (var comment in comments)
                {
                    var commentModel = comment.ToModel();
                    commentModel.CreatedDate = comment.CreatedDate;
                    commentModel.StrCreatedDate = comment.CreatedDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");

                    model.Comments.Add(commentModel);
                }
            }

            var files = _fileService.GetByTicketIdAsync(model.Id);
            if (files?.Count > 0)
            {
                foreach (var file in files)
                {
                    var fileModel = file.ToModel();

                    model.Files.Add(fileModel);
                }
            }
        }
        #endregion
    }
}
