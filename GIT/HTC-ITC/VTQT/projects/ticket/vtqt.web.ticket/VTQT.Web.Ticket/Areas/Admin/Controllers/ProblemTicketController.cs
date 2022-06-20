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
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Ticket.Models.ProblemTicket;
using FileModel = VTQT.SharedMvc.Ticket.Models.FileModel;
using ProjectModel = VTQT.SharedMvc.Ticket.Models.ProjectModel;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class ProblemTicketController : AdminMvcController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public ProblemTicketController(IWorkContext workContext)
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
            var searchModel = new ProblemTicketGridSearchModel();
            var resProject = await ApiHelper.ExecuteAsync<ProjectModel>("project/get-by-code", new { code = "00001" }, Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<TicketModel>("problem-ticket/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            ViewData["user"] = model.AvailableUsers;
            ViewData["priority"] = model.AvailablePriorities;
            ViewData["status"] = model.AvailableStatus;
            ViewData["ticketArea"] = model.ProblemTicketModel.AvailableTicketAreas;
            ViewData["ticketProvince"] = model.ProblemTicketModel.AvailableTicketProvinces;
            ViewData["organization"] = model.ProblemTicketModel.AvailableOrganizationUnits;

            return View(searchModel);
        }

        public async Task<IActionResult> Create()
        {
            var resProject = await ApiHelper.ExecuteAsync<ProjectModel>("project/get-by-code", new { code = "00001" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            var res = await ApiHelper.ExecuteAsync<TicketModel>("problem-ticket/create", new { projectId = projectModel?.Id }, Method.GET, ApiHosts.Ticket);
            var model = res.data;

            if (model != null)
            {
                model.CreatedBy = model.ModifiedBy = model.Assignee = _workContext.UserId;
                model.CreatedDate = model.ModifiedDate = model.CreatedDate.ToLocalTime();
                model.StartDate = model.StartDate.ToLocalTime();
                model.ProjectId = projectModel?.Id;
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
            IEnumerable<NetworkLinkTicketModel> networkLinkTickets,
            IEnumerable<ChannelTicketModel> channelTickets,
            IEnumerable<DeviceTicketModel> deviceTickets,
            IEnumerable<ApprovalTicketModel> approvalTickets,
            IEnumerable<InfrastructorFeeModel> feeTickets,
            ProblemTicketModel problemTicketModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.NetworkLinkTickets.AddRange(networkLinkTickets);
            model.ChannelTickets.AddRange(channelTickets);
            model.DeviceTickets.AddRange(deviceTickets);
            model.ApprovalTickets.AddRange(approvalTickets);
            model.InfrastructorFees.AddRange(feeTickets);
            model.ProblemTicketModel = problemTicketModel;

            var res = await ApiHelper
                .ExecuteAsync("problem-ticket/create", model, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync("/problem-ticket/deletes", ids, Method.POST, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<TicketModel>("problem-ticket/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
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

                if (model.ProblemTicketModel != null)
                {
                    model.ProblemTicketModel.ProblemTicketId = model.ProblemTicketModel.Id;
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
            IEnumerable<NetworkLinkTicketModel> networkLinkTickets,
            IEnumerable<ChannelTicketModel> channelTickets,
            IEnumerable<DeviceTicketModel> deviceTickets,
            IEnumerable<ApprovalTicketModel> approvalTickets,
            IEnumerable<InfrastructorFeeModel> feeTickets,
            ProblemTicketModel problemTicketModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            model.NetworkLinkTickets.AddRange(networkLinkTickets);
            model.ChannelTickets.AddRange(channelTickets);
            model.DeviceTickets.AddRange(deviceTickets);
            model.ApprovalTickets.AddRange(approvalTickets);
            model.InfrastructorFees.AddRange(feeTickets);
            model.ProblemTicketModel = problemTicketModel;

            var res = await ApiHelper
                .ExecuteAsync("problem-ticket/edit", model, Method.POST, ApiHosts.Ticket);
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
                .ExecuteAsync<TicketModel>("problem-ticket/edit", new { id = id }, Method.GET, ApiHosts.Ticket);
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
            ProblemTicketGridSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.StrStartDate = searchModel.StartDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrFinishDate = searchModel.FinishDate?
                .ToString("s", CultureInfo.InvariantCulture);

            var resProject = await ApiHelper.ExecuteAsync<ProjectModel>("project/get-by-code", new { code = "00001" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<ProblemTicketGridModel>>("/problem-ticket/get", searchModel, Method.GET, ApiHosts.Ticket);
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
        public async Task<IActionResult> GetApprovalDetailTicket(
            [DataSourceRequest] DataSourceRequest request,
            ApprovalTicketSearchModel searchModel)
        {
            searchModel.BindRequest(request);
            searchModel.PageSize = 1000;

            var res = await ApiHelper
                .ExecuteAsync<List<ApprovalTicketModel>>("/approval-ticket/get-detail", searchModel, Method.GET, ApiHosts.Ticket);
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

        [HttpGet]
        public async Task<IActionResult> AddNetworkLink(string date)
        {
            var res = await ApiHelper.ExecuteAsync<NetworkLinkTicketModel>("network-link-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            if (date != null && !string.IsNullOrEmpty(date) && date.Split('-').Length > 0)
            {
                model.StartDate = new DateTime(Int32.Parse(date.Split('-')[2]), Int32.Parse(date.Split('-')[1]), Int32.Parse(date.Split('-')[0]), Int32.Parse(date.Split('-')[3]), Int32.Parse(date.Split('-')[4]), Int32.Parse(date.Split('-')[5]));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNetworkLink(NetworkLinkTicketModel addNetworkLinkTicket)
        {

            var res = await ApiHelper.ExecuteAsync("/network-link-ticket/create", addNetworkLinkTicket, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult()
            {
                data = res.data
            });
        }

        public async Task<IActionResult> AddChannel(string date)
        {
            var res = await ApiHelper.ExecuteAsync<ChannelTicketModel>("channel-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            if (date != null && !string.IsNullOrEmpty(date) && date.Split('-').Length > 0)
            {
                model.StartDate = new DateTime(Int32.Parse(date.Split('-')[2]), Int32.Parse(date.Split('-')[1]), Int32.Parse(date.Split('-')[0]), Int32.Parse(date.Split('-')[3]), Int32.Parse(date.Split('-')[4]), Int32.Parse(date.Split('-')[5]));
            }

            return View(model ?? new ChannelTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddChannel(ChannelTicketModel addChannel)
        {

            var res = await ApiHelper.ExecuteAsync("/channel-ticket/create", addChannel, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> AddDevice(string date)
        {
            var res = await ApiHelper.ExecuteAsync<DeviceTicketModel>("device-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            if (date != null && !string.IsNullOrEmpty(date) && date.Split('-').Length > 0)
            {
                model.StartDate = new DateTime(Int32.Parse(date.Split('-')[2]), Int32.Parse(date.Split('-')[1]), Int32.Parse(date.Split('-')[0]), Int32.Parse(date.Split('-')[3]), Int32.Parse(date.Split('-')[4]), Int32.Parse(date.Split('-')[5]));
            }
            return View(model ?? new DeviceTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddDevice(DeviceTicketModel deviceTicket)
        {

            var res = await ApiHelper.ExecuteAsync("/device-ticket/create", deviceTicket, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> AddApproval()
        {
            var res = await ApiHelper.ExecuteAsync<ApprovalTicketModel>("approval-ticket/create", null,
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ApprovalTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddApproval(ApprovalTicketModel addApproval)
        {

            var res = await ApiHelper.ExecuteAsync("/approval-ticket/create", addApproval, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
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

        [HttpPost]
        public async Task<IActionResult> AddInfrastructorFee(InfrastructorFeeModel addFee)
        {

            var res = await ApiHelper.ExecuteAsync("/infrastructor-fee/create", addFee, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion

        #region EditItems
        public async Task<IActionResult> EditNetworkLink(string id)
        {
            var res = await ApiHelper.ExecuteAsync<NetworkLinkTicketModel>("network-link-ticket/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            model.StartDate = model.StartDate.ToLocalTime();
            model.FinishDate = model.FinishDate.ToLocalTime();

            return View(model ?? new NetworkLinkTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditNetworkLink(NetworkLinkTicketModel networkLinkTicket)
        {

            var res = await ApiHelper.ExecuteAsync("/network-link-ticket/edit", networkLinkTicket, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditChannel(string id)
        {
            var res = await ApiHelper.ExecuteAsync<ChannelTicketModel>("channel-ticket/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            model.StartDate = model.StartDate.ToLocalTime();
            model.FinishDate = model.FinishDate.ToLocalTime();

            return View(model ?? new ChannelTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditChannel(ChannelTicketModel editChannel)
        {

            var res = await ApiHelper.ExecuteAsync("/channel-ticket/edit", editChannel, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditDevice(string id)
        {
            var res = await ApiHelper.ExecuteAsync<DeviceTicketModel>("device-ticket/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);

            var model = res.data;
            model.StartDate = model.StartDate.ToLocalTime();
            model.FinishDate = model.FinishDate.ToLocalTime();

            return View(model ?? new DeviceTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditDevice(DeviceTicketModel editDevice)
        {

            var res = await ApiHelper.ExecuteAsync("/device-ticket/edit", editDevice, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditApproval(string id)
        {
            var res = await ApiHelper.ExecuteAsync<ApprovalTicketModel>("approval-ticket/edit", new { id = id },
                Method.GET, ApiHosts.Ticket);

            var model = res.data;

            return View(model ?? new ApprovalTicketModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditApproval(ApprovalTicketModel editApproval)
        {

            var res = await ApiHelper.ExecuteAsync("/approval-ticket/edit", editApproval, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditInfrastructorFee(string id)
        {
            var res = await ApiHelper.ExecuteAsync<InfrastructorFeeModel>("infrastructor-fee/edit", new { id = id },
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

        [HttpPost]
        public async Task<IActionResult> EditInfrastructorFee(InfrastructorFeeModel editFeeTicket)
        {

            var res = await ApiHelper.ExecuteAsync("/infrastructor-fee/edit", editFeeTicket, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }
        #endregion

        #region DeleteItems

        [HttpPost]
        public async Task<IActionResult> DeleteNetworkLink(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/network-link-ticket/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChannel(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/channel-ticket/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDevice(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/device-ticket/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApproval(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/approval-ticket/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFee(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/infrastructor-fee/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
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

        private void GetAvailableData()
        {
            var approverTickets = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)Core.Domain.Ticket.Enum.ApproverTicket.DeputyGeneralManagerTechnical).ToString(),
                    Text = Core.Domain.Ticket.Enum.ApproverTicket.DeputyGeneralManagerTechnical.GetEnumDescription()
                },
                new SelectListItem
                {
                    Value = ((int)Core.Domain.Ticket.Enum.ApproverTicket.DeputyGeneralManagerNetworkInfrastructor).ToString(),
                    Text = Core.Domain.Ticket.Enum.ApproverTicket.DeputyGeneralManagerNetworkInfrastructor.GetEnumDescription()
                }
            };
            ViewData["approverTicket"] = approverTickets;
        }

        private static string GetReason(IEnumerable<SelectListItem> vs, string Show)
        {
            if (Show is null)
                return "";
            var check = vs.FirstOrDefault(x => x.Value.Equals(Show));
            return check is null ? "" : check.Text;
        }

        private async Task GetAvailable()
        {
            #region whItem
            var res = await ApiHelper
                .ExecuteAsync<List<WareHouseModel>>("/warehouse-item/get-available", null, Method.GET, ApiHosts.Warehouse);

            var categories = new List<SelectListItem>();

            var data = res.data;

            if (data?.Count > 0)
            {
                foreach (var m in data)
                {
                    var item = new SelectListItem
                    {
                        Value = m.Code,
                        Text = m.Name
                    };
                    categories.Add(item);
                }
            }

            categories.OrderBy(e => e.Text);
            ViewData["warehouses"] = categories;

            #endregion

            #region Cable

            var res1 = await ApiHelper
                .ExecuteAsync<List<CableModel>>("/cable/get-available", null, Method.GET, ApiHosts.Ticket);

            var categories1 = new List<SelectListItem>();

            var data1 = res1.data;

            if (data1?.Count > 0)
            {
                foreach (var m1 in data1)
                {
                    var item1 = new SelectListItem
                    {
                        Value = m1.Id,
                        Text = m1.Name
                    };
                    categories1.Add(item1);
                }
            }

            categories1.OrderBy(e => e.Text);
            ViewData["cable"] = categories1;

            #endregion

            #region Phenomena

            var res2 = await ApiHelper
                .ExecuteAsync<List<SharedMvc.Ticket.PhenomenaModel>>("/phenomena/get-available", null, Method.GET, ApiHosts.Ticket);

            var categories2 = new List<SelectListItem>();

            var data2 = res2.data;

            if (data2?.Count > 0)
            {
                foreach (var m2 in data2)
                {
                    var item2 = new SelectListItem
                    {
                        Value = m2.Id,
                        Text = m2.Name
                    };
                    categories2.Add(item2);
                }
            }

            categories2.OrderBy(e => e.Text);
            ViewData["phenomena"] = categories2;

            #endregion
        }

        #endregion

        #region Files

        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            var files = Request.Form.Files;
            var ticketId = Request.Form["ticketId"];
            var ticketCode = Request.Form["ticketCode"];
            var fileUploads = new List<FileModel>();

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

                    var f = new FileModel
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
        public async Task<IActionResult> GetExcel([FromQuery] ProblemTicketGridSearchModel searchModel)
        {
            searchModel.StrStartDate = searchModel.StartDate?
                .ToString("s", CultureInfo.InvariantCulture);
            searchModel.StrFinishDate = searchModel.FinishDate?
                .ToString("s", CultureInfo.InvariantCulture);
            var stt = 1;

            var resProject = await ApiHelper.ExecuteAsync<ProjectModel>("project/get-by-code", new { code = "00001" },
                Method.GET, ApiHosts.Ticket);
            var projectModel = resProject.data;

            searchModel.ProjectId = projectModel?.Id;

            var res = await ApiHelper
                .ExecuteAsync<List<ProblemTicketExcelModel>>("/problem-ticket/get-excel", searchModel, Method.GET, ApiHosts.Ticket);
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
                infoRow["Title"] = "Báo cáo sự cố Mạng";
                dtInfo.Rows.Add(infoRow);
                ds.Tables.Add(dtInfo);

                var dtDataName = "Data";
                var dtData = data.ToDataTable();
                dtData.TableName = dtDataName;
                ds.Tables.Add(dtData);

                // var tmpPath = Path.GetFullPath("~/wwwroot/Templates/Reports/ReportDetail_vi.xlsx").Replace("~\\", "");
                var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/ProblemTicket.xlsx");

                var wb = new Workbook(tmpPath);
                var wd = new WorkbookDesigner(wb);
                wd.SetDataSource(dataSet: ds);
                wd.Process();
                wd.Workbook.CalculateFormula();

                var dstStream = new MemoryStream();
                wb.Save(dstStream, SaveFormat.Xlsx);
                dstStream.Seek(0, SeekOrigin.Begin);

                dstStream.Position = 0;
                return File(dstStream, "application/vnd.ms-excel", "report_ticket_suco.xlsx");
            }

            return Ok();
        }
        #endregion

        #region Inphieu

        public async Task<ActionResult> ExportDone(string? id)
        {
            #region Validation

            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var res = await ApiHelper.ExecuteAsync<TicketModel>("/print/get-by-id-to-ticket?id=" + id + "", null, Method.GET, ApiHosts.Ticket);
            var entity = res.data;

            var result = await ApiHelper.ExecuteAsync<List<ProblemTicketModel>>($"/problem-ticket/detail-get?ticketId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultNetworkLinkTicket = await ApiHelper.ExecuteAsync<List<NetworkLinkTicketModel>>($"/network-link-ticket/detail-get?ticketId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultChannelTicket = await ApiHelper.ExecuteAsync<List<ChannelTicketModel>>($"/channel-ticket/detail-get?ticketId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultDeviceTicket = await ApiHelper.ExecuteAsync<List<DeviceTicketModel>>($"/device-ticket/detail-get?ticketId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultApprovalTicket = await ApiHelper.ExecuteAsync<List<ApprovalTicketModel>>($"/approval-ticket/detail-get?ticketId={id}", null, Method.GET, ApiHosts.Ticket);

            var resultInfrastructorFee = await ApiHelper.ExecuteAsync<List<InfrastructorFeeModel>>($"/infrastructor-fee/detail-get?ticketId={id}", null, Method.GET, ApiHosts.Ticket);

            if (entity is null || result.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || resultNetworkLinkTicket.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || resultChannelTicket.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || resultDeviceTicket.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            if (entity is null || resultApprovalTicket.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            if (entity is null || resultInfrastructorFee.data is null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            #endregion
            var stt = 1;
            var models = new List<ProblemTicketRecallModel>();
            if (result.data?.Count > 0)
            {
                foreach (var order in result.data)
                {
                    var m = new ProblemTicketRecallModel()
                    {
                        STT = stt,
                        TicketArea = order.TicketArea,
                        TicketProvince = order.TicketProvince,
                        ChannelCapacity = order.ChannelCapacity,
                        EcalatePosition = order.EcalatePosition,
                        ProcessingUnit = order.ProcessingUnit,
                        KindOfReason = order.KindOfReason,
                        DetailReason = order.DetailReason,
                        ProcessingDepartment = order.ProcessingDepartment,
                        Sla = order.Sla,
                        HourTimeMinus = order.HourTimeMinus,
                        MinuteTimeMinus = order.MinuteTimeMinus,
                        SecondTimeMinus = order.SecondTimeMinus,
                    };
                    stt++;
                    models.Add(m);
                }
            }

            var ds = new DataSet();
            var dtInfo = new DataTable("ProblemTicket");
            dtInfo.Columns.Add("Code", typeof(string));
            dtInfo.Columns.Add("Subject", typeof(string));
            dtInfo.Columns.Add("Detail", typeof(string));
            dtInfo.Columns.Add("StartDate", typeof(string));
            dtInfo.Columns.Add("FinishDate", typeof(string));
            dtInfo.Columns.Add("Deadline", typeof(string));
            dtInfo.Columns.Add("Status", typeof(string));
            dtInfo.Columns.Add("Priority", typeof(string));
            dtInfo.Columns.Add("Category", typeof(string));
            dtInfo.Columns.Add("FirstReason", typeof(string));
            dtInfo.Columns.Add("LastReason", typeof(string));
            dtInfo.Columns.Add("Note", typeof(string));
            dtInfo.Columns.Add("Solution", typeof(string));
            dtInfo.Columns.Add("PendingHourTime", typeof(string));

            var infoRow = dtInfo.NewRow();
            infoRow["Code"] = models.Any() ? res.data.Code : string.Empty;
            infoRow["Subject"] = models.Any() ? res.data.Subject : string.Empty;
            infoRow["Detail"] = models.Any() ? res.data.Detail : string.Empty;
            infoRow["StartDate"] = res.data.StartDate.ToString();
            infoRow["FinishDate"] = res.data.FinishDate.ToString();
            infoRow["Deadline"] = res.data.Deadline.ToString();
            infoRow["Status"] = models.Any() ? res.data.Status : string.Empty;
            infoRow["Priority"] = models.Any() ? res.data.Priority : string.Empty;
            infoRow["Category"] = models.Any() ? res.data.Category : string.Empty;
            infoRow["FirstReason"] = models.Any() ? res.data.FirstReason : string.Empty;
            infoRow["LastReason"] = models.Any() ? res.data.LastReason : string.Empty;
            infoRow["Note"] = models.Any() ? res.data.Note : string.Empty;
            infoRow["Solution"] = models.Any() ? res.data.Solution : string.Empty;
            infoRow["PendingHourTime"] = res.data.PendingHourTime.ToString();

            dtInfo.Rows.Add(infoRow);
            ds.Tables.Add(dtInfo);

            var dtDataName = "Data";
            var dtData = models.ToDataTable();
            dtData.TableName = dtDataName;
            ds.Tables.Add(dtData);

            //GetData
            await GetAvailable();

            #region NetworkLinkTicket

            var getcable = (IEnumerable<SelectListItem>)ViewData["cable"];
            var getPhenomena = (IEnumerable<SelectListItem>)ViewData["phenomena"];
            var stt1 = 1;
            var models1 = new List<NetworkLinkTicketRecallModel>();
            if (resultNetworkLinkTicket.data?.Count > 0)
            {
                foreach (var order1 in resultNetworkLinkTicket.data)
                {
                    var m1 = new NetworkLinkTicketRecallModel()
                    {
                        STT = stt1,
                        CableId = GetReason(getcable, order1.CableId),
                        NetworkLinkId = order1.NetworkLinkId,
                        NetworkLinkName = order1.NetworkLinkName,
                        CategoryId = order1.CategoryId,
                        StartDate = order1.StartDate,
                        FinishDate = order1.FinishDate,
                        PhenomenaId = GetReason(getPhenomena, order1.PhenomenaId),
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

            #region ChannelTicket

            var getcableChannel = (IEnumerable<SelectListItem>)ViewData["cable"];
            var getPhenomenaChannel = (IEnumerable<SelectListItem>)ViewData["phenomena"];
            var stt2 = 1;
            var models2 = new List<ChannelTicketRecallModel>();
            if (resultChannelTicket.data?.Count > 0)
            {
                foreach (var order2 in resultChannelTicket.data)
                {
                    var m2 = new ChannelTicketRecallModel()
                    {
                        STT = stt2,
                        CableId = GetReason(getcableChannel, order2.CableId),
                        ChannelId = order2.ChannelId,
                        ChannelName = order2.ChannelName,
                        CategoryId = order2.CategoryId,
                        StartDate = order2.StartDate,
                        FinishDate = order2.FinishDate,
                        PhenomenaId = GetReason(getPhenomenaChannel, order2.PhenomenaId),
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

            #region DeviceTicket

            var getPhenomenaDevice = (IEnumerable<SelectListItem>)ViewData["phenomena"];
            var stt3 = 1;
            var models3 = new List<DeviceTicketRecallModel>();
            if (resultChannelTicket.data?.Count > 0)
            {
                foreach (var order3 in resultDeviceTicket.data)
                {
                    var m3 = new DeviceTicketRecallModel()
                    {
                        STT = stt3,
                        DeviceId = order3.DeviceId,
                        DeviceName = order3.DeviceName,
                        CategoryId = order3.CategoryId,
                        StartDate = order3.StartDate,
                        FinishDate = order3.FinishDate,
                        PhenomenaId = GetReason(getPhenomenaDevice, order3.PhenomenaId),
                    };
                    stt3++;
                    models3.Add(m3);
                }
            }


            var dtDataName3 = "Data3";
            var dtData3 = models3.ToDataTable();
            dtData3.TableName = dtDataName3;
            ds.Tables.Add(dtData3);
            #endregion

            #region ApprovalTicket

            GetAvailableData();
            var getApprovalTicket = (IEnumerable<SelectListItem>)ViewData["approverTicket"];
            var stt4 = 1;
            var models4 = new List<ApprovalTicketRecallModel>();
            if (resultApprovalTicket.data?.Count > 0)
            {
                foreach (var order4 in resultApprovalTicket.data)
                {
                    var m4 = new ApprovalTicketRecallModel()
                    {
                        STT = stt4,
                        Confirm = order4.Confirm,
                        ReasonDetail = order4.ReasonDetail,
                        Approver = GetReason(getApprovalTicket, order4.Approver),
                        Progress = order4.Progress,
                    };
                    stt4++;
                    models4.Add(m4);
                }
            }


            var dtDataName4 = "Data4";
            var dtData4 = models4.ToDataTable();
            dtData4.TableName = dtDataName4;
            ds.Tables.Add(dtData4);

            #endregion

            #region InfrastructorFee

            var getInfrastructorFee = (IEnumerable<SelectListItem>)ViewData["warehouses"];
            var stt5 = 1;
            var models5 = new List<InfrastructorFeeRecallModel>();
            if (resultInfrastructorFee.data?.Count > 0)
            {
                foreach (var order5 in resultInfrastructorFee.data)
                {
                    var m5 = new InfrastructorFeeRecallModel()
                    {
                        STT = stt5,
                        Code = order5.Code,
                        Name = order5.Name,
                        Description = order5.Description,
                        Fee = order5.Fee,
                        WareHouseItemCode = GetReason(getInfrastructorFee, order5.WareHouseItemCode),
                    };
                    stt5++;
                    models5.Add(m5);
                }
            }


            var dtDataName5 = "Data5";
            var dtData5 = models5.ToDataTable();
            dtData5.TableName = dtDataName5;
            ds.Tables.Add(dtData5);
            #endregion

            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/Ticket.xls");
            var wb = new Workbook(tmpPath);
            var wd = new WorkbookDesigner(wb);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();


            var dstStream = new MemoryStream();
            wb.Save(dstStream, Aspose.Cells.SaveFormat.Xlsx);
            dstStream.Seek(0, SeekOrigin.Begin);

            dstStream.Position = 0;
            return File(dstStream, "application/vnd.ms-excel", "ticket-" + res.data.Code + ".xlsx");

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