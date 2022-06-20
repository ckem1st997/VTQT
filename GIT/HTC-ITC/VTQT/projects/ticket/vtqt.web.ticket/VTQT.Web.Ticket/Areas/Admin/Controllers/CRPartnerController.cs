using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Ticket.Models.CR;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class CRPartnerController : AdminMvcController
    {
        #region Fields

        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CRPartnerController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Khởi tạo trang Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var searchModel = new CRPartnerGridSearchModel();
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00005" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<CRModel>("cr-partner/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
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

            var resPriority = await ApiHelper.ExecuteAsync<List<CRCategoryModel>>("/cr-category/get-available", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var list2 = new List<SelectListItem>();
            if (resPriority.data?.Count > 0)
            {
                model.ProjectId = projectModel?.Id;
                foreach (var item2 in resPriority.data)
                {
                    var tem2 = new SelectListItem()
                    {
                        Text = item2.Name,
                        Value = item2.Id
                    };
                    list2.Add(tem2);
                }
            }

            ViewData["category"] = list2;

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

            return View(searchModel);
        }

        public async Task<IActionResult> Create()
        {
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00005" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<CRModel>("cr-partner/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            if (model != null)
            {
                model.CreatedBy = _workContext.UserId;
                model.CreatedDate = model.ModifiedDate = DateTime.UtcNow.ToLocalTime();
                model.ProjectId = projectModel?.Id;
                model.StartDate = DateTime.UtcNow.ToLocalTime();
                model.CreatedDate = DateTime.UtcNow.ToLocalTime();
                model.FinishDate = DateTime.UtcNow.ToLocalTime();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CRModel model,
            CRPartnerModel crPartnerModel)
        {
            model.CreatedDate = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.CRPartnerModel = crPartnerModel;
            var res = await ApiHelper
                .ExecuteAsync("cr-partner/create", model, Method.POST, ApiHosts.Ticket);
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
        /// Gọi Api xóa cr/ crPartner
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
                .ExecuteAsync("/cr-partner/deletes", ids, Method.POST, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<CRModel>("cr-partner/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            if (model != null)
            {
                model.ModifiedBy = _workContext.UserId;

                if (model.CRPartnerModel != null)
                {
                    model.CRPartnerModel.CRPartnerId = model.CRPartnerModel.Id;
                }

                model.Comments?.Sort();
                model.UserLogin = _workContext.UserId;
                model.StartDate = model.StartDate.ToLocalTime();
                model.CreatedDate = model.CreatedDate.ToLocalTime();
                model.FinishDate = model.FinishDate.ToLocalTime();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            CRModel model,
            CRPartnerModel crPartnerModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.CRPartnerModel = crPartnerModel;

            var res = await ApiHelper
                .ExecuteAsync("cr-partner/edit", model, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync<CRModel>("cr-partner/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
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
            }

            return View(model);
        }

        #endregion Methods

        #region List

        /// <summary>
        /// Lấy danh sách cr/ crPartner phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            CRPartnerGridSearchModel searchModel)
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

            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00005" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<CRPartnerGridModel>>("/cr-partner/get", searchModel, Method.GET, ApiHosts.Ticket);
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
        public async Task<IActionResult> GetUserComment(string userId, string content, string crPartnerId)
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
                comment.CrPartnerId = crPartnerId;
            }

            var resCmt =
                await ApiHelper.ExecuteAsync<CommentModel>("comment/create", comment, Method.POST, ApiHosts.Ticket);

            return Ok(new XBaseResult
            {
                success = true,
                data = comment
            });
        }

        #endregion Utilities

        #region Inphieu

        public async Task<ActionResult> ExportDone(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<CRModel>("/print/get-by-id-to-cr?id=" + id + "", null, Method.GET, ApiHosts.Ticket);
            var entity = res.data;
            var result = await ApiHelper.ExecuteAsync<List<CRPartnerModel>>($"/cr-partner/detail-get?CrId={id}", null, Method.GET, ApiHosts.Ticket);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion
            var stt = 1;
            var models = new List<CRPartnerRecallModel>();
            if (result.data?.Count > 0)
            {
                foreach (var order in result.data)
                {
                    var m = new CRPartnerRecallModel()
                    {
                        STT = stt,
                        StartTimeAction = order.StartTimeAction.ToString(),
                        RestoreTimeService = order.RestoreTimeService.ToString(),
                        HourTimeMinus = order.HourTimeMinus,
                        MinuteTimeMinus = order.MinuteTimeMinus,
                        SecondTimeMinus = order.SecondTimeMinus,
                        OverTimeRegister = order.OverTimeRegister,
                        Supervisor = order.Supervisor
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("CRPartner");
            dtInfo.Columns.Add("Code", typeof(string));
            dtInfo.Columns.Add("Name", typeof(string));
            dtInfo.Columns.Add("Category", typeof(string));
            dtInfo.Columns.Add("Status", typeof(string));
            dtInfo.Columns.Add("CreatedBy", typeof(string));
            dtInfo.Columns.Add("CreatedDate", typeof(string));
            dtInfo.Columns.Add("ModifiedBy", typeof(string));
            dtInfo.Columns.Add("ModifiedDate", typeof(string));
            dtInfo.Columns.Add("Detail", typeof(string));
            dtInfo.Columns.Add("Note", typeof(string));
            dtInfo.Columns.Add("StartDate", typeof(string));
            dtInfo.Columns.Add("FinishDate", typeof(string));
            dtInfo.Columns.Add("ImplementationSteps", typeof(string));
            dtInfo.Columns.Add("CrReason", typeof(string));
            dtInfo.Columns.Add("FieldHandler", typeof(string));

            var infoRow = dtInfo.NewRow();
            infoRow["Code"] = models.Any() ? res.data.Code : string.Empty;
            infoRow["Name"] = models.Any() ? res.data.Name : string.Empty;
            infoRow["Category"] = models.Any() ? res.data.Category : string.Empty;
            infoRow["Status"] = models.Any() ? res.data.Status : string.Empty;
            infoRow["CreatedBy"] = models.Any() ? res.data.CreatedBy : string.Empty;
            infoRow["CreatedDate"] = res.data.CreatedDate.ToString();
            infoRow["ModifiedBy"] = models.Any() ? res.data.ModifiedBy : string.Empty;
            infoRow["ModifiedDate"] = res.data.ModifiedDate.ToString();
            infoRow["Detail"] = res.data.Detail.ToString();
            infoRow["Note"] = res.data.Note.ToString();
            infoRow["StartDate"] = res.data.StartDate.ToString();
            infoRow["FinishDate"] = res.data.FinishDate.ToString();
            infoRow["ImplementationSteps"] = models.Any() ? res.data.ImplementationSteps : string.Empty;
            infoRow["CrReason"] = models.Any() ? res.data.CrReason : string.Empty;
            infoRow["FieldHandler"] = models.Any() ? res.data.FieldHandler : string.Empty;

            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/CrPartnerExcel.xls");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();


            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", "cr-partner-" + res.data.Code + ".xlsx");

        }

        #endregion

        #region Files

        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            var files = Request.Form.Files;
            var crId = Request.Form["crId"];
            var crCode = Request.Form["crCode"];
            var fileUploads = new List<SharedMvc.Ticket.Models.FileModel>();

            if (files?.Count > 0)
            {
                foreach (var file in files)
                {
                    var dynamicPath = $"wwwroot/Uploads/CR/{DateTime.Now.ToString("yyyy/MM/dd")}/{crCode}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), dynamicPath);

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var f = new SharedMvc.Ticket.Models.FileModel
                    {
                        CrId = crId,
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

        #endregion

        #region Comment
        public async Task<IActionResult> Comment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync<List<CommentModel>>("comment/get-CrPartner", new { CrPartnerId = id }, Method.GET, ApiHosts.Ticket);
            var comments = res.data;
            comments.Sort();
            ViewData["crPartnerId"] = id;
            ViewData["UserLogin"] = _workContext.UserId;

            return View(comments);
        }
        #endregion
    }
}