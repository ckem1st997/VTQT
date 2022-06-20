using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public class ReportValueSearchModel : BaseSearchModel
    {
        [XBaseResourceDisplayName("Common.Fields.FromDate")]
        public DateTime? FromDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ToDate")]
        public DateTime? ToDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Warehouse")]
        public string WareHouseId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarehouseItem")]
        public string WareHouseItemId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Proposer")]
        public string UserId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ProjectName")]
        public string ProjectId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepartmentName")]
        public string DepartmentId { get; set; }

        public string StrFromDate { get; set; }

        public string StrToDate { get; set; }

        public string RouteKey { get; set; }

        public IList<SelectListItem> AvailableWarehouses { get; set; }

        public IList<SelectListItem> AvailableItems { get; set; }

        public IList<SelectListItem> AvailableProjects { get; set; }

        public IList<SelectListItem> AvailableDepartments { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public ReportValueSearchModel()
        {
            AvailableDepartments = new List<SelectListItem>();
            AvailableItems = new List<SelectListItem>();
            AvailableProjects = new List<SelectListItem>();
            AvailableUsers = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }
    }
}
