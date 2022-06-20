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
using VTQT.SharedMvc.Master.Extensions;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Ticket.Models.CR;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class CRMxController : AdminMvcController
    {
        #region Fields

        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public CRMxController(IWorkContext workContext)
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
            var searchModel = new CRMxGridSearchModel();
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00006" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<CRModel>("cr-mx/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
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

            var resTicketArea = await ApiHelper.ExecuteAsync<List<TicketAreaModel>>("/ticket-area/get-available", null, Method.GET, ApiHosts.Ticket);
            var list4 = new List<SelectListItem>();
            if (resTicketArea.data?.Count > 0)
            {
                foreach (var item4 in resTicketArea.data)
                {
                    var tem4 = new SelectListItem()
                    {
                        Text = item4.Name,
                        Value = item4.Id
                    };
                    list4.Add(tem4);
                }
            }

            ViewData["ticketArea"] = list4;

            return View(searchModel);
        }

        public async Task<IActionResult> Create()
        {
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00006" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<CRModel>("cr-mx/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
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
            IEnumerable<ApprovalCRMxModel> approvalCRMxs,
            IEnumerable<ConfirmCRMxModel> confirmCRMxs,
            CRMxModel crMxModel)
        {
            model.CreatedDate = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.ApprovalCRMx.AddRange(approvalCRMxs);
            model.ConfirmCRMxs.AddRange(confirmCRMxs);
            model.CRMxModel = crMxModel;
            var res = await ApiHelper
                .ExecuteAsync("cr-mx/create", model, Method.POST, ApiHosts.Ticket);
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
        /// Gọi Api xóa cr/ crmx
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
                .ExecuteAsync("/cr-mx/deletes", ids, Method.POST, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<CRModel>("cr-mx/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            if (model != null)
            {
                model.ModifiedBy = _workContext.UserId;

                if (model.CRMxModel != null)
                {
                    model.CRMxModel.CRMxId = model.CRMxModel.Id;
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
            IEnumerable<ApprovalCRMxModel> approvalCRMxs,
            IEnumerable<ConfirmCRMxModel> confirmCRMxs,
            CRMxModel crMxModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }
            model.ApprovalCRMx.AddRange(approvalCRMxs);
            model.ConfirmCRMxs.AddRange(confirmCRMxs);
            model.CRMxModel = crMxModel;

            var res = await ApiHelper
                .ExecuteAsync("cr-mx/edit", model, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync<CRModel>("cr-mx/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
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
        /// Lấy danh sách cr/ crmx phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            CRMxGridSearchModel searchModel)
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

            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00006" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<CRMxGridModel>>("/cr-mx/get", searchModel, Method.GET, ApiHosts.Ticket);
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

        #region ConfirmCRMx

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmCRMx(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/confirm-cr-mx/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> AddConfirmCRMx()
        {
            var res = await ApiHelper.ExecuteAsync<ConfirmCRMxModel>("confirm-cr-mx/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ConfirmCRMxModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddConfirmCRMx(ConfirmCRMxModel confirmCRMx)
        {

            var res = await ApiHelper.ExecuteAsync("/confirm-cr-mx/create", confirmCRMx, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetConfirmCRMx(
            [DataSourceRequest] DataSourceRequest request,
            ConfirmCRMxSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<ConfirmCRMxModel>>("/confirm-cr-mx/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        public async Task<IActionResult> EditConfirmCRMx(string id)
        {
            var res = await ApiHelper.ExecuteAsync<ConfirmCRMxModel>("confirm-cr-mx/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ConfirmCRMxModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditConfirmCRMx(ConfirmCRMxModel editConfirmCRMx)
        {

            var res = await ApiHelper.ExecuteAsync("/confirm-cr-mx/edit", editConfirmCRMx, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion ConfirmCRMx

        #region ApprovalCRMx

        [HttpPost]
        public async Task<IActionResult> DeleteApprovalMx(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/approval-cr-mx/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetApprovalCRMx(
            [DataSourceRequest] DataSourceRequest request,
            ApprovalCRMxSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<ApprovalCRMxModel>>("/approval-cr-mx/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        public async Task<IActionResult> AddApprovalMx()
        {
            var res = await ApiHelper.ExecuteAsync<ApprovalCRMxModel>("approval-cr-mx/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ApprovalCRMxModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddApprovalMx(ApprovalCRMxModel addApprovalMx)
        {

            var res = await ApiHelper.ExecuteAsync("/approval-cr-mx/create", addApprovalMx, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditApprovalMx(string id)
        {
            var res = await ApiHelper.ExecuteAsync<ApprovalCRMxModel>("approval-cr-mx/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ApprovalCRMxModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditApprovalMx(ApprovalCRMxModel editApprovalMx)
        {

            var res = await ApiHelper.ExecuteAsync("/approval-cr-mx/edit", editApprovalMx, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion ApprovalCRMx

        #region Utilities

        [HttpPost]
        public async Task<IActionResult> GetUserComment(string userId, string content, string crMxId)
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
                comment.CrMxId = crMxId;
            }

            var resCmt =
                await ApiHelper.ExecuteAsync<CommentModel>("comment/create", comment, Method.POST, ApiHosts.Ticket);

            return Ok(new XBaseResult
            {
                success = true,
                data = comment
            });
        }

        private void GetAvailableData()
        {
            var approverCRMx = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)Core.Domain.Ticket.Enum.ApproverCRMx.DeputyGeneralManagerTechnical).ToString(),
                    Text = Core.Domain.Ticket.Enum.ApproverCRMx.DeputyGeneralManagerTechnical.GetEnumDescription()
                }
            };
            ViewData["approverCRMx"] = approverCRMx;

            var confirmCR = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)Core.Domain.Ticket.Enum.ConfirmCR.Noc).ToString(),
                    Text = Core.Domain.Ticket.Enum.ConfirmCR.Noc.GetEnumDescription()
                }
            };
            ViewData["confirmCR"] = confirmCR;
        }

        private static string GetReason(IEnumerable<SelectListItem> vs, string Show)
        {
            if (Show is null)
                return "";
            var check = vs.FirstOrDefault(x => x.Value.Equals(Show));
            return check is null ? "" : check.Text;
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

            var result = await ApiHelper.ExecuteAsync<List<CRMxModel>>($"/cr-mx/detail-get?CrId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultApprovalCR = await ApiHelper.ExecuteAsync<List<ApprovalCRMxModel>>($"/approval-cr-mx/detail-get?CrMxId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultConfirmCR = await ApiHelper.ExecuteAsync<List<ConfirmCRMxModel>>($"/confirm-cr-mx/detail-get?CrMxId={id}", null, Method.GET, ApiHosts.Ticket);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || resultApprovalCR.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || resultConfirmCR.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            #endregion
            var stt = 1;
            var models = new List<CRMxRecallModel>();
            if (result.data?.Count > 0)
            {
                foreach (var order in result.data)
                {
                    var m = new CRMxRecallModel()
                    {
                        STT = stt,
                        CrArea = order.CrArea,
                        CrProvince = order.CrProvince,
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
            var dtInfo = new DataTable("CRMx");
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

            #region ApprovalCR

            GetAvailableData();
            var getApprovalCR = (IEnumerable<SelectListItem>)ViewData["approverCRMx"];
            var stt1 = 1;
            var models1 = new List<ApprovalCRRecallModel>();
            if (resultApprovalCR.data?.Count > 0)
            {
                foreach (var order1 in resultApprovalCR.data)
                {
                    var m1 = new ApprovalCRRecallModel()
                    {
                        STT = stt1,
                        Confirm = order1.Confirm,
                        ReasonDetail = order1.ReasonDetail,
                        Approver = GetReason(getApprovalCR, order1.Approver),
                        Progress = order1.Progress,
                    };
                    stt1++;
                    models1.Add(m1);
                }
            }


            var dtDataName1 = "Data1";
            var dtData1 = models1.ToDataTable();
            dtData1.TableName = dtDataName1;
            ds.Tables.Add(dtData1);

            #endregion

            #region ConfirmCR
            var getConfirmCR = (IEnumerable<SelectListItem>)ViewData["confirmCR"];
            var stt2 = 1;
            var models2 = new List<ConfirmCRRecallModel>();
            if (resultConfirmCR.data?.Count > 0)
            {
                foreach (var order2 in resultConfirmCR.data)
                {
                    var m2 = new ConfirmCRRecallModel()
                    {
                        STT = stt2,
                        Confirm = order2.Confirm,
                        ReasonDetail = order2.ReasonDetail,
                        Approver = GetReason(getConfirmCR, order2.Approver),
                        Progress = order2.Progress,
                    };
                    stt2++;
                    models2.Add(m2);
                }
            }


            var dtDataName2 = "Data2";
            var dtData2 = models2.ToDataTable();
            dtData2.TableName = dtDataName2;
            ds.Tables.Add(dtData2);
            #endregion

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/CrMx.xls");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();


            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", "cr-nx-" + res.data.Code + ".xlsx");

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

            var res = await ApiHelper.ExecuteAsync<List<CommentModel>>("comment/get-CrMx", new { CrMxId = id }, Method.GET, ApiHosts.Ticket);
            var comments = res.data;
            comments.Sort();
            ViewData["crMxId"] = id;
            ViewData["UserLogin"] = _workContext.UserId;

            return View(comments);
        }
        #endregion
    }
}