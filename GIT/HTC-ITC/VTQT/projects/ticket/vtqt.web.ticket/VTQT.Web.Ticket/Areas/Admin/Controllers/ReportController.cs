using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Web.Ticket.Areas.Admin.Controllers
{
    public class ReportController : AdminMvcController
    {

        #region Fields
        private readonly Dictionary<string, string> reportRoute;
        #endregion

        #region Ctor
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ReportController()
        {
            reportRoute = new Dictionary<string, string>
            {
                { "FTTH", "/report/ftth" },
                { "Channel", "/report/channel" },
                { "Ticket", "/report/ticket" },
                { "CR", "/report/cr" },
                { "ChannelDatetime", "/report/channel-datetime" },
            };
        }
        #endregion

        #region Methods

        /// <summary>
        /// Hàm Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var view = new ReportValueSearchModel() { RouteKey = reportRoute["FTTH"] };
            return View(view);
        }

        #endregion

        #region ListTree
        /// <summary>
        /// Gọi Api lấy cấu trúc cây báo cáo
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<ReportTreeModel>>("/report/get-report-list-by-appid", null, Method.GET, ApiHosts.Master);

            var data = res.data;
            IList<ReportTreeModel> cg = new List<ReportTreeModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    if (item.key == reportRoute["FTTH"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["Channel"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["Ticket"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["CR"])
                    {
                        cg.Add(item);
                    }
                    else if (item.key == reportRoute["ChannelDatetime"])
                    {
                        cg.Add(item);
                    }
                }
            }

            return Ok(cg);
        }

        /// <summary>
        /// Load partial view
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IActionResult> ListenTreeSelect(string key)
        {
            if (key == reportRoute["FTTH"])
            {
                return PartialView("ReportFTTH", new ReportValueSearchModel() { RouteKey = reportRoute["FTTH"] });
            }
            else if (key == reportRoute["Channel"])
            {
                return PartialView("ReportChannel", new ReportValueSearchModel() { RouteKey = reportRoute["Channel"] });
            }
            else if (key == reportRoute["ReportTicket"])
            {
                return PartialView("ReportTicket", new ReportValueSearchModel() { RouteKey = reportRoute["Ticket"] });
            }
            else if (key == reportRoute["ReportChannelDatetime"])
            {
                return PartialView("ReportChannelDatetime", new ReportValueSearchModel() { RouteKey = reportRoute["ChannelDatetime"] });
            }
            else if (key == reportRoute["ReportCR"])
            {
                return PartialView("ReportCR", new ReportValueSearchModel() { RouteKey = reportRoute["CR"] });
            }
            return View();
        }
        #endregion ListTree
    }
}