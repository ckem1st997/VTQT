using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using VTQT.Core.Infrastructure;
using VTQT.Core.Logging;
using VTQT.Web.Framework.Extensions;

namespace VTQT.Web.Framework.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MvcNotifyAttribute : ActionFilterAttribute
    {
        public const string NotificationsKey = "xbase.notifications.all";

        // Tạm comment lại vì đang không tự Property Injection được,
        // mà khi Resolve ở ctor thì không có Entries => Resolve tại scope của Action
        //public IMvcNotifier MvcNotifier { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var mvcNotifier = EngineContext.Current.Resolve<IMvcNotifier>();

            if (mvcNotifier == null || !mvcNotifier.Entries.Any())
                return;

            var controller = (Controller)filterContext.Controller;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HandleAjaxRequest(mvcNotifier.Entries.FirstOrDefault(), filterContext.HttpContext.Response);

                // Check if Pjax
                bool.TryParse(filterContext.HttpContext.Request.Headers[Mvc.Pjax.Constants.PjaxHeader], out var isPjax);
                if (isPjax)
                {
                    PersistViewData(controller.ViewData, mvcNotifier.Entries.Where(x => x.Durable == false));
                    PersistTempData(controller.TempData, mvcNotifier.Entries.Where(x => x.Durable == true));
                }

                return;
            }

            PersistViewData(controller.ViewData, mvcNotifier.Entries.Where(x => x.Durable == false));
            PersistTempData(controller.TempData, mvcNotifier.Entries.Where(x => x.Durable == true));

            mvcNotifier.Entries.Clear();
        }

        private void PersistViewData(ViewDataDictionary viewData, IEnumerable<MvcNotifyEntry> source)
        {
            if (!source.Any())
                return;

            var existing = (viewData[NotificationsKey] ?? new HashSet<MvcNotifyEntry>()) as HashSet<MvcNotifyEntry>;

            source.Each(x =>
            {
                if (x.Message.Text.HasValue())
                    existing.Add(x);
            });

            viewData[NotificationsKey] = TrimSet(existing);
        }

        private void PersistTempData(ITempDataDictionary tempData, IEnumerable<MvcNotifyEntry> source)
        {
            if (!source.Any())
                return;

            //var existing = (bag[NotificationsKey] ?? new HashSet<MvcNotifyEntry>()) as HashSet<MvcNotifyEntry>;
            var existing = new HashSet<MvcNotifyEntry>();
            if (tempData.ContainsKey(NotificationsKey))
                existing = tempData.Get<HashSet<MvcNotifyEntry>>(NotificationsKey);

            source.Each(x =>
            {
                if (x.Message.Text.HasValue())
                    existing.Add(x);
            });

            //tempData[NotificationsKey] = TrimSet(existing);
            tempData.Put(NotificationsKey, TrimSet(existing));
        }

        private void HandleAjaxRequest(MvcNotifyEntry entry, HttpResponse response)
        {
            if (entry == null)
                return;

            response.Headers.Add("X-XBase-Message-Type", entry.Type.ToString().ToLower());
            response.Headers.Add("X-XBase-Message", Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.Message.Text)));
        }

        private HashSet<MvcNotifyEntry> TrimSet(HashSet<MvcNotifyEntry> entries)
        {
            if (entries.Count <= 20)
            {
                return entries;
            }

            return new HashSet<MvcNotifyEntry>(entries.Skip(entries.Count - 20));
        }
    }

}
