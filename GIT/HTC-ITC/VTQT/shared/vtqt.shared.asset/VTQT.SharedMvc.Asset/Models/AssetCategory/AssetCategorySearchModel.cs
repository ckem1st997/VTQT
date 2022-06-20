using VTQT.Core.Domain;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Asset.Models
{
    public class AssetCategorySearchModel : BaseSearchModel
    {
        public ActiveStatus Status { get; set; }
    }
}
