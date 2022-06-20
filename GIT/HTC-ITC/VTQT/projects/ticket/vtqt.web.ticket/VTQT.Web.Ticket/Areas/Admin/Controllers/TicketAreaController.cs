﻿using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class TicketAreaController : AdminMvcController
    {
        #region Ctor

        public TicketAreaController()
        {
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            var searchModel = new TicketAreaSearchModel();

            return View(searchModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<TicketAreaModel>("/ticket-area/details", new { id = id }, Method.GET,
                ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var res = await ApiHelper.ExecuteAsync<TicketAreaModel>("/ticket-area/create", null, Method.GET,
                ApiHosts.Ticket);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketAreaModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/ticket-area/create", model, Method.POST, ApiHosts.Ticket);
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
            var res = await ApiHelper.ExecuteAsync<TicketAreaModel>("/ticket-area/edit", new { id = id }, Method.GET,
                ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TicketAreaModel model)
        {
            ModelState.Remove("Code");
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var res = await ApiHelper.ExecuteAsync("/ticket-area/edit", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/ticket-area/deletes", ids, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Activates(ActivatesModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/ticket-area/activates", model, Method.POST, ApiHosts.Ticket);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methods

        #region Lists

        // TODO-Remove
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Get([DataSourceRequest] DataSourceRequest request,
            TicketAreaSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<TicketAreaModel>>("/ticket-area/get", searchModel, Method.GET,
                ApiHosts.Ticket);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount
            };
            return Ok(result);
        }

        #endregion
    }
}
