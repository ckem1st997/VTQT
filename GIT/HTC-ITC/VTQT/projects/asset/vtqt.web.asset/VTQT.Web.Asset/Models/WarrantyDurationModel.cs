using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Web.Framework;

namespace VTQT.Web.Asset.Models
{
    public class WarrantyDurationModel
    {
        [XBaseResourceDisplayName("Asset.Assets.Fields.Name", "Name")]
        public string Name { get; set; }
        [XBaseResourceDisplayName("Asset.Assets.Fields.Code", "Code")]
        public string Code { get; set; }
        [XBaseResourceDisplayName("Common.Fields.AssetCategory", "CategoryId")]
        public string CategoryId { get; set; }
        [XBaseResourceDisplayName("Common.Fields.BalanceQuantity", "BalanceQuantity")]
        public decimal BalanceQuantity { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.WhereAsset", "WhereAsset")]
        public string WhereAsset { get; set; }       
        
        [XBaseResourceDisplayName("Asset.Assets.Fields.LimitDate", "LimitDate")]
        public string LimitDate { get; set; }
    }
}
