using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class MaintenanceModel : BaseEntityModel
    {
        [XBaseResourceDisplayName("Common.Fields.Employee")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.EmployeeName")]
        public string EmployeeName { get; set; }

        [XBaseResourceDisplayName("Asset.Maintenances.Fields.Action")]
        public string Action { get; set; }

        [XBaseResourceDisplayName("Asset.Maintenances.Fields.Content")]
        public string Content { get; set; }

        [XBaseResourceDisplayName("Asset.Maintenances.Fields.Amount")]
        public decimal Amount { get; set; }

        [XBaseResourceDisplayName("Asset.Maintenances.Fields.MaintenancedDate")]
        public DateTime MaintenancedDate { get; set; }

        public IList<MaintenanceDetailModel> MaintenanceDetails { get; set; }

        public IList<SelectListItem> AvailableActions { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        public MaintenanceModel()
        {
            AvailableActions = new List<SelectListItem>();
            AvailableUsers = new List<SelectListItem>();
            MaintenanceDetails = new List<MaintenanceDetailModel>();
        }
    }
}
