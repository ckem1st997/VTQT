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
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Master.Models;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class StationTicketController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public StationTicketController(IWorkContext workContext)
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
            var searchModel = new StationTicketGridSearchModel();
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00002" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<TicketModel>("station-ticket/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            ViewData["user"] = model.AvailableUsers;
            ViewData["priority"] = model.AvailablePriorities;
            ViewData["status"] = model.AvailableStatus;
            ViewData["ticketArea"] = model.StationTicketModel.AvailableTicketAreas;
            ViewData["station"] = model.AvailableStations;
            ViewData["ticketProvince"] = model.StationTicketModel.AvailableTicketProvinces;

            return View(searchModel);
        }

        public async Task<IActionResult> Create()
        {
            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00002" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<TicketModel>("station-ticket/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            if (model != null)
            {
                model.CreatedBy = model.ModifiedBy = model.Assignee = _workContext.UserId;
                model.CreatedDate = model.ModifiedDate = DateTime.UtcNow.ToLocalTime();
                model.ProjectId = projectModel?.Id;
                model.StartDate = DateTime.UtcNow.ToLocalTime();
                var resItem = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("warehouse-item/get-available", null,
                    Method.GET, ApiHosts.Warehouse);
                if (resItem.data?.Count > 0)
                {
                    resItem.data.ForEach(item =>
                    {
                        var x = new SelectListItem
                        {
                            Value = item.Code,
                            Text = $"[{item.Code}] {item.Name}"
                        };
                        model.AvailableItems.Add(x);
                    });
                }
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TicketModel model,            
            StationTicketModel stationTicketModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.StationTicketModel = stationTicketModel;

            var res = await ApiHelper
                .ExecuteAsync("station-ticket/create", model, Method.POST, ApiHosts.Ticket);
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
        /// Gọi Api xóa ticket/ ticket sự cố
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
                .ExecuteAsync("/station-ticket/deletes", ids, Method.POST, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<TicketModel>("station-ticket/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            if (model != null)
            {
                model.ModifiedBy = _workContext.UserId;
                var resItem = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("warehouse-item/get-available", null,
                    Method.GET, ApiHosts.Warehouse);
                if (resItem.data?.Count > 0)
                {
                    resItem.data.ForEach(item =>
                    {
                        var x = new SelectListItem
                        {
                            Value = item.Code,
                            Text = $"[{item.Code}] {item.Name}"
                        };
                        model.AvailableItems.Add(x);
                    });
                }

                if (model.StationTicketModel != null)
                {
                    model.StationTicketModel.StationTicketId = model.StationTicketModel.Id;
                }

                model.Comments?.Sort();
                model.UserLogin = _workContext.UserId;
                model.StartDate = model.StartDate.ToLocalTime();
                model.FinishDate = model.FinishDate.ToLocalTime();
                model.Deadline = model.Deadline.ToLocalTime();
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(
            TicketModel model,
            StationTicketModel stationTicketModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.StationTicketModel = stationTicketModel;

            var res = await ApiHelper
                .ExecuteAsync("station-ticket/edit", model, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync<TicketModel>("station-ticket/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            if (model != null)
            {
                var resItem = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("warehouse-item/get-available", null,
                    Method.GET, ApiHosts.Warehouse);
                if (resItem.data?.Count > 0)
                {
                    resItem.data.ForEach(item =>
                    {
                        var x = new SelectListItem
                        {
                            Value = item.Code,
                            Text = $"[{item.Code}] {item.Name}"
                        };
                        model.AvailableItems.Add(x);
                    });
                }

                model.Comments?.Sort();
                model.UserLogin = _workContext.UserId;
                model.StartDate = model.StartDate.ToLocalTime();
                model.FinishDate = model.FinishDate.ToLocalTime();
                model.Deadline = model.Deadline.ToLocalTime();
            }

            return View(model);
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách ticket/ ticket sự cố phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get(
            [DataSourceRequest] DataSourceRequest request,
            StationTicketGridSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.StrStartDate = searchModel.StartDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrFinishDate = searchModel.FinishDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00002" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<StationTicketGridModel>>("/station-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            if (data?.Count > 0)
            {
                data.ForEach(x =>
                {
                    if (x.Duration <= 0) return;
                    var t = TimeSpan.FromSeconds(x.Duration);
                    x.DurationTime = t.ToString(@"dd") + " ngày - " + t.ToString(@"hh\:mm\:ss");
                });
            }

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetNetworkLinkTicket(
            [DataSourceRequest] DataSourceRequest request,
            NetworkLinkTicketSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<NetworkLinkTicketModel>>("/network-link-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetChannelTicket(
            [DataSourceRequest] DataSourceRequest request,
            ChannelTicketSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<ChannelTicketModel>>("/channel-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetDeviceTicket(
            [DataSourceRequest] DataSourceRequest request,
            DeviceTicketSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<DeviceTicketModel>>("/device-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetApprovalTicket(
            [DataSourceRequest] DataSourceRequest request,
            ApprovalTicketSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<ApprovalTicketModel>>("/approval-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetInfrastructorFeeTicket(
            [DataSourceRequest] DataSourceRequest request,
            InfrastructorSearchFeeModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<InfrastructorFeeModel>>("/infrastructor-fee/get", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }
        #endregion

        #region AddingItems

        public async Task<IActionResult> AddNetworkLink()
        {
            var res = await ApiHelper.ExecuteAsync<NetworkLinkTicketModel>("network-link-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new NetworkLinkTicketModel());
        }

        public async Task<IActionResult> AddChannel()
        {
            var res = await ApiHelper.ExecuteAsync<ChannelTicketModel>("channel-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ChannelTicketModel());
        }

        public async Task<IActionResult> AddDevice()
        {
            var res = await ApiHelper.ExecuteAsync<DeviceTicketModel>("device-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new DeviceTicketModel());
        }

        public async Task<IActionResult> AddApproval()
        {
            var res = await ApiHelper.ExecuteAsync<ApprovalTicketModel>("approval-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ApprovalTicketModel());
        }

        public async Task<IActionResult> AddInfrastructorFee()
        {
            var res = await ApiHelper.ExecuteAsync<InfrastructorFeeModel>("infrastructor-fee/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            if (model != null)
            {
                var resItem = await ApiHelper.ExecuteAsync<List<WareHouseItemModel>>("warehouse-item/get-available", null,
                    Method.GET, ApiHosts.Warehouse);
                if (resItem.data?.Count > 0)
                {
                    resItem.data.ForEach(item =>
                    {
                        var x = new SelectListItem
                        {
                            Value = item.Code,
                            Text = $"[{item.Code}] {item.Name}"
                        };
                        model.AvailableItems.Add(x);
                    });
                }
            }

            return View(model ?? new InfrastructorFeeModel());
        }

        #endregion

        #region Assign

        public async Task<IActionResult> Assign(string assignee)
        {
            var res = await ApiHelper.ExecuteAsync<AssignmentModel>("assignment/create", null, Method.GET,
                ApiHosts.Ticket);

            var model = res.data;
            model.Performer = assignee;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(AssignmentModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper
                .ExecuteAsync("assignment/create", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { success = true });
        }
        #endregion

        #region Utilities

        public async Task<IActionResult> GetDetailReasons(string search, int page, string reasonId)
        {
            var res = await ApiHelper.ExecuteAsync<List<SelectListItem>>(
                $"/ticket-reason/get-list-detail?reasonId={reasonId}", null, Method.GET, ApiHosts.Ticket);

            var data = res.data;
            var totalCount = data != null ? data.Count : 0;

            if (!string.IsNullOrEmpty(search) && data?.Count > 0)
            {
                data = data.Where(x => x.Text.ToLower().Contains(search.ToLower())).ToList();
            }

            data = data.Skip((page - 1) * 10).Take(10).ToList();

            return Ok(new XBaseResult
            {
                data = data,
                totalCount = totalCount
            });
        }

        public async Task<IActionResult> GetDetailReason(string id)
        {
            var res = await ApiHelper
                .ExecuteAsync<TicketReasonModel>("/ticket-reason/get-by-id", new { id = id }, Method.GET, ApiHosts.Ticket);

            var reason = res.data;
            return Ok(reason);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserComment(string userId, string content, string ticketId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<UserModel>("user/get-by-id", new { id = userId }, Method.GET,
                ApiHosts.Master);

            var user = res.data;
            var comment = new CommentModel();
            if (user != null)
            {
                comment.UserId = userId;
                comment.UserName = user.FullName;
                comment.CreatedDate = DateTime.Now;
                comment.Content = content;
                comment.StrCreatedDate = comment.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                comment.TicketId = ticketId;
            }

            var resCmt =
                await ApiHelper.ExecuteAsync<CommentModel>("comment/create", comment, Method.POST, ApiHosts.Ticket);

            return Ok(new XBaseResult
            {
                success = true,
                data = comment
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetNetworkLink(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<NetworkLinkModel>("network-link/get-by-id", new { id = id },
                Method.GET, ApiHosts.Ticket);
            var model = res.data;

            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetChannel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<ChannelModel>("channel/get-by-id", new { id = id },
                Method.GET, ApiHosts.Ticket);
            var model = res.data;

            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetDevice(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<DeviceModel>("device/get-by-id", new { id = id },
                Method.GET, ApiHosts.Ticket);
            var model = res.data;

            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetItem(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Ok(new XBaseResult
                {
                    success = false
                });
            }

            var res = await ApiHelper.ExecuteAsync<WareHouseItemModel>("warehouse-item/get-by-code", new { code = code },
                Method.GET, ApiHosts.Warehouse);
            var model = res.data;

            return Ok(new XBaseResult
            {
                data = model,
                success = true
            });
        }
        #endregion

        #region Files

        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            var files = Request.Form.Files;
            var ticketId = Request.Form["ticketId"];
            var ticketCode = Request.Form["ticketCode"];
            var fileUploads = new List<SharedMvc.Ticket.Models.FileModel>();

            if (files?.Count > 0)
            {
                foreach (var file in files)
                {
                    var dynamicPath = $"wwwroot/Uploads/Tickets/{DateTime.Now.ToString("yyyy/MM/dd")}/{ticketCode}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), dynamicPath);

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var f = new SharedMvc.Ticket.Models.FileModel
                    {
                        TicketId = ticketId,
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

        #region Excel
        [HttpGet]
        public async Task<IActionResult> GetExcel([FromQuery] StationTicketGridSearchModel searchModel)
        {
            searchModel.StrStartDate = searchModel.StartDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrFinishDate = searchModel.FinishDate?
                .ToString("s", CultureInfo.InvariantCulture);
            var stt = 1;

            var resProject = await ApiHelper.ExecuteAsync<SharedMvc.Ticket.Models.ProjectModel>("project/get-by-code", new { code = "00002" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<StationTicketExcelModel>>("/station-ticket/get-excel", searchModel, Method.GET, ApiHosts.Ticket);
            var data = res.data;

            if (data?.Count > 0)
            {
                data.ForEach(x =>
                {
                    x.Stt = stt++;
                });

                var ds = new DataSet();
                var dtInfo = new DataTable("Info");
                dtInfo.Columns.Add("Title", typeof(string));
                var infoRow = dtInfo.NewRow();
                infoRow["Title"] = "Báo cáo sự cố Trạm";
                dtInfo.Rows.Add(infoRow);
                ds.Tables.Add(dtInfo);

                var dtDataName = "Data";
                var dtData = data.ToDataTable();
                dtData.TableName = dtDataName;
                ds.Tables.Add(dtData);

                // var tmpPath = Path.GetFullPath("~/wwwroot/Templates/Reports/ReportDetail_vi.xlsx").Replace("~\\", "");
                var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/StationTicket.xlsx");

                var wb = new Workbook(tmpPath);
                var wd = new WorkbookDesigner(wb);
                wd.SetDataSource(dataSet: ds);
                wd.Process();
                wd.Workbook.CalculateFormula();

                var dstStream = new MemoryStream();
                wb.Save(dstStream, SaveFormat.Xlsx);
                dstStream.Seek(0, SeekOrigin.Begin);

                dstStream.Position = 0;
                return File(dstStream, "application/vnd.ms-excel", "report_ticket_tram.xlsx");
            }

            return Ok();
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

            var res = await ApiHelper.ExecuteAsync<List<CommentModel>>("comment/get", new { ticketId = id }, Method.GET, ApiHosts.Ticket);
            var comments = res.data;
            comments.Sort();
            ViewData["TicketId"] = id;
            ViewData["UserLogin"] = _workContext.UserId;

            return View(comments);
        }
        #endregion
    }
}
