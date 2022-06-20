using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class ParitcularFtthController : AdminMvcController
    {
        #region Fields

        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public ParitcularFtthController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var searchModel = new ParitcularFtthGridSearchModel();
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00007" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<FtthModel>("paritcular-ftth/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            var resEmployees = await ApiHelper.ExecuteAsync<List<SharedMvc.Master.Models.UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            var list = new List<SelectListItem>();
            if (resEmployees.data?.Count > 0)
            {
                foreach (var item in resEmployees.data)
                {
                    var tem = new SelectListItem()
                    {
                        Text = item.FullName + "-" + item.UserName,
                        Value = item.Id
                    };
                    list.Add(tem);
                }
            }

            ViewData["employees"] = list;

            var list1 = new List<SelectListItem>();
            if (resEmployees.data?.Count > 0)
            {
                foreach (var item1 in resEmployees.data)
                {
                    var tem1 = new SelectListItem()
                    {
                        Text = item1.FullName + "-" + item1.UserName,
                        Value = item1.Id
                    };
                    list1.Add(tem1);
                }
            }

            ViewData["assignee"] = list1;

            var resStatus = await ApiHelper.ExecuteAsync<List<StatusModel>>("/status/get-available", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var list3 = new List<SelectListItem>();
            if (resStatus.data?.Count > 0)
            {
                model.ProjectId = projectModel?.Id;
                foreach (var item3 in resStatus.data)
                {
                    var tem3 = new SelectListItem()
                    {
                        Text = item3.Name,
                        Value = item3.Id
                    };
                    list3.Add(tem3);
                }
            }

            ViewData["status"] = list3;

            var resReason = await ApiHelper.ExecuteAsync<List<TicketReasonModel>>("/ticket-reason/get-available", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var list4 = new List<SelectListItem>();
            if (resReason.data?.Count > 0)
            {
                model.ProjectId = projectModel?.Id;
                foreach (var item4 in resReason.data)
                {
                    var tem4 = new SelectListItem()
                    {
                        Text = item4.Description,
                        Value = item4.Id
                    };
                    list4.Add(tem4);
                }
            }

            ViewData["reason"] = list4;

            var resPhenomena = await ApiHelper.ExecuteAsync<List<PhenomenaModel>>("/phenomena/get-available", null, Method.GET, ApiHosts.Ticket);
            var list5 = new List<SelectListItem>();
            if (resPhenomena.data?.Count > 0)
            {
                foreach (var item5 in resPhenomena.data)
                {
                    var tem5 = new SelectListItem()
                    {
                        Text = item5.Name,
                        Value = item5.Id
                    };
                    list5.Add(tem5);
                }
            }

            ViewData["phenomena"] = list5;

            return View(searchModel);
        }

        public async Task<IActionResult> Create()
        {
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00007" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<FtthModel>("paritcular-ftth/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;
            if (model != null)
            {
                model.CreatedBy = _workContext.UserId;
                model.CreatedDate = model.ModifiedDate = DateTime.UtcNow.ToLocalTime();
                model.ProjectId = projectModel?.Id;
                model.StartDate = DateTime.UtcNow.ToLocalTime();
                model.CreatedDate = DateTime.UtcNow.ToLocalTime();
                model.FinishDate = DateTime.UtcNow.ToLocalTime();
                model.SlaStartTime = DateTime.UtcNow.ToLocalTime();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            FtthModel model,
            ParitcularFtthModel paritcularFtthModel)
        {
            model.CreatedDate = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.ParitcularFtthModel = paritcularFtthModel;
            var res = await ApiHelper
                .ExecuteAsync("paritcular-ftth/create", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult
            {
                success = true,
                data = model.Id
            });
        }

        /// <summary>
        /// Gọi Api xóa ftth/ paritcularFtth
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("/paritcular-ftth/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<FtthModel>("paritcular-ftth/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            if (model != null)
            {
                model.ModifiedBy = _workContext.UserId;

                if (model.ParitcularFtthModel != null)
                {
                    model.ParitcularFtthModel.ParitcularFtthId = model.ParitcularFtthModel.Id;
                }

                model.Comments?.Sort();
                model.UserLogin = _workContext.UserId;
                model.StartDate = model.StartDate.ToLocalTime();
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.FinishDate = model.FinishDate.ToLocalTime();
                model.SlaStartTime = model.SlaStartTime.ToLocalTime();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            FtthModel model,
            ParitcularFtthModel paritcularFtthModel)
        {
            model.CreatedDate = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            model.ParitcularFtthModel = paritcularFtthModel;

            var res = await ApiHelper
                .ExecuteAsync("paritcular-ftth/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<FtthModel>("paritcular-ftth/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            if (model != null)
            {
                model.Comments?.Sort();
                model.UserLogin = _workContext.UserId;
                model.StartDate = model.StartDate.ToLocalTime();
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.FinishDate = model.FinishDate.ToLocalTime();
                model.SlaStartTime = model.SlaStartTime.ToLocalTime();
            }

            return View(model);
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sách ftth/ paritcularftth phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            ParitcularFtthGridSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.StrStartDate = searchModel.StartDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrFinishDate = searchModel.FinishDate?
                .ToString("s", CultureInfo.InvariantCulture);

            searchModel.StrStartTimeAction = searchModel.StartTimeAction?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrRestoreTimeService = searchModel.RestoreTimeService?
                .ToString("s", CultureInfo.InvariantCulture);

            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00007" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<ParitcularFtthGridModel>>("/paritcular-ftth/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            if (data?.Count > 0)
            {
                data.ForEach(x =>
                {
                    if (x.Total.ToInt() <= 0) return;
                    var t = TimeSpan.FromSeconds(x.Total.ToInt());
                    x.Total = t.ToString(@"dd") + " ngày - " + t.ToString(@"hh\:mm\:ss");
                });
            }

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion List

        #region Utilities

        [HttpPost]
        public async Task<IActionResult> GetUserComment(string userId, string content, string ftthId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new XBaseResult
                {
                    success = false
                }
                );
            }

            var res = await ApiHelper.ExecuteAsync<UserModel>("user/get-by-id", new { id = userId }, Method.GET,
                ApiHosts.Master);

            var user = res.data;
            var comment = new CommentModel();
            if (user != null)
            {
                comment.UserId = userId;
                comment.UserName = user.FullName;
                comment.CreatedDate = DateTime.UtcNow.ToLocalTime();
                comment.Content = content;
                comment.StrCreatedDate = comment.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                comment.FtthId = ftthId;
            }

            var resCmt =
                await ApiHelper.ExecuteAsync<CommentModel>("comment/create", comment, Method.POST, ApiHosts.Ticket);

            return Ok(new XBaseResult
            {
                success = true,
                data = comment
            });
        }

        /// <summary>
        /// Khởi tạo danh sách dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task GetDropDownList(FtthModel model)
        {
            var availableProject = await ApiHelper
                .ExecuteAsync<List<SharedMvc.Master.Models.ProjectModel>>("/project/get-available", null, Method.GET, ApiHosts.Master);

            if (availableProject?.data?.Count > 0)
            {
                availableProject.data.ForEach(item =>
                {
                    model.AvailableProject
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString(),
                    });
                });
            }
        }

        private async Task GetDropDownListTecology(TechnologyModel model)
        {
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00007" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var resPhenomena = await ApiHelper.ExecuteAsync<List<PhenomenaModel>>("/phenomena/get-available", null, Method.GET, ApiHosts.Ticket);

            if (resPhenomena?.data?.Count > 0)
            {
                resPhenomena.data.ForEach(item =>
                {
                    model.AvailablePhenomena
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id,
                    });
                });
            }

            var resReason = await ApiHelper.ExecuteAsync<List<TicketReasonModel>>("/ticket-reason/get-available", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);

            if (resReason?.data?.Count > 0)
            {
                resReason.data.ForEach(item =>
                {
                    model.AvailableReason
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Description,
                        Value = item.Id,
                    });
                });
            }

            var resStatus = await ApiHelper.ExecuteAsync<List<StatusModel>>("/status/get-available", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);

            if (resStatus?.data?.Count > 0)
            {
                resStatus.data.ForEach(item =>
                {
                    model.AvailableStatus
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id,
                    });
                });
            }
        }

        private async Task GetDropDownListCsReport(CsReportModel model)
        {
           

            var resPhenomena = await ApiHelper.ExecuteAsync<List<RatingModel>>("/rating/get-available", null, Method.GET, ApiHosts.Ticket);

            if (resPhenomena?.data?.Count > 0)
            {
                resPhenomena.data.ForEach(item =>
                {
                    model.AvailableRating
                    .Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id,
                    });
                });
            }
        }

        #endregion Utilities

        #region Files

        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            var files = Request.Form.Files;
            var ftthId = Request.Form["ftthId"];
            var ftthCode = Request.Form["ftthCode"];
            var fileUploads = new List<SharedMvc.Ticket.Models.FileModel>();

            if (files?.Count > 0)
            {
                foreach (var file in files)
                {
                    var dynamicPath = $"wwwroot/Uploads/FTTH/{DateTime.Now.ToString("yyyy/MM/dd")}/{ftthCode}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), dynamicPath);

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var f = new SharedMvc.Ticket.Models.FileModel
                    {
                        FtthId = ftthId,
                        FileName = file.FileName,
                        MimeType = file.ContentType,
                        Size = file.Length,
                        Extension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1),
                        Path = Path.Combine(dynamicPath, file.FileName)
                    };
                    fileUploads.Add(f);
                    string fullPath = Path.Combine(filePath, f.FileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            var res = await ApiHelper.ExecuteAsync("file/create", fileUploads, Method.POST, ApiHosts.Ticket);

            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult
            {
                data = fileUploads,
                success = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string url, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), url);

            if (!string.IsNullOrEmpty(filePath))
            {
                filePath = filePath.Replace("\\", "/");
            }

            try
            {
                //Read the File data into Byte Array.
                byte[] bytes = System.IO.File.ReadAllBytes(filePath);

                //Send the File to Download.
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }

        #endregion Files

        #region Comment

        public async Task<IActionResult> Comment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync<List<CommentModel>>("comment/get-Ftth", new { FtthId = id }, Method.GET, ApiHosts.Ticket);
            var comments = res.data;
            comments.Sort();
            ViewData["ftthId"] = id;
            ViewData["UserLogin"] = _workContext.UserId;

            return View(comments);
        }

        #endregion Comment

        #region CSReceive

        public async Task<IActionResult> AddCSReceive(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<FtthModel>("paritcular-ftth/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);
            var model = res.data;

            await GetDropDownList(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCSReceive(FtthModel ftth)
        {
            ftth.ProjectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("paritcular-ftth/edit", ftth, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region TechnologyReceive

        public async Task<IActionResult> AddTechnologyReceive(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<FtthModel>("paritcular-ftth/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);
            var model = res.data;

            await GetDropDownList(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddTechnologyReceive(FtthModel ftth)
        {
            ftth.ProjectId = "EA4AE47A-EF6D-4B01-853B-2894202BA965";
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("paritcular-ftth/edit", ftth, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region TechnologySucess

        public async Task<IActionResult> AddTechnologySucess(string id)
        {
            var ftth = new TechnologyModel();
            ftth.FtthId = id;
            var res = await ApiHelper.ExecuteAsync<TechnologyModel>("/technology/create", new { id = id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            await GetDropDownListTecology(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddTechnologySucess(TechnologyModel technology)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("technology/create", technology, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region CsReport

        public async Task<IActionResult> AddCsReport(string id)
        {
            var ftth = new CsReportModel();
            ftth.FtthId = id;
            var res = await ApiHelper.ExecuteAsync<CsReportModel>("/csReport/create", new { id = id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            await GetDropDownListCsReport(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCsReport(CsReportModel csReport)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("csReport/create", csReport, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion
    }
}