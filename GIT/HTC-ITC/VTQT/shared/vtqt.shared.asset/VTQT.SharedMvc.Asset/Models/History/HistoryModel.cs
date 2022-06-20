using System;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class HistoryModel : BaseEntityModel
    {
        public string AssetId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Action")]
        public string Action { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Content { get; set; }

        public string User { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Date")]
        public DateTime TimeStamp { get; set; }
    }
}
