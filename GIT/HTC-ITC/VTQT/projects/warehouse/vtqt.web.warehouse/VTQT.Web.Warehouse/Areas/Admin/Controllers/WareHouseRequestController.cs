using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;
using System;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class WareHouseRequestController : AdminMvcController
    {
        #region
        private readonly string UserName = CommonHelper.GetAppSetting<string>("Jira:UserName");
        private readonly string PassWord = CommonHelper.GetAppSetting<string>("Jira:Password");
        private readonly string JiraUrl = CommonHelper.GetAppSetting<string>("Jira:Url");
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WareHouseRequestController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion

        #region List

        public IActionResult Index()
        {
            var searchModel = new WareHouseRequestSearchModel();

            return View(searchModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetJira()
        {
            var client = new RestClient("https://jira.htc-itc.vn");
            client.Authenticator = new HttpBasicAuthenticator("haivn", "H@iVN@2021");

            var request1 = new RestRequest("/rest/issueNav/1/issueTable", Method.POST);
            request1.AddParameter(p: new Parameter("startIndex", 0, ParameterType.QueryString));
            request1.AddParameter(p: new Parameter("jql", "project = QTXK AND resolution = Unresolved ORDER BY priority DESC, updated DESC", ParameterType.QueryString));
            request1.AddParameter(p: new Parameter("layoutKey", "list-view", ParameterType.QueryString));
            request1.AddHeader("X-Atlassian-Token", "no-check");
            request1.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            var r = client.Execute(request1);

            return Ok(new XBaseResult { success = r.IsSuccessful, data = r.Content, httpStatusCode = (int)r.StatusCode });
        }



        public async Task<IActionResult> GetIssuessJira(WarehouseBalanceSearchModel searchModel)
        {
          //  var user = "nhanth";
            var user = _workContext.UserName;
            var jql_1 = string.Format("assignee={0}+and+project=QTXK+and+status!=Closed", user);
            var jql_2 = string.Format("creator={0}+and+project=QTXK+and+status!=Closed", user);

            //var jql_1 = string.Format("assignee={0}+and+project=QLT+and+status!=Closed", user);
            //var jql_2 = string.Format("creator={0}+and+project=QLT+and+status!=Closed", user);
            // var jql_2 = string.Format("assignee={0}+and+project=QTXK+and+status!=Closed+or+creator={0}+and+project=QTXK+and+status!=Closed", user);
            if (searchModel.Keywords.HasValue())
            {
                jql_1 = jql_1 + "+and+key=" + searchModel.Keywords + "+order+by+created";
                jql_2 = jql_2 + "+and+key=" + searchModel.Keywords + "+order+by+created";
            }

            var jql = jql_1 + "+or+" + jql_2;
            jql = jql + "&startAt=" + (searchModel.PageIndex - 1) * searchModel.PageSize + "";

            var client = new RestClient(JiraUrl)
            {
                Authenticator = new HttpBasicAuthenticator(UserName, PassWord)
            };
            var request = new RestRequest("/rest/api/2/search?jql=" + jql + "", Method.GET);
            //  request.AddParameter("jql", jql);
         //   request.AddParameter("fields", "key,issuetype,project,created,priority,assignee,status,description,summary,creator,customfield_10300");
            //  request.AddParameter("startAt", searchModel.PageIndex * searchModel.PageSize);
            //   request.AddParameter("maxResults", searchModel.PageSize);
            var response = await client.ExecuteAsync(request);
            var result = new DataSourceResult();
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
            {
                var r = JsonConvert.DeserializeObject<JiraModels>(response.Content);
                if (r != null)
                {
                    var data = r.issues.Where(x => x.fields != null).Select(s => new JiraResponseModel
                    {

                        Key = s.key,
                        Link = JiraUrl + "/browse/" + s.key,
                        IssueType = s.fields.issuetype == null ? "" : s.fields.issuetype.name,
                        ProjectName = s.fields.project == null ? "" : s.fields.project.name,
                        CreatedDate = $"{s.fields.created:dd/MM/yyyy}",
                        Priority = s.fields.priority == null ? "" : s.fields.priority.name,
                        Status = s.fields.status == null ? "" : s.fields.status.name,
                        Summary = s.fields.summary,
                        Description = s.fields.description,
                        Creator = s.fields.creator == null ? "" : s.fields.creator.displayName,
                        Assignee = s.fields.assignee == null ? "" : s.fields.assignee.displayName,
                        Reason = s.fields.customfield_10300 == null ? "" : s.fields.customfield_10300.value,
                        RequestType = s.key.Contains("QTXK") ? "Xuất kho" : "Nhập kho",
                        ColorStatus = s.fields.status == null ? "" : s.fields.status.statusCategory.colorName,
                        // Warn=DateTime.Compare(DateTime.UtcNow,s.fields.created)
                        Warn = true

                    }).ToList();
                    result.Data = data;
                    result.Total = r.total;
                    return Ok(result);

                }
            }

            return Ok(result);

        }
        public async Task<IActionResult> GetIssueJira(WareHouseRequestSearchModel searchModel)
        {
           // var user = "nhanth";
            var user = _workContext.UserName;
            var jql_1 = string.Format("assignee={0}+and+project=QTXK+and+status!=Closed", user);
            var jql_2 = string.Format("creator={0}+and+project=QTXK+and+status!=Closed", user);
            // var jql_2 = string.Format("assignee={0}+and+project=QTXK+and+status!=Closed+or+creator={0}+and+project=QTXK+and+status!=Closed", user);
            if (searchModel.Keywords.HasValue())
            {
                jql_1 = jql_1 + "+and+key=" + searchModel.Keywords + "";
                jql_2 = jql_2 + "+and+key=" + searchModel.Keywords + "";
            }

            var jql = jql_1 + "+or+" + jql_2;
            jql = jql + "&startAt=" + (searchModel.PageIndex - 1) * searchModel.PageSize + "";

            var client = new RestClient(JiraUrl)
            {
                Authenticator = new HttpBasicAuthenticator(UserName, PassWord)
            };
            var request = new RestRequest("/rest/api/2/search?jql=" + jql + "", Method.GET);
            request.AddParameter("jql", jql);
            request.AddParameter("fields", "key,issuetype,project,created,priority,assignee,status,description,summary,creator,customfield_10300");
            request.AddParameter("startAt", searchModel.PageIndex * searchModel.PageSize);
            request.AddParameter("maxResults", searchModel.PageSize);
            var response = await client.ExecuteAsync(request);
            var result = new DataSourceResult();
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
            {
                var r = JsonConvert.DeserializeObject<JiraModels>(response.Content);
                if (r != null)
                {
                    var data = r.issues.Where(x => x.fields != null).Select(s => new JiraGetValue
                    {

                        Key = s.key,
                        Link = JiraUrl + "/browse/" + s.key,
                        IssueType = s.fields.issuetype == null ? "" : s.fields.issuetype.name,
                        ProjectName = s.fields.project == null ? "" : s.fields.project.name,
                        CreatedDate = $"{s.fields.created:dd/MM/yyyy HH:mm:ss}",
                        Priority = s.fields.priority == null ? "" : s.fields.priority.name,
                        Status = s.fields.status == null ? "" : s.fields.status.name,
                        Summary = s.fields.summary,
                        Description = s.fields.description,
                        Creator = s.fields.creator == null ? "" : s.fields.creator.displayName,
                        Assignee = s.fields.assignee == null ? "" : s.fields.assignee.displayName,
                        Reason = s.fields.customfield_10300 == null ? "" : s.fields.customfield_10300.value,
                        AssignBy = s.fields.status == null ? "" : s.fields.status.statusCategory.colorName

                    }).ToList();
                    result.Data = data;
                    result.Total = r.total;
                    return Ok(result);

                }
            }

            return Ok(result);

        }
        public async Task<IActionResult> GetIssuesJira(WareHouseRequestSearchModel searchModel)
        {
            var jql = "project=QTXK";
            if (searchModel.Keywords.HasValue())
                jql = jql + "&assignee=" + searchModel.Keywords.Trim();

            var client = new RestClient(JiraUrl)
            {
                Authenticator = new HttpBasicAuthenticator(UserName, PassWord)
            };
            var request = new RestRequest("/rest/api/2/search?" + jql + "", Method.GET);
            request.AddParameter("jql", jql);
            request.AddParameter("fields", "key,issuetype,project,created,priority,assignee,status,description,summary,creator,customfield_10300");
            request.AddParameter("startAt", searchModel.PageIndex * searchModel.PageSize);
            request.AddParameter("maxResults", searchModel.PageSize);
            var response = await client.ExecuteAsync(request);
            var result = new DataSourceResult();
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
            {
                var r = JsonConvert.DeserializeObject<JiraModels>(response.Content);
                if (r != null)
                {
                    var data = r.issues.Where(x => x.fields != null).Select(s => new JiraGetValue
                    {

                        Key = s.key,
                        Link = JiraUrl + "/browse/" + s.key,
                        IssueType = s.fields.issuetype == null ? "" : s.fields.issuetype.name,
                        ProjectName = s.fields.project == null ? "" : s.fields.project.name,
                        CreatedDate = $"{s.fields.created:dd/MM/yyyy HH:mm:ss}",
                        Priority = s.fields.priority == null ? "" : s.fields.priority.name,
                        Status = s.fields.status == null ? "" : s.fields.status.name,
                        Summary = s.fields.summary,
                        Description = s.fields.description,
                        Creator = s.fields.creator == null ? "" : s.fields.creator.displayName,
                        Assignee = s.fields.assignee == null ? "" : s.fields.assignee.displayName,
                        Reason = s.fields.customfield_10300 == null ? "" : s.fields.customfield_10300.value,
                        AssignBy = s.fields.status == null ? "" : s.fields.status.statusCategory.colorName
                    }).ToList();
                    result.Data = data;
                    result.Total = r.total;
                    return Ok(result);

                }
            }

            return Ok(result);

        }


        //private static string GetStatus(Fields fields)
        //{
        //    switch (fields.status.name)
        //    {
        //        case "Approved":
        //            return "Trưởng bộ phận đã phê duyệt yêu cầu xuất kho";
        //        case y:
        //            // code block
        //            break;
        //        default:
        //            // code block
        //            break;
        //    }
        //}
        #endregion List
    }
}