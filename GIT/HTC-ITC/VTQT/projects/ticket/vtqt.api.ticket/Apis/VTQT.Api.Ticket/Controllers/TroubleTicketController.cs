using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
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

namespace VTQT.Api.Ticket.Controllers
{
    [Route("trouble-ticket")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.TroubleTicket")]
    public class TroubleTicketController : AdminApiController
    {
        #region Fields
        private readonly ITroubleTicketService _troubleTicketService;
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
        private readonly ITicketProgressService _ticketProgressService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;
        private readonly IKeycloakService _keycloakService;
        #endregion

        #region Ctor
        public TroubleTicketController(
            ITroubleTicketService troubleTicketService,
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
            ITicketProgressService ticketProgressService,
            ISendDiscordHelper sendDiscordHelper,
            IWorkContext workContext,
            IKeycloakService keycloakService)
        {
            _troubleTicketService = troubleTicketService;
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
            _ticketProgressService = ticketProgressService;
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
        [AppApiAction("Ticket.AppActions.TroubleTicket.Index")]
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
        //[AppApiAction("Ticket.AppActions.TroubleTicket.Create")]
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
                AvailableProcessingUnits = _organizationUnitService.GetMvcListChildren(false, projectId),
                TroubleTicketModel = new TroubleTicketModel
                {
                    AvailableTicketAreas = _ticketAreaService.GetMvcListItems(false),                    
                    AvailableTicketProvinces = _ticketProvinceService.GetMvcListItems(false),
                    AvailableOrganizationUnits = _organizationUnitService.GetMvcListItems(false, projectId),
                    AvailableTicketProgress = _ticketProgressService.GetMvcListItems(false),
                    AvailableProcessingUnits = _organizationUnitService.GetMvcListChildren(false, projectId)
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
        //[AppApiAction("Ticket.AppActions.TroubleTicket.Create")]
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
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(ticketModel.CreatedBy).UserName + " đã thêm mới ticket vấn đề có mã là: " + ticketModel.Code);
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
        //[AppApiAction("Ticket.AppActions.TroubleTicket.Edit")]
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
        //[AppApiAction("Ticket.AppActions.TroubleTicket.Edit")]
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
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(ticketModel.CreatedBy).UserName + " đã sửa ticket vấn đề có mã là: " + ticketModel.Code);
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
        //[AppApiAction("Ticket.AppActions.TroubleTicket.Details")]
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
        //[AppApiAction("Ticket.AppActions.TroubleTicket.Deletes")]
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
            var mes = _workContext.UserName + " đã xóa ticket vấn đề có mã là: ";
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
        public async Task<IActionResult> Get([FromQuery] TroubleTicketGridSearchModel searchModel)
        {
            var searchContext = new TroubleTicketSearchContext
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

            var models = _troubleTicketService.Get(searchContext);

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
        public async Task<IActionResult> GetExcelData([FromQuery] TroubleTicketGridSearchModel searchModel)
        {
            var searchContext = new TroubleTicketSearchContext
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

            var models = _troubleTicketService.GetExcelData(searchContext);

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

            if (ticketModel.TroubleTicketModel != null)
            {
                ticketModel.TroubleTicketModel.TicketId = entity.Id;
                var troubleTicketEntity = ticketModel.TroubleTicketModel.ToEntity();
                await _troubleTicketService.InsertAsync(troubleTicketEntity);
            }

            if (ticketModel.ApprovalTickets?.Count > 0)
            {
                var approvalTickets = new List<ApprovalTicket>();
                foreach (var approvalTicketModel in ticketModel.ApprovalTickets)
                {
                    var approvalTicketEntity = approvalTicketModel.ToEntity();
                    approvalTicketEntity.TicketId = entity.Id;
                    approvalTickets.Add(approvalTicketEntity);
                }
                await _approvalTicketService.InsertsAsync(approvalTickets);
            }

            if (ticketModel.NetworkLinkTickets?.Count > 0)
            {
                var networkLinkTickets = new List<NetworkLinkTicket>();
                foreach (var networkLinkTicketModel in ticketModel.NetworkLinkTickets)
                {
                    var networkLink = await _networkLinkService.GetByIdAsync(networkLinkTicketModel.NetworkLinkId);
                    var networkLinkTicketEntity = networkLinkTicketModel.ToEntity();
                    networkLinkTicketEntity.TicketId = entity.Id;
                    networkLinkTicketEntity.StartDate = networkLinkTicketModel.StartDate;
                    networkLinkTicketEntity.FinishDate = networkLinkTicketModel.FinishDate;
                    networkLinkTicketEntity.NetworkLinkName = networkLink.Name;
                    networkLinkTickets.Add(networkLinkTicketEntity);
                }
                await _networkLinkTicketService.InsertsAsync(networkLinkTickets);
            }

            if (ticketModel.ChannelTickets?.Count > 0)
            {
                var channelTickets = new List<ChannelTicket>();
                foreach (var channelTicketModel in ticketModel.ChannelTickets)
                {
                    var channel = await _channelService.GetByIdAsync(channelTicketModel.ChannelId);
                    var channelTicketEntity = channelTicketModel.ToEntity();
                    channelTicketEntity.TicketId = entity.Id;
                    channelTicketEntity.StartDate = channelTicketModel.StartDate;
                    channelTicketEntity.FinishDate = channelTicketModel.FinishDate;
                    channelTicketEntity.ChannelName = channel.Name;
                    channelTickets.Add(channelTicketEntity);
                }
                await _channelTicketService.InsertsAsync(channelTickets);
            }

            if (ticketModel.DeviceTickets?.Count > 0)
            {
                var deviceTickets = new List<DeviceTicket>();
                foreach (var deviceTicketModel in ticketModel.DeviceTickets)
                {
                    var device = await _deviceService.GetByIdAsync(deviceTicketModel.DeviceId);
                    var deviceTicketEntity = deviceTicketModel.ToEntity();
                    deviceTicketEntity.TicketId = entity.Id;
                    deviceTicketEntity.StartDate = deviceTicketModel.StartDate;
                    deviceTicketEntity.FinishDate = deviceTicketModel.FinishDate;
                    deviceTicketEntity.DeviceName = device.Name;
                    deviceTickets.Add(deviceTicketEntity);
                }
                await _deviceTicketService.InsertsAsync(deviceTickets);
            }

            if (ticketModel.InfrastructorFees?.Count > 0)
            {
                var infrastructorFees = new List<InfrastructorFee>();
                foreach (var infrastructorFeeModel in ticketModel.InfrastructorFees)
                {
                    var infrastructorFeeEntity = infrastructorFeeModel.ToEntity();
                    infrastructorFeeEntity.TicketId = entity.Id;
                    infrastructorFeeEntity.Code = infrastructorFeeModel.Code;
                    if (string.IsNullOrEmpty(infrastructorFeeModel.Code))
                    {
                        infrastructorFeeEntity.Code = await _autoCodeService.GenerateCode(nameof(InfrastructorFee));
                    }
                    infrastructorFees.Add(infrastructorFeeEntity);
                }
                await _infrastructorFeeService.InsertsAsync(infrastructorFees);
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

            if (ticketModel.TroubleTicketModel != null)
            {
                var troubleTicketEntity = await _troubleTicketService.GetByTicketIdAsync(entity.Id);
                if (troubleTicketEntity != null)
                {
                    troubleTicketEntity = ticketModel.TroubleTicketModel.ToEntity(troubleTicketEntity);
                    troubleTicketEntity.TicketId = entity.Id;
                    await _troubleTicketService.UpdateAsync(troubleTicketEntity);
                }
            }

            if (ticketModel.ApprovalTickets?.Count > 0)
            {
                var approvalTicketUpdates = new List<ApprovalTicket>();
                var approvalTicketEntities = _approvalTicketService.GetByTicketIdAsync(entity.Id);
                if (approvalTicketEntities?.Count > 0)
                {
                    await _approvalTicketService.DeletesAsync(approvalTicketEntities.Select(x => x.Id));
                }

                foreach (var approvalTicketModel in ticketModel.ApprovalTickets)
                {
                    var e = approvalTicketModel.ToEntity();
                    e.TicketId = entity.Id;
                    approvalTicketUpdates.Add(e);
                }

                await _approvalTicketService.InsertsAsync(approvalTicketUpdates);
            }

            if (ticketModel.NetworkLinkTickets?.Count > 0)
            {
                var networkLinkTicketUpdates = new List<NetworkLinkTicket>();
                var networkLinkTicketEntities = _networkLinkTicketService.GetByTicketIdAsync(entity.Id);
                if (networkLinkTicketEntities?.Count > 0)
                {
                    await _networkLinkTicketService.DeletesAsync(networkLinkTicketEntities.Select(x => x.Id));
                }

                foreach (var networkLinkTicketModel in ticketModel.NetworkLinkTickets)
                {
                    var e = networkLinkTicketModel.ToEntity();
                    e.StartDate = networkLinkTicketModel.StartDate;
                    e.FinishDate = networkLinkTicketModel.FinishDate;
                    e.TicketId = entity.Id;
                    networkLinkTicketUpdates.Add(e);
                }

                await _networkLinkTicketService.InsertsAsync(networkLinkTicketUpdates);
            }

            if (ticketModel.ChannelTickets?.Count > 0)
            {
                var channelTicketUpdates = new List<ChannelTicket>();
                var channelTicketEntities = _channelTicketService.GetByTicketIdAsync(entity.Id);
                if (channelTicketEntities?.Count > 0)
                {
                    await _channelTicketService.DeletesAsync(channelTicketEntities.Select(x => x.Id));
                }

                foreach (var channelTicketModel in ticketModel.ChannelTickets)
                {
                    var e = channelTicketModel.ToEntity();
                    e.StartDate = channelTicketModel.StartDate;
                    e.FinishDate = channelTicketModel.FinishDate;
                    e.TicketId = entity.Id;
                    channelTicketUpdates.Add(e);
                }

                await _channelTicketService.InsertsAsync(channelTicketUpdates);
            }

            if (ticketModel.DeviceTickets?.Count > 0)
            {
                var deviceTicketUpdates = new List<DeviceTicket>();
                var deviceTicketEntities = _deviceTicketService.GetByTicketIdAsync(entity.Id);
                if (deviceTicketEntities?.Count > 0)
                {
                    await _deviceTicketService.DeletesAsync(deviceTicketEntities.Select(x => x.Id));
                }

                foreach (var deviceTicketModel in ticketModel.DeviceTickets)
                {
                    var e = deviceTicketModel.ToEntity();
                    e.StartDate = deviceTicketModel.StartDate;
                    e.FinishDate = deviceTicketModel.FinishDate;
                    e.TicketId = entity.Id;
                    deviceTicketUpdates.Add(e);
                }

                await _deviceTicketService.InsertsAsync(deviceTicketUpdates);
            }

            if (ticketModel.InfrastructorFees?.Count > 0)
            {
                var infrastructorFeeUpdates = new List<InfrastructorFee>();
                var infrastructorFeeEntities = _infrastructorFeeService.GetByTicketIdAsync(entity.Id);
                if (infrastructorFeeEntities?.Count > 0)
                {
                    await _infrastructorFeeService.DeletesAsync(infrastructorFeeEntities.Select(x => x.Id));
                }

                foreach (var infrastructorFeeModel in ticketModel.InfrastructorFees)
                {
                    var e = infrastructorFeeModel.ToEntity();
                    e.TicketId = entity.Id;
                    infrastructorFeeUpdates.Add(e);
                }

                await _infrastructorFeeService.InsertsAsync(infrastructorFeeUpdates);
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
            model.AvailableProcessingUnits = _organizationUnitService.GetMvcListChildren(false, model.ProjectId);
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

            var troubleTicketEntity = await _troubleTicketService.GetByTicketIdAsync(model.Id);
            if (troubleTicketEntity != null)
            {
                model.TroubleTicketModel = troubleTicketEntity.ToModel();
                model.TroubleTicketModel.AvailableTicketAreas = _ticketAreaService.GetMvcListItems(false);
                model.TroubleTicketModel.AvailableTicketProvinces = _ticketProvinceService.GetMvcListItems(false);
                model.TroubleTicketModel.AvailableTicketProgress = _ticketProgressService.GetMvcListItems(false);
                model.TroubleTicketModel.AvailableOrganizationUnits = _organizationUnitService.GetMvcListItems(false, model.ProjectId);
                model.TroubleTicketModel.AvailableProcessingUnits = _organizationUnitService.GetMvcListChildren(false, model.ProjectId);
            }
            else
            {
                model.TroubleTicketModel = new TroubleTicketModel
                {
                    AvailableTicketAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableTicketProvinces = _ticketProvinceService.GetMvcListItems(false),
                    AvailableTicketProgress = _ticketProgressService.GetMvcListItems(false),
                    AvailableOrganizationUnits = _organizationUnitService.GetMvcListItems(false, model.ProjectId),
                    AvailableProcessingUnits = _organizationUnitService.GetMvcListChildren(false, model.ProjectId)
                };
            }

            var networkLinkTickets = _networkLinkTicketService.GetByTicketIdAsync(model.Id);
            if (networkLinkTickets?.Count > 0)
            {
                foreach (var networkLinkTicket in networkLinkTickets)
                {
                    var networkLinkTicketModel = networkLinkTicket.ToModel();
                    networkLinkTicketModel.StartDate = networkLinkTicket.StartDate;
                    networkLinkTicketModel.FinishDate = networkLinkTicket.FinishDate;
                    networkLinkTicketModel.AvailableCables = _cableService.GetMvcListItems(false);
                    networkLinkTicketModel.AvailableNetworkLinks = _networkLinkService.GetMvcListItems(false);
                    networkLinkTicketModel.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
                    model.NetworkLinkTickets.Add(networkLinkTicketModel);
                }
            }

            var channelTickets = _channelTicketService.GetByTicketIdAsync(model.Id);
            if (channelTickets?.Count > 0)
            {
                foreach (var channelTicket in channelTickets)
                {
                    var channelTicketModel = channelTicket.ToModel();
                    channelTicketModel.StartDate = channelTicket.StartDate;
                    channelTicketModel.FinishDate = channelTicket.FinishDate;
                    channelTicketModel.AvailableCables = _cableService.GetMvcListItems(false);
                    channelTicketModel.AvailableChannels = _channelService.GetMvcListItems(false);
                    channelTicketModel.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
                    model.ChannelTickets.Add(channelTicketModel);
                }
            }

            var deviceTickets = _deviceTicketService.GetByTicketIdAsync(model.Id);
            if (deviceTickets?.Count > 0)
            {
                foreach (var deviceTicket in deviceTickets)
                {
                    var deviceTicketModel = deviceTicket.ToModel();
                    deviceTicketModel.StartDate = deviceTicket.StartDate;
                    deviceTicketModel.FinishDate = deviceTicket.FinishDate;
                    deviceTicketModel.AvailableDevices = _deviceService.GetMvcListItems(false);
                    deviceTicketModel.AvailablePhenomenas = _phenomenaService.GetMvcListItems(false);
                    model.DeviceTickets.Add(deviceTicketModel);
                }
            }

            var infrastructorFees = _infrastructorFeeService.GetByTicketIdAsync(model.Id);
            if (infrastructorFees?.Count > 0)
            {
                foreach (var infrastructorFee in infrastructorFees)
                {
                    var infrastructorFeeModel = infrastructorFee.ToModel();

                    model.InfrastructorFees.Add(infrastructorFeeModel);
                }
            }

            var approvalTickets = _approvalTicketService.GetByTicketIdAsync(model.Id);
            if (approvalTickets?.Count > 0)
            {
                foreach (var approvalTicket in approvalTickets)
                {
                    var approvalTicketModel = approvalTicket.ToModel();

                    model.ApprovalTickets.Add(approvalTicketModel);
                }
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
