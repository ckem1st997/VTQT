using System;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public partial class ReportSearchModel : BaseSearchModel
    {
        [XBaseResourceDisplayName("Common.Fields.FromDate")]
        public DateTime? FromDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ToDate")]
        public DateTime? ToDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Warehouse")]
        public string WareHouseId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.WarehouseItem")]
        public string WareHouseItemId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ReportType")]
        public int ReportType { get; set; }

        public string RouteKey { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepartmentName")]
        public int DepartmentId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ProjectName")]
        public int ProjectId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Proposer")]
        public string Proposer { get; set; }

        public string StrFromDate { get; set; }

        public string StrToDate { get; set; }
    }
}