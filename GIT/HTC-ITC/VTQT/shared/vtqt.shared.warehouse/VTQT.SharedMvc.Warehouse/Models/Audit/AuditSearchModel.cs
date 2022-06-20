using System;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class AuditSearchModel : BaseSearchModel
    {
        public string RouteKey { get; set; }

        public string WareHouesId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.FromDate")]
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.EmployeeName")]
        public string EmployeeId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CustomerName")]
        public string CustomerCode { get; set; }

        public string StrFromDate { get; set; }

        public string StrToDate { get; set; }
    }
}
