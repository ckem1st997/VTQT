using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Web.Framework;

namespace VTQT.Web.Asset.Models
{
    public class ProjectBaseModel
    {
        [XBaseResourceDisplayName("asset.assets.fields.excel.organizationunitid", "OrganizationUnitName")]
        public string OrganizationUnitName { get; set; }

        [XBaseResourceDisplayName("asset.assetdecreased.fields.quantity", "Quantity")]

        public decimal Quantity { get; set; }

        [XBaseResourceDisplayName("Common.Fields.BrokenQuantity", "BrokenQuantity")]
        public decimal BrokenQuantity { get; set; } 
        
        [XBaseResourceDisplayName("Common.Fields.EndWarrantyDuration", "EndWarrantyDuration")]
        public decimal EndWarrantyDuration { get; set; }
        [XBaseResourceDisplayName("Common.Fields.SoldQuantity", "SoldQuantity")]
        public decimal SoldQuantity { get; set; }
        [XBaseResourceDisplayName("Common.Fields.RecallQuantity", "RecallQuantity")]
        public decimal RecallQuantity { get; set; }
    }
}
