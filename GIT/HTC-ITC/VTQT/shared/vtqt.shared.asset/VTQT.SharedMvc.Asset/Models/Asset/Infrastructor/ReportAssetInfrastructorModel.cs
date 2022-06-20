using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class ReportAssetInfrastructorModel : BaseEntityModel
    {

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.Area")]
        public string Area { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Station")]
        public string StationCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.StationName")]
        public string StationName { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.Note")]
        public string Note { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.LongItude")]
        public string LongItude { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.LatItude")]
        public string LatItude { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Name")]
        public string Name { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Excel.Quantity")]
        public string Quantity { get; set; }


        [XBaseResourceDisplayName("WareHouse.Units.Fields.UnitName")]
        public string UnitName { get; set; }


        [XBaseResourceDisplayName("Common.Fields.CurrentStatus")]
        public string CurrentUsageStatus { get; set; }

    }
}