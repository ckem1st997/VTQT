using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class ReportAssetValueSearchModel : BaseSearchModel
    {
        [XBaseResourceDisplayName("Common.Fields.FromDate")]
        public DateTime? FromDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ToDate")]
        public DateTime? ToDate { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.OrganizationUnitId")]
        public string OrganizationUnitId { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.ItemCode")]

        public string ItemCode { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string RouteKey { get; set; }

    }
}