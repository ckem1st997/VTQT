using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public partial class MaintenanceDetailModel : BaseEntityModel
    {
        public string MaintenanceId { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AssetId")]
        public string AssetId { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Code")]
        public string AssetCode { get; set; }

        [XBaseResourceDisplayName("Asset.Assets.Fields.Name")]
        public string AssetName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.AssetCategory")]
        public string AssetCategoryId { get; set; }

        [XBaseResourceDisplayName("Asset.MaintenanceDetails.Fields.ReasonDescription")]
        public string ReasonDescription { get; set; }

        public IList<SelectListItem> AvailableAssets { get; set; }

        public MaintenanceDetailModel()
        {
            AvailableAssets = new List<SelectListItem>();
        }
    }
}
