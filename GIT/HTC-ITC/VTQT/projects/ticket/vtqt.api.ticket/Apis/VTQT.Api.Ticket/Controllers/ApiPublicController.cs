using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VTQT.Core;
using VTQT.Core.Domain.Ticket;
using VTQT.Services.Localization;
using VTQT.Services.Master;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Helpers;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Ticket.Controllers
{
    [AllowAnonymous]
    [Route("api-public")]
    [ApiController]
    public class ApiPublicController : AdminApiController
    {
        #region Fields

        private readonly IParitcularFtthService _paritcularFtthService;
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

        #endregion Fields

        #region Ctor

        public ApiPublicController(
            IParitcularFtthService paritcularFtthService,
            IFtthService ftthService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IAutoCodeService autoCodeService,
            IUserModelHelper userModelHelper,
            IStatusSerive statusSerive,
            ITicketReasonService ticketReasonService,
            IFileService fileService,
            IPhenomenaService phenomenaService,
            ICommentService commentService)
        {
            _paritcularFtthService = paritcularFtthService;
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
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("create-paritcular-ftth")]
        [HttpGet]
        public IActionResult Create()
        {
            // id to Ftth-Sự cố lẻ
            var projectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            var model = new FtthModel
            {
                AvailableUsers = _userModelHelper.GetMvcListItems(),
                AvailableStatus = _statusService.GetMvcListItems(false, projectId),
                ParitcularFtthModel = new ParitcularFtthModel
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
                data = model,
                success = model.StatusModel != null
            });
        }


        /// <summary>
        /// Lấy dữ ftth
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit-paritcular-ftth")]
        [HttpGet]
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

            UpdataDataFTTH(model);

            // Locales
            AddMvcLocales(_languageService, model.Locales,
                (locale, languageId) =>
                {
                    locale.Name = entity.GetLocalized(x => x.Subject, languageId, false, false);
                });

            return Ok(new XBaseResult
            {
                data = model,
                success = model.StatusModel != null
            });
        }


        /// <summary>
        /// Thêm mới Ftth
        /// </summary>
        /// <param name="ftthModel"></param>
        /// <returns></returns>
        [Route("create-paritcular-ftth")]
        [HttpPost]
        public async Task<IActionResult> Create(FtthModel ftthModel)
        {
            ftthModel.ParitcularFtthModel = new ParitcularFtthModel();
            ftthModel.Code = DateTime.UtcNow.ToUnixTime().ToString();
            ftthModel.Id = Guid.NewGuid().ToString();
            ftthModel.CreatedBy = "755d4f67-5a5b-4e0f-8990-ebf7cdc5af75";
            ftthModel.ModifiedBy = "755d4f67-5a5b-4e0f-8990-ebf7cdc5af75";
            ftthModel.CreatedDate = DateTime.UtcNow;
            ftthModel.ModifiedDate = DateTime.UtcNow;
            ftthModel.StartDate = DateTime.UtcNow;
            ftthModel.FinishDate = DateTime.UtcNow;
            ftthModel.ParitcularFtthModel.ReasonId = "01243983-e014-4070-9f08-43b52202f7f6";
            ftthModel.ParitcularFtthModel.PhenomenaId = "a4e26f62-bb51-4c6f-8b5d-6183679a8652";
            ftthModel.ParitcularFtthModel.StatusId = "1A59BF65-6B06-4DD5-B607-9D7A5AD08363";
            ftthModel.SlaStartTime = DateTime.UtcNow;
            ftthModel.ParitcularFtthModel.Treatment="";
            ftthModel.ParitcularFtthModel.DetailReason="";
            if (!ModelState.IsValid)
                return InvalidModelResult();

            if (await _ftthService.ExistedAsync(ftthModel.Code))
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(T("Common.Notify.AlreadyExist"), T("Ticket.FTTH.Fields.Code"))
                });
            var model = await _statusService.GetByIdAsync(ftthModel.ParitcularFtthModel.StatusId);
            ftthModel.StatusModel.Add(model.ToModel());
            ftthModel.ProjectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            foreach (var item in ftthModel.StatusModel)
            {
                item.ProjectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            }

            await InsertFtth(ftthModel);
            var entity = _ftthService.GetById(ftthModel.Id);

            return Ok(new XBaseResult
            {
                message = entity != null
                    ? "Thêm thành công. Mã của phiếu là: " + entity.Id
                    : "Thêm thất bại, xin vui lòng thử lại !",
                success = entity != null
            });
        }


        /// <summary>
        /// Cập nhật ftth
        /// </summary>
        /// <param name="ftthModel"></param>
        /// <returns></returns>
        /// <response code="200">Updated successfully</response>
        [Route("edit-paritcular-ftth")]
        [HttpPost]
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
            var paritcularFtth = await _paritcularFtthService.GetByFtthIdAsync(ftthModel.Id);
            var mode = await _statusService.GetByIdAsync(paritcularFtth.StatusId);
            ftthModel.StatusModel.Add(mode.ToModel());
            ftthModel.ProjectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            foreach (var item in ftthModel.StatusModel)
            {
                item.ProjectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            }

            await UpdateFtth(ftthModel, entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Updated"), T("Common.FTTH"))
            });
        }

        #endregion Methods

        #region Utitlities

        private async Task UpdateLocalesAsync(Core.Domain.Ticket.Ftth entity, FtthModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Subject, localized.Name,
                    localized.LanguageId);
            }
        }

        private async Task InsertFtth(FtthModel ftthModel)
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
            if (ftthModel.ParitcularFtthModel != null)
            {
                ftthModel.ParitcularFtthModel.FtthId = entity.Id;
                var paritcularFtthModel = ftthModel.ParitcularFtthModel;
                var problemEntity = paritcularFtthModel.ToEntity();

                await _paritcularFtthService.InsertAsync(problemEntity);
            }


            // Locales
            await UpdateLocalesAsync(entity, ftthModel);
        }

        private async Task UpdateFtth(FtthModel ftthModel, Core.Domain.Ticket.Ftth entity)
        {
            ftthModel.ModifiedDate = DateTime.UtcNow;
            ftthModel.StartDate = entity.StartDate;
            ftthModel.FinishDate = entity.FinishDate;
            ftthModel.CreatedDate = entity.CreatedDate;
            ftthModel.CreatedBy = entity.CreatedBy;
            ftthModel.ModifiedBy = entity.ModifiedBy;
            ftthModel.SlaStartTime = entity.SlaStartTime;
            ftthModel.FinishDate = entity.FinishDate;
             //total time
            ftthModel.Code = entity.Code;
            entity = ftthModel.ToEntity(entity);
            //total time
            var unixTimeStart = ftthModel.StartDate.ToUnixTime();
            var unixTimeFinish = entity.FinishDate == null ? 0: entity.FinishDate.Value.ToUnixTime();
            entity.TotalTime = unixTimeFinish - unixTimeStart;
            await _ftthService.UpdateAsync(entity);

            if (ftthModel.ParitcularFtthModel != null)
            {
                var paritcularFtth = await _paritcularFtthService.GetByFtthIdAsync(entity.Id);
                if (paritcularFtth != null)
                {
                    paritcularFtth = ftthModel.ParitcularFtthModel.ToEntity(paritcularFtth);
                    paritcularFtth.FtthId = entity.Id;
                    var paritcularFtthModel = ftthModel.ParitcularFtthModel;

                    await _paritcularFtthService.UpdateAsync(paritcularFtth);
                }
            }

            await UpdateLocalesAsync(entity, ftthModel);
        }

        private async void UpdataDataFTTH(FtthModel model)
        {
            model.AvailableUsers = _userModelHelper.GetMvcListItems();
            model.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);

            var paritcularFtth = await _paritcularFtthService.GetByFtthIdAsync(model.Id);
            if (paritcularFtth != null)
            {
                model.ParitcularFtthModel = paritcularFtth.ToModel();
                model.ParitcularFtthModel.AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId);
                model.ParitcularFtthModel.AvailableReason = _ticketReasonService.GetMvcList(false, model.ProjectId);
                model.ParitcularFtthModel.AvailablePhenomena = _phenomenonService.GetMvcListItems(false);
            }
            else
            {
                model.ParitcularFtthModel = new ParitcularFtthModel
                {
                    AvailableStatus = _statusService.GetMvcListItems(false, model.ProjectId),
                    AvailableReason = _ticketReasonService.GetMvcList(false, model.ProjectId),
                    AvailablePhenomena = _phenomenonService.GetMvcListItems(false),
                };
            }
        }

        #endregion Utitlities
    }
}