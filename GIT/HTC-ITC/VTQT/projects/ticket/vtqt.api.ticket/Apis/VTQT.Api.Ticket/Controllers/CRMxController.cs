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
    [Route("cr-mx")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.CRMx")]
    public class CRMxController : AdminApiController
    {
        #region Fields

        private readonly ICRMxService _crmxService;
        private readonly ICRService _crService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly ICRCategoryService _crCategoryService;
        private readonly IStatusSerive _statusService;
        private readonly ITicketAreaService _ticketAreaService;
        private readonly IFileService _fileService;
        private readonly ITicketProvinceService _ticketProvince;
        private readonly ICommentService _commentService;
        private readonly IInfrastructorFeeCRService _infrastructorFeeCRService;
        private readonly IApprovalCRService _approvalCRService;
        private readonly IApprovalCRMxService _approvalCRMxService;
        private readonly IConfirmCRService _confirmCRService;
        private readonly IConfirmCRMxService _confirmCRMxService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;
        private readonly IKeycloakService _keycloakService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion Fields

        #region Ctor

        public CRMxController(
            ICommentService commentService,
            ICRMxService crmxService,
            ICRService crService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService,
            IUserModelHelper userModelHelper,
            ICRCategoryService CrCategoryService,
            IStatusSerive statusSerive,
            ITicketAreaService ticketAreaService,
            IFileService fileService,
            IInfrastructorFeeCRService infrastructorFeeCRService,
            IApprovalCRService approvalCRService,
            IApprovalCRMxService approvalCRMxService,
            IConfirmCRService confirmCRService,
            IConfirmCRMxService confirmCRMxService,
            ITicketProvinceService ticketProvinceService,
            ISendDiscordHelper sendDiscordHelper,
            IWorkContext workContext,
            IKeycloakService keycloakService,
            IApprovalProgressService approvalProgressService)
        {
            _commentService = commentService;
            _crmxService = crmxService;
            _crService = crService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _autoCodeService = autoCodeService;
            _userModelHelper = userModelHelper;
            _statusService = statusSerive;
            _ticketAreaService = ticketAreaService;
            _fileService = fileService;
            _crCategoryService = CrCategoryService;
            _ticketProvince = ticketProvinceService;
            _infrastructorFeeCRService = infrastructorFeeCRService;
            _approvalCRService = approvalCRService;
            _approvalCRMxService = approvalCRMxService;
            _confirmCRService = confirmCRService;
            _confirmCRMxService = confirmCRMxService;
            _sendDiscordHelper = sendDiscordHelper;
            _workContext = workContext;
            _keycloakService = keycloakService;
            _approvalProgressService = approvalProgressService;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.CRMx.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng CRMx
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        public IActionResult Create(string projectId)
        {
            var model = new CRModel
            {
                AvailableUsers = _userModelHelper.GetMvcListItems(),
                AvailableCRCategores = _crCategoryService.GetMvcListItems(false, projectId),
                AvailableStatus = _statusService.GetMvcListItems(false, projectId),
                AvailableAprrovalProgress = _approvalProgressService.GetMvcListItems(false),
                CRMxModel = new CRMxModel
                {
                    AvailableCRAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableCRProvinces = _ticketProvince.GetMvcListItems(false),
                },
                AvailableApprovers = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ApproverCRMx.DeputyGeneralManagerTechnical).ToString(),
                        Text = ApproverCRMx.DeputyGeneralManagerTechnical.GetEnumDescription()
                    }
                },
                AvailableConfirmCRs = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.Noc).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.Noc.GetEnumDescription()
                    },
                     new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.MS).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.MS.GetEnumDescription()
                    },
                     new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.CS).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.CS.GetEnumDescription()
                    } ,
                     new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.DepartmentNoc).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.DepartmentNoc.GetEnumDescription()
                    },
                      new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.DepartmentMS).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.DepartmentMS.GetEnumDescription()
                    },
                      new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.DepartmentCSKH).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.DepartmentCSKH.GetEnumDescription()
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
        /// Thêm mới CRMx
        /// </summary>
        /// <param name="crModel"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(CRModel crModel)
        {
            crModel.Code = DateTime.Now.ToUnixTime().ToString();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _crService.ExistedAsync(crModel.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.CR.Fields.Code"))
                });

            InsertMx(crModel);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(crModel.CreatedBy).UserName + " đã thêm mới CR xin tác động MX lấy tuyến có mã là: " + crModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.TicketMx"))
            });
        }

        /// <summary>
        /// Lấy dữ cr
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _crService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.CR"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            UpdataDataMx(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật cr
        /// </summary>
        /// <param name="crModel"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(CRModel crModel)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _crService.GetByIdAsync(crModel.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.CR"))
                });

            UpdateMx(crModel, entity);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(crModel.CreatedBy).UserName + " đã sửa CR xin tác động MX lấy tuyến có mã là: " + crModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.CRMx"))
            });
        }

        /// <summary>
        /// Lấy chi tiết cr
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _crService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.CR"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            UpdataDataMx(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Name, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách CR
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
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

            var mes = _workContext.UserName + " đã xóa CR xin tác động MX lấy tuyến có mã là: ";
            foreach (var item in ids)
            {
                var entity = await _crService.GetByIdAsync(item);
                if (entity != null)
                {
                    mes = mes + entity.Code + "  ";
                }
            }

            var res = await _crService.DeletesAsync(ids);
            if (res > 0)
                await _sendDiscordHelper.SendMessage(mes);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.CR"))
            });
        }

        [Route("detail-get")]
        [HttpGet]
        public async Task<IActionResult> DetailGet([FromQuery] CRMxGridSearchModel searchModel)
        {
            var searchContext = new CRMxSearchContext
            {
                CrId = searchModel.CrId
            };

            var models = new List<CRMxModel>();
            var entities = _crmxService.GetByCRMxId(searchContext);
            var ticketAreas = _ticketAreaService.GetAll(true);
            var ticketProvinces = _ticketProvince.GetAll(true);

            foreach (var e in entities)
            {
                var m = e.ToModel();

                if (!string.IsNullOrWhiteSpace(m.CrArea))
                    m.CrArea = ticketAreas.FirstOrDefault(w => w.Id == m.CrArea)?.Name;

                if (!string.IsNullOrWhiteSpace(m.CrProvince))
                    m.CrProvince = ticketProvinces.FirstOrDefault(w => w.Id == m.CrProvince)?.Name;

                models.Add(m);
            }

            return Ok(new XBaseResult
            {
                data = models
            });
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sách CR và CRMx
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CRMxGridSearchModel searchModel)
        {
            var searchContext = new CRMxSearchContext
            {
                Keywords = searchModel.Keywords,
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize,
                ProjectId = searchModel.ProjectId,
                CreatedBy = searchModel.EmployeeId,
                StartDate = searchModel.StartDate,
                FinishDate = searchModel.FinishDate,
                Trangthai = searchModel.Trangthai,
                Vungsuco = searchModel.Vungsuco,
                CategoryId = searchModel.CategoryId,
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

            if (!string.IsNullOrEmpty(searchModel.StrStartTimeAction))
            {
                searchContext.StartTimeAction = DateTime.ParseExact(searchModel.StrStartTimeAction, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            if (!string.IsNullOrEmpty(searchModel.StrRestoreTimeService))
            {
                searchContext.RestoreTimeService = DateTime.ParseExact(searchModel.StrRestoreTimeService, "s",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.AdjustToUniversal);
            }

            var models = await _crmxService.Get(searchContext);

            return Ok(new XBaseResult
            {
                data = models,
                success = true,
                totalCount = models.TotalCount
            });
        }

        #endregion List

        #region Utitlities

        private async Task UpdateLocalesAsync(Core.Domain.Ticket.CR entity, CRModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        private async void InsertMx(CRModel crModel)
        {
            var entity = crModel.ToEntity();
            entity.CreatedDate = entity.ModifiedDate = DateTime.UtcNow;
            entity.Code = crModel.Code;
            entity.StartDate = crModel.StartDate;
            entity.FinishDate = crModel.FinishDate;

            //total time
            var unixTimeStart = crModel.StartDate.ToUnixTime();
            var unixTimeFinish = crModel.FinishDate.Value.ToUnixTime();
            entity.TotalTime = unixTimeFinish - unixTimeStart;

            await _crService.InsertAsync(entity);

            if (crModel.CRMxModel != null)
            {
                crModel.CRMxModel.CrId = entity.Id;
                var crMxModel = crModel.CRMxModel;
                var problemEntity = crMxModel.ToEntity();
                problemEntity.StartTimeAction = crMxModel.StartTimeAction;
                problemEntity.RestoreTimeService = crMxModel.RestoreTimeService;
                problemEntity.HourTimeMinus = crMxModel.HourTimeMinus;
                await _crmxService.InsertAsync(problemEntity);
            }

            if (crModel.ApprovalCRMx?.Count > 0)
            {
                var approvalCRMx = new List<ApprovalCRMx>();
                foreach (var approvalCRMxModel in crModel.ApprovalCRMx)
                {
                    var approvalCRMxEntity = approvalCRMxModel.ToEntity();
                    approvalCRMxEntity.CrMxId = entity.Id;
                    approvalCRMx.Add(approvalCRMxEntity);
                }
                await _approvalCRMxService.InsertsAsync(approvalCRMx);
            }

            if (crModel.ConfirmCRMxs?.Count > 0)
            {
                var confirmCRMx = new List<Core.Domain.Ticket.ConfirmCRMx>();
                foreach (var confirmCRMxModel in crModel.ConfirmCRMxs)
                {
                    var confirmCRMxEntity = confirmCRMxModel.ToEntity();
                    confirmCRMxEntity.crMxId = entity.Id;
                    confirmCRMx.Add(confirmCRMxEntity);
                }
                await _confirmCRMxService.InsertsAsync(confirmCRMx);
            }
            if (crModel.Comments?.Count > 0)
            {
                var comments = new List<Comment>();
                foreach (var commentModel in crModel.Comments)
                {
                    var commentEntity = commentModel.ToEntity();
                    commentEntity.CrMxId = entity.Id;
                    commentEntity.CreatedDate = DateTime.UtcNow;
                    comments.Add(commentEntity);
                }
                await _commentService.InsertsAsync(comments);
            }

            if (crModel.Files?.Count > 0)
            {
                var files = new List<File>();
                foreach (var fileModel in crModel.Files)
                {
                    var fileEntity = fileModel.ToEntity();
                    fileEntity.CrId = entity.Id;
                    files.Add(fileEntity);
                }
                await _fileService.InsertsAsync(files);
            }

            // Locales
            await UpdateLocalesAsync(entity, crModel);
        }

        private async void UpdateMx(CRModel crModel, Core.Domain.Ticket.CR entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.StartDate = crModel.StartDate;
            entity.FinishDate = crModel.FinishDate;
            entity.CreatedDate = crModel.CreatedDate;

            entity = crModel.ToEntity(entity);
            //total time
            var unixTimeStart = crModel.StartDate.ToUnixTime();
            var unixTimeFinish = crModel.FinishDate.Value.ToUnixTime();
            entity.TotalTime = unixTimeFinish - unixTimeStart;

            await _crService.UpdateAsync(entity);

            if (crModel.CRMxModel != null)
            {
                var crMx = await _crmxService.GetByCRMxIdAsync(entity.Id);
                if (crMx != null)
                {
                    crMx = crModel.CRMxModel.ToEntity(crMx);
                    crMx.CrId = entity.Id;
                    var crMxModel = crModel.CRMxModel;
                    crMx.StartTimeAction = crMxModel.StartTimeAction;
                    crMx.RestoreTimeService = crMxModel.RestoreTimeService;
                    crMx.HourTimeMinus = crMxModel.HourTimeMinus;
                    await _crmxService.UpdateAsync(crMx);
                }
            }

            if (crModel.ApprovalCRMx?.Count > 0)
            {
                var approvalCRMxUpdates = new List<ApprovalCRMx>();
                var approvalCRMxEntities = _approvalCRMxService.GetByCRMxIdAsync(entity.Id);
                if (approvalCRMxEntities?.Count > 0)
                {
                    await _approvalCRMxService.DeletesAsync(approvalCRMxEntities.Select(x => x.Id));
                }

                foreach (var approvalCRMxModel in crModel.ApprovalCRMx)
                {
                    var e = approvalCRMxModel.ToEntity();
                    e.CrMxId = entity.Id;
                    approvalCRMxUpdates.Add(e);
                }

                await _approvalCRMxService.InsertsAsync(approvalCRMxUpdates);
            }

            if (crModel.ConfirmCRMxs?.Count > 0)
            {
                var confirmCRMxUpdates = new List<Core.Domain.Ticket.ConfirmCRMx>();
                var confirmCRMxEntities = _confirmCRMxService.GetByCRMxIdAsync(entity.Id);
                if (confirmCRMxEntities?.Count > 0)
                {
                    await _confirmCRMxService.DeletesAsync(confirmCRMxEntities.Select(x => x.Id));
                }

                foreach (var confirmCRMxModel in crModel.ConfirmCRMxs)
                {
                    var e = confirmCRMxModel.ToEntity();
                    e.crMxId = entity.Id;
                    confirmCRMxUpdates.Add(e);
                }

                await _confirmCRMxService.InsertsAsync(confirmCRMxUpdates);
            }

            if (crModel.Comments?.Count > 0)
            {
                var commentUpdates = new List<Comment>();
                var commentInserts = new List<Comment>();
                var commentEntities = _commentService.GetByCRMxIdAsync(entity.Id);
                foreach (var commentModel in crModel.Comments)
                {
                    if (commentEntities?.Select(x => x.Id)?.Contains(commentModel.Id) ?? false)
                    {
                        var comment = commentEntities.FirstOrDefault(x => x.Id == commentModel.Id);
                        comment = commentModel.ToEntity(comment);

                        commentUpdates.Add(comment);
                    }
                    else
                    {
                        var comment = commentModel.ToEntity();
                        comment.CrMxId = entity.Id;
                        comment.CreatedDate = DateTime.UtcNow;
                        commentInserts.Add(comment);
                    }
                }
                await _commentService.UpdatesAsync(commentUpdates);
                await _commentService.InsertsAsync(commentInserts);
            }

            if (crModel.Files?.Count > 0)
            {
                var fileUpdates = new List<File>();
                var fileInserts = new List<File>();
                var fileEntities = _fileService.GetByCRIdAsync(entity.Id);
                foreach (var fileModel in crModel.Files)
                {
                    if (fileEntities?.Select(x => x.Id)?.Contains(fileModel.Id) ?? false)
                    {
                        var file = fileEntities.FirstOrDefault(x => x.Id == fileModel.Id);
                        file = fileModel.ToEntity(file);

                        fileUpdates.Add(file);
                    }
                    else
                    {
                        var file = fileModel.ToEntity();
                        file.CrId = entity.Id;

                        fileInserts.Add(file);
                    }
                }
                await _fileService.UpdatesAsync(fileUpdates);
                await _fileService.InsertsAsync(fileInserts);
            }

            // Locales
            await UpdateLocalesAsync(entity, crModel);
        }

        private async void UpdataDataMx(CRModel model)
        {
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableCRCategores = _crCategoryService.GetMvcListItems(false, model.ProjectId);
            model.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);
            model.AvailableAprrovalProgress = _approvalProgressService.GetMvcListItems(false);
            model.AvailableApprovers = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int) ApproverCRMx.DeputyGeneralManagerTechnical).ToString(),
                    Text = ApproverCRMx.DeputyGeneralManagerTechnical.GetEnumDescription()
                }
            };
            model.AvailableConfirmCRs = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int) Core.Domain.Ticket.Enum.ConfirmCR.Noc).ToString(),
                    Text = Core.Domain.Ticket.Enum.ConfirmCR.Noc.GetEnumDescription()
                },
                new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.MS).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.MS.GetEnumDescription()
                    },
                     new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.CS).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.CS.GetEnumDescription()
                    },
                     new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.DepartmentNoc).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.DepartmentNoc.GetEnumDescription()
                    },
                      new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.DepartmentMS).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.DepartmentMS.GetEnumDescription()
                    },
                      new SelectListItem
                    {
                        Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.DepartmentCSKH).ToString(),
                        Text = Core.Domain.Ticket.Enum.ConfirmCR.DepartmentCSKH.GetEnumDescription()
                    }
            };

            var crMx = await _crmxService.GetByCRMxIdAsync(model.Id);
            if (crMx != null)
            {
                model.CRMxModel = crMx.ToModel();
                model.CRMxModel.AvailableCRAreas = _ticketAreaService.GetMvcListItems(false);
                model.CRMxModel.AvailableCRProvinces = _ticketProvince.GetMvcListItems(false);
            }
            else
            {
                model.CRMxModel = new CRMxModel
                {
                    AvailableCRAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableCRProvinces = _ticketProvince.GetMvcListItems(false),
                };
            }

            var comments = _commentService.GetByCRMxIdAsync(model.Id);
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

            var approvalCRMx = _approvalCRMxService.GetByCRMxIdAsync(model.Id);
            if (approvalCRMx?.Count > 0)
            {
                foreach (var approvalCRMxS in approvalCRMx)
                {
                    var approvalCRMxModel = approvalCRMxS.ToModel();

                    model.ApprovalCRMx.Add(approvalCRMxModel);
                }
            }
            var confirmCRMx = _confirmCRMxService.GetByCRMxIdAsync(model.Id);
            if (confirmCRMx?.Count > 0)
            {
                foreach (var confirmCRMxS in confirmCRMx)
                {
                    var confirmCRMxModel = confirmCRMxS.ToModel();

                    model.ConfirmCRMxs.Add(confirmCRMxModel);
                }
            }
            var files = _fileService.GetByCRIdAsync(model.Id);
            if (files?.Count > 0)
            {
                foreach (var file in files)
                {
                    var fileModel = file.ToModel();

                    model.Files.Add(fileModel);
                }
            }
        }

        #endregion Utitlities
    }
}