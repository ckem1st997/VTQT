using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Web.Framework;

namespace VTQT.Web.Warehouse.Models
{
    public class JiraGetValue
    {
        [XBaseResourceDisplayName("Common.Fields.Jira.Key")]

        public string Key { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Link")]

        public string Link { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.IssueType")]

        public string IssueType { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.ProjectName")]

        public string ProjectName { get; set; }
        [XBaseResourceDisplayName("common.fields.createddate")]

        public string CreatedDate { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Priority")]

        public string Priority { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.AssignBy")]

        public string AssignBy { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Status")]

        public string Status { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Summary")]

        public string Summary { get; set; }
        [XBaseResourceDisplayName("common.fields.description")]

        public string Description { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Reason")]
        public string Reason { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Creator")]

        public string Creator { get; set; }
        [XBaseResourceDisplayName("Common.Fields.Jira.Assignee")]

        public string Assignee { get; set; }
    }
}
