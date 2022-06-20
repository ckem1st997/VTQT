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
    [Route("cr-htc")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.CRHTC")]
    public class CRHTCController : AdminApiController
    {
        #region Fields

        private readonly ICRHTCService _crhtcService;
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
        private readonly IConfirmCRService _confirmCRService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;
        private readonly IKeycloakService _keycloakService;
        private readonly IApprovalProgressService _approvalProgressService;

        #endregion Fields

        #region Ctor

        public CRHTCController(
            ICommentService commentService,
            ICRHTCService crhtcService,
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
            IConfirmCRService confirmCRService,
            ITicketProvinceService ticketProvinceService,
            ISendDiscordHelper sendDiscordHelper,
            IWorkContext workContext,
            IKeycloakService keycloakService,
            IApprovalProgressService approvalProgressService)
        {
            _commentService = commentService;
            _crhtcService = crhtcService;
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
            _confirmCRService = confirmCRService;
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
        [AppApiAction("Ticket.AppActions.CRHTC.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng CR
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
                CRHTCModel = new CRHTCModel
                {
                    AvailableCRAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableCRProvinces = _ticketProvince.GetMvcListItems(false),
                },
                AvailableApprovers = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = ((int)ApproverCR.DeputyGeneralManagerTechnical).ToString(),
                        Text = ApproverCR.DeputyGeneralManagerTechnical.GetEnumDescription()
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
        /// Thêm mới CR
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

            InsertCR(crModel);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(crModel.CreatedBy).UserName + " đã thêm mới CR HTC có mã là: " + crModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.CR"))
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

            UpdataDataCR(model);

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

            UpdateCR(crModel, entity);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(crModel.CreatedBy).UserName + " đã sửa CR HTC có mã là: " + crModel.Code);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.CR"))
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

            UpdataDataCR(model);

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
            var mes = _workContext.UserName + " đã xóa CR HTC có mã là: ";
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
        public async Task<IActionResult> DetailGet([FromQuery] CRHTCGridSearchModel searchModel)
        {
            var searchContext = new CRHTCSearchContext
            {
                CrId = searchModel.CrId
            };

            var models = new List<CRHTCModel>();
            var entities = _crhtcService.GetByCRHTCId(searchContext);
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
        /// Lấy danh sách CR và CRHTC
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CRHTCGridSearchModel searchModel)
        {
            var searchContext = new CRHTCSearchContext
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
                StartTimeAction = searchModel.StartTimeAction,
                RestoreTimeService = searchModel.RestoreTimeService
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

            var models = await _crhtcService.Get(searchContext);

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

        private async void InsertCR(CRModel crModel)
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

            if (crModel.CRHTCModel != null)
            {
                crModel.CRHTCModel.CrId = entity.Id;
                var crHtcModel = crModel.CRHTCModel;
                var problemEntity = crHtcModel.ToEntity();
                problemEntity.StartTimeAction = crHtcModel.StartTimeAction;
                problemEntity.RestoreTimeService = crHtcModel.RestoreTimeService;
                problemEntity.HourTimeMinus = crHtcModel.HourTimeMinus;

                await _crhtcService.InsertAsync(problemEntity);
            }

            if (crModel.ApprovalCR?.Count > 0)
            {
                var approvalCR = new List<ApprovalCR>();
                foreach (var approvalCRModel in crModel.ApprovalCR)
                {
                    var approvalCREntity = approvalCRModel.ToEntity();
                    approvalCREntity.CrId = entity.Id;
                    approvalCR.Add(approvalCREntity);
                }
                await _approvalCRService.InsertsAsync(approvalCR);
            }

            if (crModel.ConfirmCRs?.Count > 0)
            {
                var confirmCR = new List<Core.Domain.Ticket.ConfirmCR>();
                foreach (var confirmCRModel in crModel.ConfirmCRs)
                {
                    var confirmCREntity = confirmCRModel.ToEntity();
                    confirmCREntity.crId = entity.Id;
                    confirmCR.Add(confirmCREntity);
                }
                await _confirmCRService.InsertsAsync(confirmCR);
            }
            if (crModel.Comments?.Count > 0)
            {
                var comments = new List<Comment>();
                foreach (var commentModel in crModel.Comments)
                {
                    var commentEntity = commentModel.ToEntity();
                    commentEntity.CrId = entity.Id;
                    commentEntity.CreatedDate = DateTime.UtcNow;
                    comments.Add(commentEntity);
                }
                await _commentService.InsertsAsync(comments);
            }
            if (crModel.InfrastructorFeeCRs?.Count > 0)
            {
                var infrastructorFeeCRs = new List<InfrastructorFeeCR>();
                foreach (var infrastructorFeeCRModel in crModel.InfrastructorFeeCRs)
                {
                    var infrastructorFeeCREntity = infrastructorFeeCRModel.ToEntity();
                    infrastructorFeeCREntity.CrId = entity.Id;
                    infrastructorFeeCREntity.Code = infrastructorFeeCRModel.Code;
                    if (string.IsNullOrEmpty(infrastructorFeeCRModel.Code))
                    {
                        infrastructorFeeCREntity.Code = await _autoCodeService.GenerateCode(nameof(InfrastructorFeeCR));
                    }
                    infrastructorFeeCRs.Add(infrastructorFeeCREntity);
                }
                await _infrastructorFeeCRService.InsertsAsync(infrastructorFeeCRs);
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

        private async void UpdateCR(CRModel crModel, Core.Domain.Ticket.CR entity)
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

            if (crModel.CRHTCModel != null)
            {
                var crhtc = await _crhtcService.GetByCRIdAsync(entity.Id);
                if (crhtc != null)
                {
                    crhtc = crModel.CRHTCModel.ToEntity(crhtc);
                    crhtc.CrId = entity.Id;
                    var crHtcModel = crModel.CRHTCModel;
                    crhtc.StartTimeAction = crHtcModel.StartTimeAction;
                    crhtc.RestoreTimeService = crHtcModel.RestoreTimeService;
                    crhtc.HourTimeMinus = crHtcModel.HourTimeMinus;

                    await _crhtcService.UpdateAsync(crhtc);
                }
            }

            if (crModel.InfrastructorFeeCRs?.Count > 0)
            {
                var infrastructorFeeCRUpdates = new List<InfrastructorFeeCR>();
                var infrastructorFeeCREntities = _infrastructorFeeCRService.GetByCRIdAsync(entity.Id);
                if (infrastructorFeeCREntities?.Count > 0)
                {
                    await _infrastructorFeeCRService.DeletesAsync(infrastructorFeeCREntities.Select(x => x.Id));
                }

                foreach (var infrastructorFeeCRModel in crModel.InfrastructorFeeCRs)
                {
                    var e = infrastructorFeeCRModel.ToEntity();
                    e.CrId = entity.Id;
                    infrastructorFeeCRUpdates.Add(e);
                }

                await _infrastructorFeeCRService.InsertsAsync(infrastructorFeeCRUpdates);
            }

            if (crModel.ApprovalCR?.Count > 0)
            {
                var approvalCRUpdates = new List<ApprovalCR>();
                var approvalCREntities = _approvalCRService.GetByCRIdAsync(entity.Id);
                if (approvalCREntities?.Count > 0)
                {
                    await _approvalCRService.DeletesAsync(approvalCREntities.Select(x => x.Id));
                }

                foreach (var approvalCRModel in crModel.ApprovalCR)
                {
                    var e = approvalCRModel.ToEntity();
                    e.CrId = entity.Id;
                    approvalCRUpdates.Add(e);
                }

                await _approvalCRService.InsertsAsync(approvalCRUpdates);
            }

            if (crModel.ConfirmCRs?.Count > 0)
            {
                var confirmCRUpdates = new List<Core.Domain.Ticket.ConfirmCR>();
                var confirmCREntities = _confirmCRService.GetByCRIdAsync(entity.Id);
                if (confirmCREntities?.Count > 0)
                {
                    await _confirmCRService.DeletesAsync(confirmCREntities.Select(x => x.Id));
                }

                foreach (var confirmCRModel in crModel.ConfirmCRs)
                {
                    var e = confirmCRModel.ToEntity();
                    e.crId = entity.Id;
                    confirmCRUpdates.Add(e);
                }

                await _confirmCRService.InsertsAsync(confirmCRUpdates);
            }

            if (crModel.Comments?.Count > 0)
            {
                var commentUpdates = new List<Comment>();
                var commentInserts = new List<Comment>();
                var commentEntities = _commentService.GetByCRIdAsync(entity.Id);
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
                        comment.CrId = entity.Id;
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

        private async void UpdataDataCR(CRModel model)
        {
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableCRCategores = _crCategoryService.GetMvcListItems(false, model.ProjectId);
            model.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);
            model.AvailableAprrovalProgress = _approvalProgressService.GetMvcListItems(false);
            model.AvailableApprovers = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int) ApproverCR.DeputyGeneralManagerTechnical).ToString(),
                    Text = ApproverCR.DeputyGeneralManagerTechnical.GetEnumDescription()
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

            var crhtc = await _crhtcService.GetByCRIdAsync(model.Id);
            if (crhtc != null)
            {
                model.CRHTCModel = crhtc.ToModel();
                model.CRHTCModel.AvailableCRAreas = _ticketAreaService.GetMvcListItems(false);
                model.CRHTCModel.AvailableCRProvinces = _ticketProvince.GetMvcListItems(false);
            }
            else
            {
                model.CRHTCModel = new CRHTCModel
                {
                    AvailableCRAreas = _ticketAreaService.GetMvcListItems(false),
                    AvailableCRProvinces = _ticketProvince.GetMvcListItems(false),
                };
            }

            var comments = _commentService.GetByCRIdAsync(model.Id);
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
            var infrastructorFeeCRs = _infrastructorFeeCRService.GetByCRIdAsync(model.Id);
            if (infrastructorFeeCRs?.Count > 0)
            {
                foreach (var infrastructorFeeCR in infrastructorFeeCRs)
                {
                    var infrastructorFeeCRModel = infrastructorFeeCR.ToModel();

                    model.InfrastructorFeeCRs.Add(infrastructorFeeCRModel);
                }
            }
            var approvalCR = _approvalCRService.GetByCRIdAsync(model.Id);
            if (approvalCR?.Count > 0)
            {
                foreach (var approvalCRS in approvalCR)
                {
                    var approvalCRModel = approvalCRS.ToModel();

                    model.ApprovalCR.Add(approvalCRModel);
                }
            }
            var confirmCR = _confirmCRService.GetByCRIdAsync(model.Id);
            if (confirmCR?.Count > 0)
            {
                foreach (var confirmCRS in confirmCR)
                {
                    var confirmCRModel = confirmCRS.ToModel();

                    model.ConfirmCRs.Add(confirmCRModel);
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