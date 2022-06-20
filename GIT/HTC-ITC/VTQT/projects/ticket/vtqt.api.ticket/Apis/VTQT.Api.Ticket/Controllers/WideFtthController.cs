using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Security;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("wide-ftth")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.WideFtth")]
    public class WideFtthController : AdminApiController
    {
        #region Fields

        private readonly IWideFtthService _wideFtthService;
        private readonly IFtthService _ftthService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly IStatusSerive _statusService;
        private readonly ITicketReasonService _ticketReasonService;
        private readonly IFileService _fileService;
        private readonly IPhenomenaService _phenomenonService;
        private readonly ICommentService _commentService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;
        private readonly IKeycloakService _keycloakService;

        #endregion Fields

        #region Ctor

        public WideFtthController(
            IWideFtthService wideFtthService,
            IFtthService ftthService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService,
            IUserModelHelper userModelHelper,
            IStatusSerive statusSerive,
            ITicketReasonService ticketReasonService,
            IFileService fileService,
            IPhenomenaService phenomenaService,
            ICommentService commentService,
            ISendDiscordHelper sendDiscordHelper,
            IWorkContext workContext,
            IKeycloakService keycloakService)
        {
            _wideFtthService = wideFtthService;
            _ftthService = ftthService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _autoCodeService = autoCodeService;
            _userModelHelper = userModelHelper;
            _statusService = statusSerive;
            _ticketReasonService = ticketReasonService;
            _fileService = fileService;
            _phenomenonService = phenomenaService;
            _commentService = commentService;
            _sendDiscordHelper = sendDiscordHelper;
            _workContext = workContext;
            _keycloakService = keycloakService;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Hàm khởi tạo Index
        /// </summary>
        /// <returns></returns>
        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.WideFtth.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng Ftth
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.WideFtth.Create")]
        public IActionResult Create(string projectId)
        {
            var model = new FtthModel
            {
                AvailableUsers = _userModelHelper.GetMvcListItems(),
                AvailableStatus = _statusService.GetMvcListItems(false, projectId),
                WideFtthModel = new WideFtthModel
                {
                    AvailableReason = _ticketReasonService.GetMvcList(false, projectId),
                    AvailablePhenomena = _phenomenonService.GetMvcListItems(false),
                    AvailableStatus = _statusService.GetMvcListItems(false, projectId),
                },
            };

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới Ftth
        /// </summary>
        /// <param name="ftthModel"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.WideFtth.Create")]
        public async Task<IActionResult> Create(FtthModel ftthModel)
        {
            ftthModel.Code = DateTime.Now.ToUnixTime().ToString();

            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _ftthService.ExistedAsync(ftthModel.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.FTTH.Fields.Code"))
                });

            InsertWideFtth(ftthModel);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(ftthModel.CreatedBy).UserName + " đã thêm mới sự cố diện rộng có mã là: " + ftthModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.FTTH"))
            });
        }

        /// <summary>
        /// Lấy dữ ftth
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.WideFtth.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _ftthService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            UpdataDataWideFtth(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Subject, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Cập nhật ftth
        /// </summary>
        /// <param name="ftthModel"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.WideFtth.Edit")]
        public async Task<IActionResult> Edit(FtthModel ftthModel)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = await _ftthService.GetByIdAsync(ftthModel.Id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH"))
                });

            UpdateWideFtth(ftthModel, entity);
            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(ftthModel.CreatedBy).UserName + " đã sửa sự cố diện rộng có mã là: " + ftthModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.FTTH"))
            });
        }

        /// <summary>
        /// Lấy chi tiết ftth
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.WideFtth.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var entity = await _ftthService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.FTTH"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            UpdataDataWideFtth(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = entity.GetLocalized(x => x.Subject, languageId, false, false);
            });

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Xóa danh sách FTTH
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <response code="200">Deleted successfully</response>
        [Route("deletes")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.WideFtth.Deletes")]
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
            var mes = _workContext.UserName + " đã xóa sự cố diện rộng có mã là: ";
            foreach (var item in ids)
            {
                var entity = await _ftthService.GetByIdAsync(item);
                if (entity != null)
                {
                    mes = mes + entity.Code + "  ";
                }
            }

            var res = await _ftthService.DeletesAsync(ids);
            if (res > 0)
                await _sendDiscordHelper.SendMessage(mes);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Deleted"), T("Common.FTTH"))
            });
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sách FTTH và WideFtth
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] WideFtthGridSearchModel searchModel)
        {
            var searchContext = new WideFtthSearchContext
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
                RestoreTimeService = searchModel.RestoreTimeService,
                Reason = searchModel.Reason,
                Phenomena = searchModel.Phenomena
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

            var models = await _wideFtthService.Get(searchContext);

            return Ok(new XBaseResult
            {
                data = models,
                success = true,
                totalCount = models.TotalCount
            });
        }

        #endregion List

        #region Utitlities

        private async Task UpdateLocalesAsync(Core.Domain.Ticket.Ftth entity, FtthModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Subject, localized.Name, localized.LanguageId);
            }
        }

        private async void InsertWideFtth(FtthModel ftthModel)
        {
            var entity = ftthModel.ToEntity();
            entity.CreatedDate = entity.ModifiedDate = DateTime.UtcNow;
            entity.Code = ftthModel.Code;
            entity.StartDate = ftthModel.StartDate;
            entity.FinishDate = ftthModel.FinishDate;
            //total time
            var unixTimeStart = ftthModel.StartDate.ToUnixTime();
            var unixTimeFinish = ftthModel.FinishDate.Value.ToUnixTime();
            entity.TotalTime = unixTimeFinish - unixTimeStart;

            await _ftthService.InsertAsync(entity);

            if (ftthModel.WideFtthModel != null)
            {
                ftthModel.WideFtthModel.FtthId = entity.Id;
                var wideFtthModel = ftthModel.WideFtthModel;
                var problemEntity = wideFtthModel.ToEntity();

                await _wideFtthService.InsertAsync(problemEntity);
            }

            if (ftthModel.Comments?.Count > 0)
            {
                var comments = new List<Comment>();
                foreach (var commentModel in ftthModel.Comments)
                {
                    var commentEntity = commentModel.ToEntity();
                    commentEntity.WideFtthId = entity.Id;
                    commentEntity.CreatedDate = DateTime.UtcNow;
                    comments.Add(commentEntity);
                }
                await _commentService.InsertsAsync(comments);
            }
            if (ftthModel.Files?.Count > 0)
            {
                var files = new List<File>();
                foreach (var fileModel in ftthModel.Files)
                {
                    var fileEntity = fileModel.ToEntity();
                    fileEntity.FtthId = entity.Id;
                    files.Add(fileEntity);
                }
                await _fileService.InsertsAsync(files);
            }

            // Locales
            await UpdateLocalesAsync(entity, ftthModel);
        }

        private async void UpdateWideFtth(FtthModel ftthModel, Core.Domain.Ticket.Ftth entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.StartDate = ftthModel.StartDate;
            entity.FinishDate = ftthModel.FinishDate;
            entity.CreatedDate = ftthModel.CreatedDate;

            entity = ftthModel.ToEntity(entity);
            //total time
            var unixTimeStart = ftthModel.StartDate.ToUnixTime();
            var unixTimeFinish = ftthModel.FinishDate.Value.ToUnixTime();
            entity.TotalTime = unixTimeFinish - unixTimeStart;
            await _ftthService.UpdateAsync(entity);

            if (ftthModel.WideFtthModel != null)
            {
                var wideFtth = await _wideFtthService.GetByWideFtthIdAsync(entity.Id);
                if (wideFtth != null)
                {
                    wideFtth = ftthModel.WideFtthModel.ToEntity(wideFtth);
                    wideFtth.FtthId = entity.Id;
                    var wideFtthModel = ftthModel.WideFtthModel;

                    await _wideFtthService.UpdateAsync(wideFtth);
                }
            }

            if (ftthModel.Comments?.Count > 0)
            {
                var commentUpdates = new List<Comment>();
                var commentInserts = new List<Comment>();
                var commentEntities = _commentService.GetByWideFtthIdAsync(entity.Id);
                foreach (var commentModel in ftthModel.Comments)
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
                        comment.WideFtthId = entity.Id;
                        comment.CreatedDate = DateTime.UtcNow;
                        commentInserts.Add(comment);
                    }
                }
                await _commentService.UpdatesAsync(commentUpdates);
                await _commentService.InsertsAsync(commentInserts);
            }

            if (ftthModel.Files?.Count > 0)
            {
                var fileUpdates = new List<File>();
                var fileInserts = new List<File>();
                var fileEntities = _fileService.GetByFTTHIdAsync(entity.Id);
                foreach (var fileModel in ftthModel.Files)
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
                        file.FtthId = entity.Id;

                        fileInserts.Add(file);
                    }
                }
                await _fileService.UpdatesAsync(fileUpdates);
                await _fileService.InsertsAsync(fileInserts);
            }

            // Locales
            await UpdateLocalesAsync(entity, ftthModel);
        }

        private async void UpdataDataWideFtth(FtthModel model)
        {
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);

            var wideFtth = await _wideFtthService.GetByWideFtthIdAsync(model.Id);
            if (wideFtth != null)
            {
                model.WideFtthModel = wideFtth.ToModel();
                model.WideFtthModel.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);
                model.WideFtthModel.AvailableReason = _ticketReasonService.GetMvcList(false, model.ProjectId);
                model.WideFtthModel.AvailablePhenomena = _phenomenonService.GetMvcListItems(false);
            }
            else
            {
                model.WideFtthModel = new WideFtthModel
                {
                    AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId),
                    AvailableReason = _ticketReasonService.GetMvcList(false, model.ProjectId),
                    AvailablePhenomena = _phenomenonService.GetMvcListItems(false),
                };
            }

            var comments = _commentService.GetByWideFtthIdAsync(model.Id);
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

            var files = _fileService.GetByFTTHIdAsync(model.Id);
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