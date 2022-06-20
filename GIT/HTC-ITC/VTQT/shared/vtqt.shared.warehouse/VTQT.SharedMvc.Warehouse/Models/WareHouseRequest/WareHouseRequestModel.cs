using System;
using System.Collections.Generic;
using VTQT.Web.Framework;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class WareHouseRequestModel
    {
        [XBaseResourceDisplayName("WareHouseRequest.Fields.issueIds")]
        public int issueIds { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.issueKeys")]
        public  string issueKeys { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.WareHouseRequestStatus")]
        public int WareHouseRequestStatus { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Status")]
        public  int Status { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Created")]

        public DateTime Created { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.WareHouesId")]
        public string WareHouesId { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Updated")]

        public DateTime Updated { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Createdby")]
        public  string Createdby { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Assignee")]
        public  string Assignee { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Summary")]
        public string Summary { get; set; }

        [XBaseResourceDisplayName("WareHouseRequest.Fields.Reference")]
        public List<Reference> Reference { get; set; }
    }
}