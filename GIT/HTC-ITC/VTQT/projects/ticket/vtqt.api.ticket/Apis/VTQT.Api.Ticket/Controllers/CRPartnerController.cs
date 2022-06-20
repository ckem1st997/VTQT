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
    [Route("cr-partner")]
    [ApiController]
    //[XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.CRPartner")]
    public class CRPartnerController : AdminApiController
    {
        #region Fields

        private readonly IKeycloakService _keycloakService;
        private readonly ICRPartnerService _crPartnerService;
        private readonly ICRService _crService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IAutoCodeService _autoCodeService;
        private readonly IUserModelHelper _userModelHelper;
        private readonly ICRCategoryService _crCategoryService;
        private readonly IStatusSerive _statusService;
        private readonly IFileService _fileService;
        private readonly ICommentService _commentService;
        private readonly ISendDiscordHelper _sendDiscordHelper;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public CRPartnerController(
            ICommentService commentService,
            ICRPartnerService crPartnerService,
            ICRService crService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService,
            IUserModelHelper userModelHelper,
            ICRCategoryService CrCategoryService,
            IStatusSerive statusSerive,
            IFileService fileService,
            ISendDiscordHelper sendDiscordHelper,
            IWorkContext workContext,
            IKeycloakService keycloakService
            )
        {
            _commentService = commentService;
            _crPartnerService = crPartnerService;
            _crService = crService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _autoCodeService = autoCodeService;
            _userModelHelper = userModelHelper;
            _statusService = statusSerive;
            _fileService = fileService;
            _crCategoryService = CrCategoryService;
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
        [AppApiAction("Ticket.AppActions.CRPartner.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Khởi tạo đối tượng CRPartner
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.CRPartner.Create")]
        public IActionResult Create(string projectId)
        {
            var model = new CRModel
            {
                AvailableUsers = _userModelHelper.GetMvcListItems(),
                AvailableCRCategores = _crCategoryService.GetMvcListItems(false, projectId),
                AvailableStatus = _statusService.GetMvcListItems(false, projectId)
            };

            // Locales
            AddMvcLocales(_languageService, model.Locales);

            return Ok(new XBaseResult
            {
                data = model
            });
        }

        /// <summary>
        /// Thêm mới CRPartner
        /// </summary>
        /// <param name="crModel"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        //[AppApiAction("Ticket.AppActions.CRPartner.Create")]
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

            InsertPartner(crModel);


            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(crModel.CreatedBy).UserName + " đã thêm mới CR đối tác có mã là: " + crModel.Code);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.CRPartner"))
            });
        }

        /// <summary>
        /// Lấy dữ cr
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.CRPartner.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _crService.GetByIdAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.DoesNotExist"), T("Common.CRPartner"))
                });

            var model = entity.ToModel();
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = entity.ModifiedDate;
            model.StartDate = entity.StartDate;
            model.FinishDate = entity.FinishDate;

            UpdataDataCRPartner(model);

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
        //[AppApiAction("Ticket.AppActions.CRPartner.Edit")]
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

            UpdatePartner(crModel, entity);

            await _sendDiscordHelper.SendMessage(_keycloakService.GetUserById(crModel.CreatedBy).UserName + " đã cập nhật CR đối tác có mã là: " + crModel.Code);
            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.CRPartner"))
            });
        }

        /// <summary>
        /// Lấy chi tiết cr
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        //[AppApiAction("Ticket.AppActions.CRPartner.Details")]
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

            UpdataDataCRPartner(model);

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
        //[AppApiAction("Ticket.AppActions.CRPartner.Deletes")]
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
            var mes = _workContext.UserName + " đã xóa CR đối tác có mã là: ";
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
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> DetailGet([FromQuery] CRPartnerGridSearchModel searchModel)
        {
            var searchContext = new CRPartnerSearchContext
            {
                CrId = searchModel.CrId
            };

            var models = new List<CRPartnerModel>();
            var entities = _crPartnerService.GetByCRPartnerId(searchContext);

            foreach (var e in entities)
            {
                var m = e.ToModel();

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
        /// Lấy danh sách CR và CRPartner
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] CRPartnerGridSearchModel searchModel)
        {
            var searchContext = new CRPartnerSearchContext
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
                OverTimeRegister = searchModel.OverTimeRegister,
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

            var models = await _crPartnerService.Get(searchContext);

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

        private async void InsertPartner(CRModel crModel)
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

            if (crModel.CRPartnerModel != null)
            {
                crModel.CRPartnerModel.CrId = entity.Id;
                var crPartnerModel = crModel.CRPartnerModel;
                var problemEntity = crPartnerModel.ToEntity();
                problemEntity.StartTimeAction = crPartnerModel.StartTimeAction;
                problemEntity.RestoreTimeService = crPartnerModel.RestoreTimeService;
                problemEntity.HourTimeMinus = crPartnerModel.HourTimeMinus;
                await _crPartnerService.InsertAsync(problemEntity);
            }

            if (crModel.Comments?.Count > 0)
            {
                var comments = new List<Comment>();
                foreach (var commentModel in crModel.Comments)
                {
                    var commentEntity = commentModel.ToEntity();
                    commentEntity.CrPartnerId = entity.Id;
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

        private async void UpdatePartner(CRModel crModel, Core.Domain.Ticket.CR entity)
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

            if (crModel.CRPartnerModel != null)
            {
                var crPartner = await _crPartnerService.GetByCRPartnerIdAsync(entity.Id);
                if (crPartner != null)
                {
                    crPartner = crModel.CRPartnerModel.ToEntity(crPartner);
                    crPartner.CrId = entity.Id;
                    var crPartnerModel = crModel.CRPartnerModel;
                    crPartner.StartTimeAction = crPartnerModel.StartTimeAction;
                    crPartner.RestoreTimeService = crPartnerModel.RestoreTimeService;
                    crPartner.HourTimeMinus = crPartnerModel.HourTimeMinus;
                    await _crPartnerService.UpdateAsync(crPartner);
                }
            }

            if (crModel.Comments?.Count > 0)
            {
                var commentUpdates = new List<Comment>();
                var commentInserts = new List<Comment>();
                var commentEntities = _commentService.GetByCRPartnerIdAsync(entity.Id);
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
                        comment.CrPartnerId = entity.Id;
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

        private async void UpdataDataCRPartner(CRModel model)
        {
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableCRCategores = _crCategoryService.GetMvcListItems(false, model.ProjectId);
            model.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);


            var crPartner = await _crPartnerService.GetByCRPartnerIdAsync(model.Id);
            if (crPartner != null)
            {
                model.CRPartnerModel = crPartner.ToModel();
            }

            var comments = _commentService.GetByCRPartnerIdAsync(model.Id);
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